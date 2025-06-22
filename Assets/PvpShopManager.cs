using Core.Utils;
using Game;
using Game.Websocket;
using Game.Websocket.Model;
using PathologicalGames;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PvpShopManager : BaseView
{
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private UINumericField m_Diamond;
    [SerializeField] private UINumericField m_Tickets;
    [Header("Tab")]
    [SerializeField] private Transform m_PvpItemContent;
    [SerializeField] private Transform m_BackgroundContent;
    [SerializeField] private Transform m_PvpItemParent;

    [Header("Item")]
    [SerializeField] private Button m_RefreshBtn;

    [Header("BG")]
    [SerializeField] private Image m_CurBackgroundImg;
    [SerializeField] private TextMeshProUGUI m_BGNameTxt;
    [SerializeField] private TextMeshProUGUI m_InfoTxt;
    [SerializeField] private Button m_NextBtn;
    [SerializeField] private Button m_PreBtn;
    [SerializeField] private Button m_BuyBtn;
    [SerializeField] private Button m_OwnBtn;

    [SerializeField] private Transform m_PvpShopItem;


    public ICallback.CallFunc2<PvpShopTab> onTabSelected;

    private PvpShopQueryResponse itemsPvpQueryShop;
    private List<PVPShopItemData> m_ListBg = new List<PVPShopItemData>();
    private PVPShopItemData curBGData;

    private PVPController pvpController;

    protected override void OnSetup()
    {
        base.OnSetup();
        pvpController = GetUIView<PVPController>();
        PlayerData.Instance.OnCurrencyChange += SetCurrency;
    }

    protected override void OnViewShown()
    {
        m_CloseBtn.onClick.AddListener(OnClickClose);

        m_NextBtn.onClick.AddListener(OnNextBG);
        m_PreBtn.onClick.AddListener(OnPreBG);
        m_RefreshBtn.onClick.AddListener(OnClickRefresh);
        m_BuyBtn.onClick.AddListener(OnBuyBG);
        Show(PvpShopTab.Item);
    }

    protected override void OnViewHidden()
    {
        m_CloseBtn.onClick.RemoveListener(OnClickClose);

        m_NextBtn.onClick.RemoveListener(OnNextBG);
        m_PreBtn.onClick.RemoveListener(OnPreBG);
        m_RefreshBtn.onClick.RemoveListener(OnClickRefresh);
        m_BuyBtn.onClick.RemoveListener(OnBuyBG);
    }

    public void Show(PvpShopTab tab = PvpShopTab.Item)
    {
        WebSocketRequestHelper.QueryPvpShopOnce((listPvpShopItem) =>
        {
            // item + BG
            itemsPvpQueryShop = listPvpShopItem;
            //m_Shop.SetActive(true);
            OnButtonTabSelected(tab);
        });
    }

    private void SetCurrency(CurrencyType type, int value)
    {
        switch(type)
        {
            case CurrencyType.Diamond:
                m_Diamond.SetInterger(value);
                break;
            case CurrencyType.Ticket:
                m_Tickets.SetInterger(value);
                break;
        }
    }

    public void ReloadData(int id, PvpShopQueryResponse listPvpShopItem)
    {
        PVPShopItemData data = PlayerData.Instance.GetItemPvpData(id);
        itemsPvpQueryShop = listPvpShopItem;
        OnButtonTabSelected(data.tab);
    }

    public void OnButtonTabSelected(PvpShopTab tab)
    {
        onTabSelected?.Invoke(tab);
        m_PvpItemContent.gameObject.SetActive(false);
        m_BackgroundContent.gameObject.SetActive(false);

        switch (tab)
        {
            case PvpShopTab.Item:
                SpawnPvpItems();
                break;

            case PvpShopTab.Background:
                //SpawnBackgound();
                break;
        }
    }

    private void SpawnPvpItems()
    {
        PoolManager.Pools["PvpItem"].DespawnAll();
        Debug.Log(itemsPvpQueryShop.items.Length);
        foreach (PVPShopItemData item in itemsPvpQueryShop.items)
        {
            if (item.tab != PvpShopTab.Item)
                continue;
            Transform trans = PoolManager.Pools["PvpItem"].Spawn(m_PvpShopItem, m_PvpItemParent);
            trans.GetComponent<ShopPvpItemInfo>()
                .SetOnPvPItemCallback(OnClickBuyPvpItem).
                InitData(item.id);
        }
        m_PvpItemContent.gameObject.SetActive(true);
    }

    public void OnClickBuyPvpItem(int id)
    {
        // Request buy pvp item.
        ConfirmPurchaseOnPvp(GetShopItem(id), (success, response) =>
        {
            if (success)
            {
                pvpController.ReloadPvpInventory();
                // Query the shop items to get the latest/remain items.
                WebSocketRequestHelper.QueryPvpShopOnce((listPvpShopItem) =>
                {
                    itemsPvpQueryShop = listPvpShopItem;
                    SpawnPvpItems();
                });
            }
        });
    }

    private PVPShopItemData GetShopItem(int id)
    {
        foreach (var item in itemsPvpQueryShop.items)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return default;
    }

    // hiennt chỗ này refresh chưa hoạt động
    private void OnClickRefresh()
    {
        WebSocketRequestHelper.RefreshPvpShopOnce((listPvpShopItem) =>
        {
            // item + BG
            itemsPvpQueryShop = listPvpShopItem;
            OnButtonTabSelected(PvpShopTab.Item);
        });
    }

    private void SpawnBackgound()
    {
        m_BackgroundContent.gameObject.SetActive(true);

        if (PlayerData.Instance.itemsPvp.Count == 0)
        {
            return;
        }

        foreach (PVPShopItemData item in PlayerData.Instance.itemsPvp)
        {
            if (item.tab != PvpShopTab.Background)
                continue;
            m_ListBg.Add(item);
        }
        SetBackgroundItem(0);
    }

    private void OnNextBG()
    {
        int curIndex = m_ListBg.IndexOf(curBGData);
        Debug.Log(curIndex);
        if (curIndex >= m_ListBg.Count - 1)
            return;
        SetBackgroundItem(curIndex + 1);
    }

    private void OnPreBG()
    {
        int curIndex = m_ListBg.IndexOf(curBGData);
        Debug.Log(curIndex);
        if (curIndex <= 0)
            return;
        SetBackgroundItem(curIndex - 1);
    }
    private void SetBackgroundItem(int ind)
    {
        curBGData = m_ListBg[ind];
        m_BGNameTxt.text = curBGData.itemName;
        m_InfoTxt.text = curBGData.itemInfo;
        m_CurBackgroundImg.sprite = BackgroundDataSO.Instance.GetBackgroundSprite(curBGData.id)[0];
        // set trang thai cho btn buy bg
        m_BuyBtn.gameObject.SetActive(!PlayerData.Instance.data.ownedBackgroundIds.Contains(curBGData.id));
        m_OwnBtn.gameObject.SetActive(PlayerData.Instance.data.ownedBackgroundIds.Contains(curBGData.id));
    }
    private void OnBuyBG()
    {
        ConfirmPurchaseOnPvp(curBGData, (success, response) =>
        {
            if (success)
            {
                PlayerData.Instance.data.ownedBackgroundIds.Add(curBGData.id);
                //PlayerData.Instance.SaveData();
                int curIndex = m_ListBg.IndexOf(curBGData);
                SetBackgroundItem(curIndex);

            }
        });
    }

    public void OnClickClose()
    {
        PoolManager.Pools["PvpItem"].DespawnAll();
        Hide();
    }

    public void ConfirmPurchaseOnPvp(PVPShopItemData pvpItem, ICallback.CallFunc3<bool, PvpShopQueryResponse> onConfirmed)
    {
        PlayerData.Instance.DecreaseTicket(pvpItem.GetPrice(), (TicketChangeResponse response) => {
            if (response != null)
            {
                if (response.success)
                {
                    LoggerUtil.Logging("response.Success", TextColor.Green);
                    OnCofirmBuyItemPvp(pvpItem.id, onConfirmed);
                }
                else
                {
                    LoggerUtil.Logging("response.Failed", TextColor.Green);
                    ShowUIView<PopupNotify>().SetOnConfirmBuyItemPvp((success) =>
                    {
                        if (success)
                            OnCofirmBuyItemPvp(pvpItem.id, onConfirmed);
                        else
                            onConfirmed?.Invoke(false, new PvpShopQueryResponse());
                    }).Init(
                        "NOT ENOUGH TICKETS",
                        "You don't have enough tickets!\nBuy more<sprite=0> to continue",
                        pvpItem.GetPrice(), false,
                        false,
                        1
                        );
                    /*
                    Transform trans = PoolManager.Pools["Popup"].Spawn(m_NotifyPopup, m_ParentPopup);
                    trans.GetComponent<PopupNotify>().SetOnConfirmBuyItemPvp((success) =>
                    {
                        if (success)
                            OnCofirmBuyItemPvp(pvpItem.id, onConfirmed);
                        else
                            onConfirmed?.Invoke(false, new PvpShopQueryResponse());
                    }).Init(
                        "NOT ENOUGH TICKETS",
                        "You don't have enough tickets!\nBuy more<sprite=0> to continue",
                        pvpItem.GetPrice(), false,
                        false,
                        1
                        );
                    */
                }
            }
            else
            {
                Debug.LogError("Response of Decrease ticket request is null");
            }
        });
    }

    private void OnCofirmBuyItemPvp(int id, ICallback.CallFunc3<bool, PvpShopQueryResponse> onConfirmed)
    {
        WebSocketRequestHelper.BuyPvpShopItemOnce(id, (Complete) =>
        {
            // item + BG
            onConfirmed?.Invoke(true, Complete);
        });
    }
}

// hiennt
// refresh trong chưa hoạt động
// test kĩ lại phần mua xong nhé, logic khi mua xong 
// - Item: despawn thằng vừa mua, hiện tại anh đang code hơi dở, chỗ đấy chắc để despawn thằng vừa mua luôn, hoặc cho despawn hết rồi spawn lại
// - Background: mua xong thì chuyển sang chữ "Purchased"
// nút close popup shop pvp chưa hoạt động
// - khi close thì cho despawn hết item pvp nhé
// logic chỗ inventory:
// - cho load all inventory khi bật popup
// - check item nào đang đc equipe trong pet (check = _id) thì không spawn ở dưới mà spawn ở trên
// - PlayerData.Instance.PetData.equipe