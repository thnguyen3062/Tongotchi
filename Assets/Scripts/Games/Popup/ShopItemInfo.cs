using Game;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemInfo : ItemInfoPopup
{
    [SerializeField] private Button m_Buy1;
    [SerializeField] private Button m_Buy10;

    protected override void OnViewShown()
    {
        base.OnViewShown();
        m_Buy1.onClick.AddListener(OnBuy1);
        m_Buy10.onClick.AddListener(OnBuy10);
    }

    protected override void OnViewHidden()
    {
        base.OnViewHidden();
        m_Buy1.onClick.RemoveListener(OnBuy1);
        m_Buy10.onClick.RemoveListener(OnBuy10);
    }

    /*
    public void Show(int id, string itemName, ItemCategory type, int ownedCount, string itemInfo, float price, float value, ICallback.CallFunc onFailToUse = null)
    {
        this.id = id;
        this.ownedCount = ownedCount;
        m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id];
        m_ItemName.text = itemName;

        string itemType = "";
        if (type == ItemCategory.Food)
            itemType = "Food";
        else if (type == ItemCategory.Medicine)
            itemType = "Medicine";
        else if (type == ItemCategory.Toy)
            itemType = "Toy";
        m_Type.text = "Category: " + itemType;

        m_OwnedCount.text = "Owned: " + ownedCount.ToString();
        m_ItemInfo.text = string.Format(itemInfo, value);
        m_Price.text = "Price: " + price;

        if (m_Confirm != null)
        {
            bool canInteract = !((type == ItemCategory.Toy && PlayerData.Instance.PetData.status.happy >= 90f) ||
                (type == ItemCategory.Food && PlayerData.Instance.PetData.status.hunger >= 90f) ||
                (type == ItemCategory.Medicine && PlayerData.Instance.PetData.status.healthValue >= 50f));
            m_Confirm.interactable = canInteract;
        }

        onFailedToUseCallback = onFailToUse;
    }
    */
    private void OnBuy1()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        //PlayerData.Instance.OnAddItem(id, 1);
        //TelegramPayment.instance.BuyStar("prodcut_" + id);
        ShowUIView<PopupConfirmPurchase>().SetOnConfirmPurchaseItemCallback(ShowPurchaseResult).InitData(PurchaseType.Item, id, 1);
        Hide();
    }

    private void OnBuy10()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        //PlayerData.Instance.OnAddItem(id, 10);
        ShowUIView<PopupConfirmPurchase>().SetOnConfirmPurchaseItemCallback(ShowPurchaseResult).InitData(PurchaseType.Item, id, 10);
        //UIManager.instance.OnBuyItem(id, 10);
        Hide();
    }

    private void ShowPurchaseResult(int id, bool isSuccess, int count, int quantity)
    {
        ShowUIView<PopupPurchaseResult>().InitDataForItem(isSuccess, id, count);
    }
}
