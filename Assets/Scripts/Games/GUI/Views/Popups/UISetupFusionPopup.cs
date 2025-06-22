using UnityEngine;
using Game;
using UnityEngine.UI;
using TMPro;
using Game.Websocket;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Game.Utils;

public class UISetupFusionPopup : BaseView
{
    [SerializeField] private PetFBF m_FisrtPet;
    [SerializeField] private PetFBF m_SecondPet;
    [SerializeField] private Button m_SecondPetBtn;
    [SerializeField] private Button m_OwnedBtn;
    [SerializeField] private Button m_RequiredBtn;
    [SerializeField] private TextMeshProUGUI m_OwnedTmp;
    [SerializeField] private TextMeshProUGUI m_RequiredTmp;
    [SerializeField] private Image m_FillImg;
    [SerializeField] private TextMeshProUGUI m_PercentTmp;
    [SerializeField] private Button m_FusionBtn;

    [SerializeField] private Sprite m_PotionThumbnail;
    [SerializeField] private Sprite m_EmptyPotionThumbnail;

    private int ownedPotionCount;
    private int requiredPotionCount;
    private InventoryItem currentPotion;
    private int firstPetId;
    private int secondPetId;

    protected override void OnViewShown()
    {
        base.OnViewShown();
        m_OwnedBtn.onClick.AddListener(StackPotion);
        m_RequiredBtn.onClick.AddListener(RemovePotion);
        m_SecondPetBtn.onClick.AddListener(ShowChooseFusionPet);
        m_FusionBtn.onClick.AddListener(StartFusion);

        GetUIView<UIMainHomePanel>().SetStatusFieldState(false);
        GameManager.Instance.PetController.gameObject.SetActive(false);
        LoadPotions();
    }

    protected override void OnViewHidden()
    {
        base.OnViewHidden();
        m_OwnedBtn.onClick.RemoveListener(StackPotion);
        m_RequiredBtn.onClick.RemoveListener(RemovePotion);
        m_SecondPetBtn.onClick.RemoveListener(ShowChooseFusionPet);
        m_FusionBtn.onClick.RemoveListener(StartFusion);

        m_FisrtPet.Refresh();
        m_SecondPet.Refresh();
        currentPotion = null;
    }

    public void ShowChooseFusionPet()
    {
        ShowUIView<UISelectPetPopup>().LoadPetsPopup(UISelectPetPopup.UIPetType.FusionOnly);
    }

    public void StackPotion()
    {
        if (ownedPotionCount > 0)
        {
            ownedPotionCount--;
            requiredPotionCount++;
        }

        UpdatePotionUI();
    }

    public void RemovePotion()
    {
        if (requiredPotionCount > 0)
        {
            ownedPotionCount++;
            requiredPotionCount--;
        }
        UpdatePotionUI();
    }

    public void SetFirstPet(int petId)
    {
        string path = $"PetPvp/IdleAnim/{petId}_{3}_front_idle";
        firstPetId = petId;
        m_FisrtPet.SetSprites(path);
    }

    public void SetSecondPet(int petId)
    {
        string path = $"PetPvp/IdleAnim/{petId}_{3}_front_idle";
        secondPetId = petId;
        m_SecondPet.SetSprites(path);
    }

    private void LoadPotions()
    {
        WebSocketRequestHelper.RequestLoadInventory((Dictionary<int, InventoryItem> items) => { 
            if (items != null && items.Count > 0)
            {
                if (items.TryGetValue(GameUtils.EVOLVE_POTION, out  InventoryItem item))
                {
                    currentPotion = item;
                    ownedPotionCount = currentPotion.quantity;
                }
                else
                {
                    currentPotion = null;
                    ownedPotionCount = 0;
                }
            }
            else
            {
                Debug.LogError("Inventory is null or empty!");
            }
            UpdatePotionUI();
        });
    }

    private void StartFusion()
    {
        int quantity = currentPotion.quantity;

        if (requiredPotionCount < quantity)
        {
            ShowUIView<UILoadingView>();
            LoggerUtil.Logging("StartFusion", $"FirstPet={firstPetId}\nSecondPet={secondPetId}\nPotionCount={requiredPotionCount}");

            WebSocketRequestHelper.RequestStartFusion(firstPetId, secondPetId, requiredPotionCount, (FusionStartResult result) => {
                if (result != null)
                {
                    if (result.Success)
                    {
                        StartCoroutine(DelayClaimFusion(result.FusionId));
                    }
                    else
                    {
                        ShowUIView<PopupLevelUpFailed>().InitFusionFailed(result.ErrorMessage, requiredPotionCount);
                        HideUIView<UILoadingView>();
                    }
                }
                else
                {
                    HideUIView<UILoadingView>();
                }
            });
        }
        else
        {
            ShowUIView<PopupNotify>().Init("EMPTY POTION", "Add more potion to fusion!", 0, false, false);
        }
    }

    private void UpdatePotionUI()
    {
        ownedPotionCount = Mathf.Clamp(ownedPotionCount, 0, 6);
        requiredPotionCount = Mathf.Clamp(requiredPotionCount, 0, 6);

        m_OwnedTmp.text = ownedPotionCount.ToString();
        m_RequiredTmp.text = requiredPotionCount.ToString();

        m_OwnedBtn.transform.GetChild(0).GetComponent<Image>().sprite = ownedPotionCount > 0 ? m_PotionThumbnail : m_EmptyPotionThumbnail;
        m_RequiredBtn.transform.GetChild(0).GetComponent<Image>().sprite = requiredPotionCount > 0 ? m_PotionThumbnail : m_EmptyPotionThumbnail;

        // Calculate the fusion success percentage here.
    }

    private IEnumerator DelayClaimFusion(string fusionId)
    {
        yield return new WaitForSeconds(1);
        PlayerData.Instance.ClaimFusion(fusionId, (result, entity) => {
            HideUIView<UILoadingView>();

            if (result.Success)
            {
                PlayerData.Instance.data.selectedPetID = result.NewPet.Code;
                BasePetPopup.SelectPet(result.NewPet.Code, () => {
                    ShowUIView<UIFusionSuccessPopup>().LoadFusionedPet(entity);
                });
            }
            else
            {
                ShowUIView<PopupLevelUpFailed>().InitFusionFailed(result.ErrorMessage, requiredPotionCount);
            }
        });
    }
}
