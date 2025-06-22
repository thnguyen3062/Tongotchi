using DanielLochner.Assets.SimpleScrollSnap;
using Game;
using Game.Websocket;
using PathologicalGames;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISelectPetPopup : BasePetPopup
{
    public enum UIPetType
    {
        All = 0,
        FusionOnly = 1,
    }

    [SerializeField] private uint maxSlot = 5;
    [Header("Header")]
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private TextMeshProUGUI m_PetCountText;
    [SerializeField] private TextMeshProUGUI m_PetNameText;

    [Header("Frame References")]
    [SerializeField] private GameObject m_NormalSelectGO;
    [SerializeField] private Button m_FusionBtn;
    [SerializeField] private Transform m_Toggle;
    [SerializeField] private Transform m_EggSkeleton;
    [SerializeField] private ScrollRect m_scrollRect;
    [SerializeField] private Transform m_PaginationField;
    [SerializeField] private Transform m_RandomEggImage;
    [SerializeField] private SimpleScrollSnap m_ScrollSnap;
    [SerializeField] private Button m_SelectBtn;
    [SerializeField] private Button m_BuyBtn;

    [SerializeField] private GameObject m_PetInfo;
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Image m_ExpBar;
    [SerializeField] private Image m_status;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private GameObject m_NotifyGO;

    [Header("Sprites")]
    [SerializeField] private Sprite petStatusHappy;
    [SerializeField] private Sprite petStatusNormal;
    [SerializeField] private Sprite petStatusSad;
    [SerializeField] private Sprite petStatusHatching;

    [Header("Fusion Fields")]
    [SerializeField] private GameObject[] m_FusionUIGOArray;
    [SerializeField] private TextMeshProUGUI m_PetLvTmp;

    private UIPetType displayPetType;
    private SkeletonGraphic[] m_PetPreviewSkeleton;
    private int currentPetsCount = -1;
    private int currentSelectedID = -1;
    private Transform newPetTransform;
    private Transform newPetPagination;
    private int selectedIndex = 0;

    private double totalDuration;
    private double duration;

    private GamePetData[] currentPetsArray = new GamePetData[0];

    GamePetData currentData;

    private void FixedUpdate()
    {
        if (currentData != null && currentData.petPhase == PetPhase.Hatching && duration >= 0)
        {
            if (duration >= 0)
            {
                duration -= Time.fixedDeltaTime;
                TimeSpan elapsedTime = TimeSpan.FromSeconds(duration);
                m_LevelText.text = $"{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
            }
            else
            {
                duration = 0;
                m_LevelText.text = $"{0:D2}:{0:D2}";
            }
        }
    }

    protected override void OnViewShown()
    {
        m_SelectBtn.onClick.AddListener(SelectPet);
        m_BuyBtn.onClick.AddListener(BuyNewSlot);
        m_CloseBtn.onClick.AddListener(Close);
        m_FusionBtn.onClick.AddListener(StartFusion);
    }

    protected override void OnViewHidden()
    {
        m_SelectBtn.onClick.RemoveListener(SelectPet);
        m_BuyBtn.onClick.RemoveListener(BuyNewSlot);
        m_FusionBtn.onClick.RemoveListener(StartFusion);
        m_CloseBtn.onClick.RemoveListener(Close);
        ResetPopup(); 
    }

    public void Close()
    {
        Hide();
    }

    public void LoadPetsPopup(UIPetType petType, Func<bool> additionalCondition = null, Action onCompleted = null)
    {
        currentPetsCount = PlayerData.Instance.userInfo.pets.Count;
        m_scrollRect.gameObject.SetActive(false);
        bool condition = additionalCondition != null ? additionalCondition() : true;
        if (condition)
        {
            WebSocketRequestHelper.LoadAllPetOnce((pets) =>
            {
                displayPetType = petType;
                var sortedPets = pets.Where(p => (displayPetType == UIPetType.FusionOnly ? (p.petLevel >= 45 && p.petEvolveLevel == 3) : true)).OrderBy(p => DateTime.ParseExact(p.spawnTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).ToArray();
                currentPetsArray = sortedPets;
                LoadPetUIElements();

                foreach (var item in m_FusionUIGOArray)
                {
                    item.gameObject.SetActive(displayPetType == UIPetType.FusionOnly);
                }
                m_NormalSelectGO.SetActive(displayPetType != UIPetType.FusionOnly);
                if (displayPetType == UIPetType.FusionOnly)
                {
                    LoadPetPvPStats();
                }
                onCompleted?.Invoke();
            });
        }
    }

    public void OnSelectedPet()
    {
        SetupButton(m_ScrollSnap.CenteredPanel);
        UpdateText(m_ScrollSnap.CenteredPanel);
        UpdatePetInfo(m_ScrollSnap.CenteredPanel);
    }

    public void AddNewUIPetSlot(GamePetData newPet)
    {
        if (newPet == null) return;

        List<GamePetData> currentPetList = new List<GamePetData>(currentPetsArray) { newPet };
        currentPetsArray = currentPetList.ToArray();
        currentPetsCount = PlayerData.Instance.userInfo.pets.Count;

        // Load the skin of the latest pet.
        Transform previewTrans = Instantiate(m_EggSkeleton, m_ContentField);
        SkeletonGraphic skel = previewTrans.GetComponent<SkeletonGraphic>();
        skel.initialSkinName = $"egg{PlayerData.Instance.userInfo.pets.Last() + 1}";
        skel.Initialize(true);
        skel.AnimationState.SetAnimation(0, "idle", true);
        skel.SetMaterialDirty();

        Transform trans = Instantiate(m_Toggle, m_PaginationField);
        trans.GetComponent<Toggle>().group = m_PaginationField.GetComponent<ToggleGroup>();
        trans.SetSiblingIndex(currentPetsCount - 1);

        m_ScrollSnap.Add(previewTrans.gameObject, currentPetsCount > 1 ? currentPetsCount - 1 : 1, true);
        m_ScrollSnap.StartingPanel = selectedIndex;

        bool useSelectedPet = false;
        int index = useSelectedPet ? selectedIndex : currentPetsArray.Length - 1;

        currentData = currentPetsArray[index];

        if (currentPetsArray.Length >= 5)
        {
            Close();
        }

        SetupButton(index);
        UpdateText(index);
        UpdatePetInfo(index);
    }

    private void ResetPopup()
    {
        for (int i = m_ContentField.childCount - 1; i >= 0; i--)
        {
            Destroy(m_ContentField.GetChild(i).gameObject);
        }

        for (int i = m_PaginationField.childCount - 1; i >= 0; i--)
        {
            Destroy(m_PaginationField.GetChild(i).gameObject);
        }
        PoolManager.Pools["SelectPet"].DespawnAll();
        currentPetsArray = new GamePetData[0];
    }

    private void StartFusion()
    {
        ShowUIView<UISetupFusionPopup>().SetFirstPet(currentSelectedID);
        Close();
    }

    private void LoadPetUIElements()
    {
        if (currentPetsArray.Length == 0) return;

        m_PetPreviewSkeleton = new SkeletonGraphic[currentPetsArray.Length];
        bool canHaveEmptySlot = currentPetsArray.Length < maxSlot;
        for (int i = 0; i < currentPetsArray.Length; i++)
        {
            Transform previewTrans;
            if (currentPetsArray[i].petPhase == PetPhase.Hatching)
            {
                previewTrans = Instantiate(m_EggSkeleton, m_ContentField);
                SkeletonGraphic skel = previewTrans.GetComponent<SkeletonGraphic>();
                skel.initialSkinName = string.Format(PetAnimController.BASE_EGG_SPINE_SKIN, currentPetsArray[i].petId + 1);
                skel.Initialize(true);
                skel.AnimationState.SetAnimation(0, "idle", true);
                skel.SetMaterialDirty();
            }
            else
            {
                InstantiatePetPreview(currentPetsArray[i]);
            }
            Transform trans = Instantiate(m_Toggle, m_PaginationField);
            trans.GetComponent<Toggle>().group = m_PaginationField.GetComponent<ToggleGroup>();

            // Load an empty slot.
            if (i == currentPetsArray.Length - 1 && canHaveEmptySlot)
            {
                newPetTransform = Instantiate(m_RandomEggImage, m_ContentField);

                newPetPagination = Instantiate(m_Toggle, m_PaginationField);
                newPetPagination.GetComponent<Toggle>().group = m_PaginationField.GetComponent<ToggleGroup>();
            }
        }


        for (int i = 0; i < currentPetsArray.Length; i++)
        {
            if (PlayerData.Instance.data.selectedPetID == currentPetsArray[i].petId)
            {
                selectedIndex = i;
                break;
            }
        }

        currentSelectedID = currentPetsArray[selectedIndex].petId;
        m_ScrollSnap.StartingPanel = selectedIndex;

        SetupButton(selectedIndex);
        UpdateText(selectedIndex);
        UpdatePetInfo(selectedIndex);
        m_scrollRect.gameObject.SetActive(true);
    }

    private void SetupButton(int index)
    {
        m_SelectBtn.gameObject.SetActive(index < currentPetsArray.Length);
        m_BuyBtn.gameObject.SetActive(index >= currentPetsArray.Length);
        m_FusionBtn.gameObject.SetActive(displayPetType == UIPetType.All && index < currentPetsArray.Length && currentPetsArray[index].petLevel >= 45 && currentPetsArray[index].petEvolveLevel == 3);
    }

    private void UpdateText(int index)
    {
        if (index < currentPetsArray.Length)
        {
            m_PetCountText.text = $"Pet {index + 1}/{currentPetsArray.Length}";
            m_PetNameText.text = currentPetsArray[index].PetName;
            currentSelectedID = currentPetsArray[index].petId;
            bool isCurrentSelected = currentSelectedID == PlayerData.Instance.data.selectedPetID;
            m_SelectBtn.interactable = !isCurrentSelected;
            m_SelectBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = isCurrentSelected ? "Selected" : "Select";

            int petLevel = currentPetsArray[index].petLevel;
            int petEvolveLevel = currentPetsArray[index].petEvolveLevel;

            DetermineUIPetLevel(petLevel, petEvolveLevel, out int offset);
            int displayLevel = petLevel - offset;
            m_PetLvTmp.text = displayLevel.ToString();
        }
        else
        {
            m_PetCountText.text = $"Pet {index + 1}/{maxSlot}";
            m_PetNameText.text = "New Pet";
            m_SelectBtn.interactable = false;
            m_SelectBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Select";
            m_PetLvTmp.text = string.Empty;
        }
    }

    private void DetermineUIPetLevel(int petLevel, int petEvolveLevel, out int offset)
    {
        if (petLevel > GameUtils.THIRD_EVOLVE_LEVEL && petEvolveLevel < 3)
        {
            // Not evolved to 3rd yet, subtract 3rd evolve threshold
            offset = petLevel - GameUtils.THIRD_EVOLVE_LEVEL;
        }
        else if (petLevel > GameUtils.SECOND_EVOLVE_LEVEL && petEvolveLevel < 2)
        {
            // Not evolved to 2nd yet, subtract 2nd evolve threshold
            offset = petLevel - GameUtils.SECOND_EVOLVE_LEVEL;
        }
        else if (petLevel > GameUtils.FIRST_EVOLVE_LEVEL && petEvolveLevel < 1)
        {
            // Not evolved to 1st yet, subtract 1st evolve threshold
            offset = petLevel - GameUtils.FIRST_EVOLVE_LEVEL;
        }
        else
        {
            // Otherwise, show full level
            offset = 0;
        }
    }

    private void UpdatePetInfo(int index)
    {
        if (index < currentPetsArray.Length)
        {
            if (!m_PetInfo.activeSelf)
                m_PetInfo.SetActive(true);

            GamePetData data = currentPetsArray[index];
            currentData = data;
            float secondPassed = 0;
            if (data.lastSavedTime != null)
            {
                DateTime lastSavedTime = GameUtils.ParseTime(data.lastSavedTime);
                TimeSpan timeDifference = GameUtils.ParseTime(data.currentTime) - lastSavedTime;
                secondPassed = (float)timeDifference.TotalSeconds;
            }

            float hunger = data.status.hunger;
            float hygiene = data.status.hygiene;
            float happiness = data.status.happy;

            hunger -= CalculateReduction(hunger, secondPassed);
            hygiene -= CalculateReduction(hygiene, secondPassed);
            happiness -= CalculateReduction(happiness, secondPassed);

            data.SetPetStatus(happiness, hygiene, hunger);

            if (data.petPhase == PetPhase.Hatching)
            {
                m_HealthBar.fillAmount = 1;
                m_ExpBar.fillAmount = 0;
                m_LevelText.text = "";
                Debug.Log("Pet is hatching!");
            }
            else
            {
                m_HealthBar.fillAmount = data.status.healthValue / GameUtils.MAX_HEALTH_VALUE;
                m_ExpBar.fillAmount = data.petExp / data.GetPetMaxExp;
                DetermineUIPetLevel(data.petLevel, data.petEvolveLevel, out int offset);
                int displayLevel = data.petLevel - offset;
                m_LevelText.text = "Lvl " + displayLevel;
            }

            SetStatus(data);
            if (data.petPhase == PetPhase.Hatching)
            {
                WebSocketRequestHelper.GetTimeOnce((time) =>
                {
                    if (string.IsNullOrEmpty(data.targetTime))
                        data.targetTime = time;
                    var targetTime = GameUtils.ParseTime(data.targetTime);
                    var currentTime = GameUtils.ParseTime(time);
                    var elapsedTime = targetTime - currentTime;
                    duration = elapsedTime.TotalSeconds;
                });
            }
        }
        else
        {
            m_PetInfo.SetActive(false);
        }
    }

    private void SetStatus(GamePetData data)
    {
        if (data.petPhase == PetPhase.Hatching)
        {
            m_status.sprite = petStatusHatching;
            return;
        }
        if (data.status.healthValue >= 0.7f)
        {
            m_status.sprite = petStatusHappy;
        }
        else if (data.status.healthValue >= 0.3f)
        {
            m_status.sprite = petStatusNormal;
        }
        else
        {
            m_status.sprite = petStatusSad;
        }
    }

    private void SelectPet()
    {
        if (currentSelectedID == -1)
            return;

        if (currentSelectedID == PlayerData.Instance.data.selectedPetID)
            return;

        if (displayPetType == UIPetType.FusionOnly)
        {
            Close();
            GetUIView<UISetupFusionPopup>().SetSecondPet(currentSelectedID);
        }
        else
        {
            SelectPet(currentSelectedID, () => {
                Close();
                GameManager.Instance.LoadTargetTime();
            });
        }
    }

    private void BuyNewSlot()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        if (PlayerData.Instance.MaxPet)
            m_NotifyGO.SetActive(true);
        else
            ShowUIView<PopupConfirmPurchase>().SetOnConfirmPurchaseSlotCallback(ShowPurchaseResult).InitDataPurchaseSlotPet(PurchaseType.Slot, CurrencyType.Diamond, 4);
    }

    private void ShowPurchaseResult(bool isSuccess)
    {
        ShowUIView<PopupPurchaseResult>().InitDataForPetSlot(isSuccess, CurrencyType.Diamond, 4);
    }

    private float CalculateReduction(float status, float secondPassed)
    {
        return 100f / 21600f * secondPassed;
    }
}