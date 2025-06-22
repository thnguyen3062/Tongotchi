using Core.Utils;
using Game;
using Game.Websocket;
using Game.Websocket.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SelectPetType
{
    Attack,
    Defense
}

public class SelectPetPVP : BaseView
{
    [SerializeField] private TextMeshProUGUI tmpPetAmount;
    [Header("Pet info")]
    [SerializeField] private Image m_PetIcon;
    [SerializeField] private UINumericField m_HealthField;
    [SerializeField] private UINumericField m_AttackField;
    [SerializeField] private UINumericField m_LuckField;
    [SerializeField] private UINumericField m_SpeedField;
    [SerializeField] private TextMeshProUGUI m_TmpItemName;
    [SerializeField] private TextMeshProUGUI m_TmpItemDesc;
    [SerializeField] private ItemPvp m_ItemPvp;
    [SerializeField] private GameObject m_ItemInfoContainer;
    [Header("Buttons")]
    [SerializeField] private Button m_PrevBtn;
    [SerializeField] private Button m_NextBtn;
    [SerializeField] private Button m_ConfirmBtn;
    [SerializeField] private Button m_CloseBtn;

    private List<GamePetData> spawnedPets = new List<GamePetData>();
    private ICallback.CallFunc2<int> onSelectPetId;
    private int currentIndex = 0;
    protected int currentPetId;

    public SelectPetPVP SetOnSelectPetId(ICallback.CallFunc2<int> func) { onSelectPetId = func; return this; }

    protected override void OnViewShown()
    {
        m_PrevBtn.onClick.AddListener(OnPreviousPet);
        m_NextBtn.onClick.AddListener(OnNextPet);
        m_ConfirmBtn.onClick.AddListener(OnConfirm);
        m_CloseBtn.onClick.AddListener(Hide);
    }

    protected override void OnViewHidden()
    {
        m_PrevBtn.onClick.RemoveListener(OnPreviousPet);
        m_NextBtn.onClick.RemoveListener(OnNextPet);
        m_ConfirmBtn.onClick.RemoveListener(OnConfirm);
        m_CloseBtn.onClick.RemoveListener(Hide);
    }

    public void InitSelectPvp()
    {
        spawnedPets.Clear();
        currentIndex = 0;
        WebSocketRequestHelper.LoadAllPetOnce((pets) =>
        {
            foreach (var pet in pets)
            {
                if (pet.petEvolveLevel < 1)
                    continue;
                spawnedPets.Add(pet);
            }
            if (spawnedPets.Count > 0)
            {
                SetPetData(spawnedPets[currentIndex]);
                gameObject.SetActive(true);
            }
            else
            {
                Toast.Show("At least 1 pet evolve level 1");
                LoggerUtil.Logging("No pet available", TextColor.Red);
            }
        });
    }

    private void SetPetData(GamePetData pet)
    {
        SetPetIndex(currentIndex, spawnedPets.Count);

        m_PetIcon.sprite = Resources.Load<Sprite>($"PetPvp/IdleAnim/{(pet.petId > 4 ? 4 :  pet.petId)}_{(pet.petEvolveLevel > 3 ? 3: pet.petEvolveLevel)}_front_idle");

        PlayerData.Instance.CalculatePetStats(pet, out float healthStat, out float attackStat, out float speedStat, out float luckStat);
        m_HealthField.SetFloat(healthStat);
        m_SpeedField.SetFloat(speedStat);
        m_AttackField.SetFloat(attackStat);
        m_LuckField.SetFloat(luckStat);

        SetPetItemInfo(pet.equipe);
        currentPetId = pet.petId;
        onSelectPetId?.Invoke(pet.petId);
    }

    public void SetPetItemInfo(InventoryPvpItemData itemData)
    {
        if (itemData == null)
        {
            m_ItemInfoContainer.SetActive(false);
        }
        else
        {
            m_TmpItemName.text = itemData.itemName;
            m_ItemPvp.InitData(itemData);

            itemData.GetValuesOfCurrentLevel(out Dictionary<PvpSpecificItemCategory, float> statDictionary);

            string finalStr = "";

            foreach (var pair in statDictionary)
            {
                finalStr += $"<sprite={(int)pair.Key}> {(pair.Value >= 0 ? "+" : "-")}{Math.Round(pair.Value, 3)} ";
            }
            finalStr.Trim();
            m_TmpItemDesc.text = finalStr;
            m_ItemInfoContainer.SetActive(true);
        }
    }

    public void OnNextPet()
    {
        currentIndex++;
        if (currentIndex >= spawnedPets.Count)
            currentIndex = 0;

        SetPetData(spawnedPets[currentIndex]);
    }

    public void OnPreviousPet()
    {
        currentIndex--;
        if (currentIndex <= 0)
            currentIndex = spawnedPets.Count - 1;

        SetPetData(spawnedPets[currentIndex]);
    }

    public void SetPetIndex(int current, int max)
    {
        tmpPetAmount.text = $"{current + 1}/{max}";
    }

    protected virtual void OnConfirm()
    {
        Hide();
    }
}
