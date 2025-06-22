using Core.Utils;
using Game;
using PathologicalGames;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : BaseView
{
    public Dictionary<int, int> diamondValueDict = new Dictionary<int, int>
    {
        {1, 230 },
        {5, 1115 },
        {10, 2154 },
        {15, 3077 },
        {20, 3846 }
    };


    [Header("Tab Button")]
    [SerializeField] private ButtonSpriteChange m_ShopAllButton;
    [SerializeField] private ButtonSpriteChange m_ShopCurrencyButton;
    [SerializeField] private ButtonSpriteChange m_ShopTicketButton;

    [Header("Tab")]
    [SerializeField] private Transform m_FoodTabParent;
    [SerializeField] private Transform m_MedicineTabParent;
    [SerializeField] private Transform m_ToyTabParent;
    [SerializeField] private Transform m_SubcribeItemParent;

    [Header("Shop Item")]
    [SerializeField] private Transform m_ShopItem;
    //[SerializeField] private ShopItemInfo m_ShopItemInfo;
    [SerializeField] private Transform m_ShopItemContent;

    [Header("Subcription Item")]
    [SerializeField] private Transform m_SubcriptionItem;
    [SerializeField] private Transform m_SubcriptionItemContent;

    [Header("Currency Exchange")]
    [SerializeField] private Transform m_CurrencyContent;
    [SerializeField] private TextMeshProUGUI m_QuantityPackText;
    [SerializeField] private TextMeshProUGUI m_DiamondText;
    [SerializeField] private TextMeshProUGUI m_TicketText;
    [SerializeField] private Button m_AddPackTicketBtn;
    [SerializeField] private Button m_ReducePackTicketBtn;
    [SerializeField] private Button m_ExchangeTicket;
    [SerializeField] private Button m_BuyDiamond1;
    [SerializeField] private Button m_BuyDiamond5;
    [SerializeField] private Button m_BuyDiamond10;
    [SerializeField] private Button m_BuyDiamond15;
    [SerializeField] private Button m_BuyDiamond20;

    [Header("Scroll Dynamic")]
    [SerializeField] private ShopDataSource m_FoodScroll;
    [SerializeField] private ShopDataSource m_MedicineScroll;
    [SerializeField] private ShopDataSource m_ToyScroll;

    [SerializeField] private TextMeshProUGUI m_ticketLabel;
    [SerializeField] private TextMeshProUGUI m_diamondLabel;

    private int m_CountPackTicket = 1;
    public ICallback.CallFunc2<ShopButtonTab> onTabSelected;

    protected override void OnSetup()
    {
        base.OnSetup();
        m_CountPackTicket = 1;
        PlayerData.Instance.OnCurrencyChange += OnUpdateCurrency;
    }

    protected override void OnViewShown()
    {
        base.OnViewShown();
        HideUIView<UISetupFusionPopup>();

        m_QuantityPackText.text = m_CountPackTicket.ToString();
        m_AddPackTicketBtn.onClick.AddListener(OnClickAddPackTicket);
        m_ReducePackTicketBtn.onClick.AddListener(OnClickReducePackTicket);
        m_ExchangeTicket.onClick.AddListener(OnClickExchangeTicket);
        m_BuyDiamond1.onClick.AddListener(OnClickBuy1Diamond);
        m_BuyDiamond5.onClick.AddListener(OnClickBuy5Diamond);
        m_BuyDiamond10.onClick.AddListener(OnClickBuy10Diamond);
        m_BuyDiamond15.onClick.AddListener(OnClickBuy15Diamond);
        m_BuyDiamond20.onClick.AddListener(OnClickBuy20Diamond);

        m_ShopAllButton.SetOnButtonSelected(OnButtonTabSelected);
        m_ShopCurrencyButton.SetOnButtonSelected(OnButtonTabSelected);
        m_ShopTicketButton.SetOnButtonSelected(OnButtonTabSelected);

        SetScrollData();
    }

    protected override void OnViewHidden()
    {
        base.OnViewHidden();
        m_AddPackTicketBtn.onClick.RemoveListener(OnClickAddPackTicket);
        m_ReducePackTicketBtn.onClick.RemoveListener(OnClickReducePackTicket);
        m_ExchangeTicket.onClick.RemoveListener(OnClickExchangeTicket);
        m_BuyDiamond1.onClick.RemoveListener(OnClickBuy1Diamond);
        m_BuyDiamond5.onClick.RemoveListener(OnClickBuy5Diamond);
        m_BuyDiamond10.onClick.RemoveListener(OnClickBuy10Diamond);
        m_BuyDiamond15.onClick.RemoveListener(OnClickBuy15Diamond);
        m_BuyDiamond20.onClick.RemoveListener(OnClickBuy20Diamond);
    }

    void OnUpdateCurrency(CurrencyType currency, int value)
    {
        if (currency == CurrencyType.Ticket)
        {
            m_ticketLabel.text = GameUtils.FormatCurrency(value);
        }
        else if (currency == CurrencyType.Diamond)
        {
            m_diamondLabel.text = GameUtils.FormatCurrency(value);
        }
    }

    public void ShowTab(ShopButtonTab tab = ShopButtonTab.Items)
    {
        //m_Shop.SetActive(true);
        PlayerData.Instance.GetCurrency(CurrencyType.Ticket);
        PlayerData.Instance.GetCurrency(CurrencyType.Diamond);
        OnButtonTabSelected(tab);
    }

    public void OnButtonTabSelected(ShopButtonTab tab)
    {
        onTabSelected?.Invoke(tab);
        m_ShopItemContent.gameObject.SetActive(false);
        m_SubcriptionItemContent.gameObject.SetActive(false);
        m_CurrencyContent.gameObject.SetActive(false);

        switch (tab)
        {
            case ShopButtonTab.Items:
                ShowAllItem();
                break;
            case ShopButtonTab.Subscribe:
                SpawnSubcriptionItem(); // Boosts
                break;
            case ShopButtonTab.Currency:
                ShowCurrencyExchange();
                break;
        }

        m_ShopAllButton.OnChangeButtonSprite(tab);
        m_ShopCurrencyButton.OnChangeButtonSprite(tab);
        m_ShopTicketButton.OnChangeButtonSprite(tab);
    }

    private void ShowAllItem()
    {
        m_ShopItemContent.gameObject.SetActive(true);
    }

    private void SetScrollData()
    {
        foreach (ItemData item in PlayerData.Instance.Items)
        {
            if (item.tab != ItemTab.Item)
                continue;
            if (!item.active)
                continue;

            SpawnItem(item);
        }
    }

    private void SpawnSubcriptionItem()
    {
        m_SubcriptionItemContent.gameObject.SetActive(true);
        PoolManager.Pools["SubcriptionItem"].DespawnAll();
        foreach (ItemData item in PlayerData.Instance.Items)
        {
            if (item.tab != ItemTab.Subscription)
                continue;
            Transform trans = PoolManager.Pools["SubcriptionItem"].Spawn(m_SubcriptionItem, m_SubcribeItemParent);
            trans.GetComponent<ShopSubcriptionItemInfo>()
                .SetOnSubcriptionItemCallback(ClickBuySubcribeItem)
                .InitData(item.id);
        }
    }

    private void ClickBuySubcribeItem(int id)
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        ConfirmBuySubscibtionItem(id);
    }

    private void SpawnItem(ItemData item)
    {
        switch (item.category)
        {
            case ItemCategory.Food:
                //parent = m_FoodTabParent;
                m_FoodScroll.AddItem(item, ShowItemInfo);
                break;
            case ItemCategory.Medicine:
                //parent = m_MedicineTabParent;
                m_MedicineScroll.AddItem(item, ShowItemInfo);
                break;
            case ItemCategory.Toy:
                //parent = m_ToyTabParent;
                m_ToyScroll.AddItem(item, ShowItemInfo);
                break;
        }

        //Transform trans = PoolManager.Pools["ShopItem"].Spawn(m_ShopItem, parent);
        //trans.GetComponent<ShopItemHandler>()
        //    .SetOnShopItemCallback(OnShopItenmCallback)
        //    .InitItem(item.id, item.currencyType, item.price);
    }

    private void ShowItemInfo(int id)
    {
        ItemData item = PlayerData.Instance.GetItemData(id);
        int ownedCount = 0;
        if (PlayerData.Instance.data.ownedItemDict.ContainsKey(item.id))
        {
            InventoryItem inventItem = PlayerData.Instance.data.ownedItemDict[id];
            ownedCount = inventItem.quantity;
        }
        ShowUIView<ShopItemInfo>().SetInfo(id, item.itemName, item.category, ownedCount, item.itemInfo, item.price, item.value);
    }

    //currency
    private void ShowCurrencyExchange()
    {
        m_CurrencyContent.gameObject.SetActive(true);
        m_CountPackTicket = 1;
        m_QuantityPackText.text = m_CountPackTicket.ToString();
    }

    private void OnClickAddPackTicket()
    {
        m_CountPackTicket++;
        m_QuantityPackText.text = m_CountPackTicket.ToString();
        m_DiamondText.text = m_CountPackTicket.ToString();
        m_TicketText.text = (m_CountPackTicket * GameUtils.TICKET_EXCHANGE_RATE).ToString();
    }

    private void OnClickReducePackTicket()
    {
        if (m_CountPackTicket <= 1)
            return;
        m_CountPackTicket--;
        m_QuantityPackText.text = m_CountPackTicket.ToString();
        m_DiamondText.text = m_CountPackTicket.ToString();
        m_TicketText.text = (m_CountPackTicket * GameUtils.TICKET_EXCHANGE_RATE).ToString();
    }

    private void ConfirmBuySubscibtionItem(int id)
    {
        ShowUIView<PopupConfirmPurchase>().SetOnConfirmPurchaseItemCallback(ShowBuySubsribtionItemResult).InitData(PurchaseType.Item, id);
    }

    private void ShowBuySubsribtionItemResult(int id, bool isSuccess, int count, int quantity)
    {
        ShowUIView<PopupPurchaseResult>().InitDataForItem(isSuccess, id, count);
        if (isSuccess)
        {
            PlayerData.Instance.GetCurrency(CurrencyType.Diamond);
            PlayerData.Instance.GetCurrency(CurrencyType.Ticket);
        }
    }

    private void OnClickExchangeTicket()
    {
        OnClickExchangeTicket(m_CountPackTicket);
    }

    private void OnClickBuy1Diamond()
    {
        OnClickExchangeDiamond(1);
    }

    private void OnClickBuy5Diamond()
    {
        OnClickExchangeDiamond(5);
    }

    private void OnClickBuy10Diamond()
    {
        OnClickExchangeDiamond(10);
    }

    private void OnClickBuy15Diamond()
    {
        OnClickExchangeDiamond(15);
    }

    private void OnClickBuy20Diamond()
    {
        OnClickExchangeDiamond(20);
    }

    public void OnClickExchangeTicket(int quantity)
    {
        ShowUIView<PopupConfirmPurchase>().SetOnConfirmExchangeCallback(OnConfirmExchange).InitDataExchange(PurchaseType.Exchange, ExchangeType.Ticket, quantity, quantity * 920);
    }

    public void OnClickExchangeDiamond(int quantity)
    {
        ShowUIView<PopupConfirmPurchase>().SetOnConfirmExchangeCallback(OnConfirmBuyDiamond).InitDataExchange(PurchaseType.Exchange, ExchangeType.Diamon, diamondValueDict[quantity], quantity);
        FirebaseAnalytics.instance.LogCustomEvent("purchase_star_clicked");
    }

    private void OnConfirmExchange(bool isSuccess, CurrencyType currencyType, ExchangeInfo info, int changeAmount = 0)
    {
        ShowUIView<PopupPurchaseResult>().InitDataExchange(isSuccess, currencyType, info, changeAmount);
    }

    private void OnConfirmBuyDiamond(bool isSuccess, CurrencyType currencyType, ExchangeInfo info, int amount = 0)
    {
        OpenBuyStar((int)info.quantityGot, (int)info.quantityLost, (success) =>
        {
            ShowUIView<PopupPurchaseResult>().InitStarPurchase(success, info);
        });
    }

    private void OpenBuyStar(int count, int price, Action<bool> onCompleted)
    {
        TelegramPayment.instance.BuyStar(count, price, onCompleted);
    }
}

[Serializable]
public class ExchangeInfo
{
    public CurrencyType currencyExchange;
    public float quantityLost;
    public CurrencyType currencyReceive;
    public float quantityGot;
}
