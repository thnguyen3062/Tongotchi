using Core.Utils;
using Game;
using Game.Websocket;
using Game.Websocket.Model;
using PathologicalGames;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PVPController : UIBaseHeaderOnly
{
    [SerializeField] private Transform m_PvpItemPrefab;
    [SerializeField] private Transform m_PvpItemParent;

    [SerializeField] private TextMeshProUGUI m_PetNameTmp;
    [SerializeField] private TextMeshProUGUI m_CupTmp;
    [SerializeField] private TextMeshProUGUI m_ItemNameTmp;

    [SerializeField] private PetPvPStatLoader m_StatLoader;
    [SerializeField] private Button m_PetItemBtn;

    [Header("Bottom Buttons")]
    [SerializeField] private Button m_LeaderboardBtn;
    [SerializeField] private Button m_SetDefensePetBtn;
    [SerializeField] private Button m_PvpShopBtn;
    [SerializeField] private Button m_FindBtn;
    [SerializeField] private Button m_ChangeBGBtn;

    private Transform curEquippedItem;
    private int currentSelectedPetId;

    protected override void OnViewShown()
    {
        m_PetItemBtn.onClick.AddListener(OnClickItem);

        // Bottom
        m_LeaderboardBtn.onClick.AddListener(OnClickLeaderBoard);
        m_PvpShopBtn.onClick.AddListener(ShowPvPShop);
        m_SetDefensePetBtn.onClick.AddListener(ChangePetDefender);
        m_FindBtn.onClick.AddListener(StartFindMatch);
        m_ChangeBGBtn.onClick.AddListener(ChangeBG);
    }

    protected override void OnViewHidden()
    {
        m_PetItemBtn.onClick.RemoveListener(OnClickItem);

        //Bottom 
        m_LeaderboardBtn.onClick.RemoveListener(OnClickLeaderBoard);
        m_PvpShopBtn.onClick.RemoveListener(ShowPvPShop);
        m_SetDefensePetBtn.onClick.RemoveListener(ChangePetDefender);
        m_FindBtn.onClick.RemoveListener(StartFindMatch);
        m_ChangeBGBtn.onClick.RemoveListener(ChangeBG);

        HideUIView<PopupItemPvp>();
        HideUIView<SelectPetDefender>();
        HideUIView<UISelectPetAttacker>();
        HideUIView<PvpShopManager>();
    }

    #region init pvp
    public void InitDataMainPvp()
    {
        //se dc luu trong pet
        GetData();
    }
    private void GetData()
    {
        m_StatLoader.LoadStats();
        InitEquipItem();
        ReloadPvpInventory();
    }

    public void ReloadPvpInventory()
    {
        WebSocketRequestHelper.QueryPvpInventoryOnce((Data, id) =>
        {
            PlayerData.Instance.LstUserItemsInventory.Clear();
            PlayerData.Instance.LstUserItemsInventory.AddRange(Data);
        });
    }
    #endregion

    #region Inventory
    private void InitEquipItem()
    {
        m_PetNameTmp.text = PlayerData.Instance.PetData.PetName;

        InventoryPvpItemData item = PlayerData.Instance.PetData.equipe;
        if (item == null)
        {
            LoggerUtil.Logging("InitEquipItem", "Pet does not have equipment!");
            return;
        }
        if (curEquippedItem != null)
            PoolManager.Pools["Popup"].Despawn(curEquippedItem);

        curEquippedItem = PoolManager.Pools["Popup"].Spawn(m_PvpItemPrefab, m_PvpItemParent);
        curEquippedItem.localPosition = Vector3.zero;
        curEquippedItem.GetComponent<ItemPvp>().InitData(item);

        item.GetValuesOfCurrentLevel(out Dictionary<PvpSpecificItemCategory, float> statDictionary);

        string finalStr = "";
        foreach (var pair in statDictionary)
        {
            finalStr += $"<sprite={(int)pair.Key}> {(pair.Key != PvpSpecificItemCategory.Speed ? "+" : "-")}{Math.Round(pair.Value, 3)} ";
        }

        m_ItemNameTmp.text = $"{item.itemName}\n{finalStr.Trim()}";
    }

    private float GetBonusValue(PvpSpecificItemCategory category, InventoryPvpItemData item)
    {
        if (item == null)
            return 0;

        return PlayerData.Instance.GetItemValue(category, item);
    }
    #endregion

    #region button click
    public void OnClickLeaderBoard()
    {
        ShowUIView<LeaderboardPvp>().InitLeaderboard(0);
    }

    public void ChangeBG()
    {
        GameManager.Instance.NextBackground();
    }

    public void ChangePetDefender()
    {
        ShowUIView<SelectPetDefender>().InitSelectPvp();
    }

    public void ShowPvPShop()
    {
        ShowUIView<PvpShopManager>();
    }
    
    public void StartFindMatch()
    {
        //m_SelectPetAttackPopup.GetComponent<SelectPetPVP>().SetOnSelectPetId((id) => currentSelectedPetId = id).InitSelectPvp();
        ShowUIView<UISelectPetAttacker>().InitSelectPvp();
    }

    public void OnClickItem()
    {
        ShowUIView<PopupItemPvp>().SetOnEquipedItemCallback((id) => {
            m_StatLoader.LoadStats();
            InitEquipItem();
        }).SetOnUpgradeEquipedItemCallback(() => { 
            InitEquipItem();
            m_StatLoader.LoadStats();
        }).InitData(PlayerData.Instance.PetData.equipe);
        /*
        m_PopupItem.gameObject.SetActive(true);
        m_PopupItem.GetComponent<PopupItemPvp>().SetOnEquipedItemCallback((id) => {
            UpdateParamOfItem();
            InitEquipItem(); }).SetOnUpgradeEquipedItemCallback(() => { InitEquipItem(); UpdateParamOfItem(); }).InitData(PlayerData.Instance.PetData.equipe);
        */
    }
    #endregion

    #region find match
    public void OnFindMatch(int petId)
    {
        
        //StartCoroutine(CountDownRoutine(petId));
    }

    public void OnConfirmDefender()
    {
        WebSocketRequestHelper.SetDefensePet(currentSelectedPetId);
        Toast.Show("Set Defense Pet Successfully");
    }

    public void OnConfirmAttack()
    {
        OnFindMatch(currentSelectedPetId);
    }
    #endregion
    /*
     * private IEnumerator CountDownRoutine(int petId)
        {
            float maxTime = 8f;
            float elapsedTime = 0;
            bool matchFound = false;

            PvpCombat pvpCombat = new PvpCombat();

            WebSocketRequestHelper.GoCombatOnce(petId, (combat) =>
            {
                matchFound = true;
                pvpCombat = combat;
            });

            while (elapsedTime < maxTime) // Stop counting when elapsedTime reaches maxTime
            {
                // Update elapsed time
                elapsedTime += 1f;

                // Calculate minutes and seconds
                int minutes = Mathf.FloorToInt(elapsedTime / 60f);
                int seconds = Mathf.FloorToInt(elapsedTime % 60f);

                FindingTime.text = "Finding Opponent...\n" + string.Format("{0:00}:{1:00}", minutes, seconds);

                if (matchFound)
                    maxTime = 2f;

                yield return new WaitForSeconds(1f); // Wait for 1 second
            }

            if (matchFound)
            {
                // code logic combat here
                ShowUIView<UIBeforeMatchPanel>().SetBeforeMatch(pvpCombat);
                //SetBeforeMatch(pvpCombat);
            }
            else
            {
                Toast.Show("No opponents found!");
            }
            FindingMatch.SetActive(false);
        }

       
        */
}