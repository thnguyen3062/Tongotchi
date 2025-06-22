using Game;
using Newtonsoft.Json;
using PathologicalGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPurchaseResult : BaseView
{
    [SerializeField] private TextMeshProUGUI m_ItemInfoText, m_ItemInfoFailText;
    [SerializeField] private TextMeshProUGUI m_OwnedText;
    [SerializeField] private Image m_ItemImageOnSuccess, m_ItemImageOnFailed;
    [SerializeField] private GameObject m_FailPopup, m_SuccessPopup;
    [SerializeField] private GameObject m_BuyMoreStar;
    // Start is called before the first frame update
    public void InitDataForItem(bool isSuccess, int id, int count = 1)
    {
        transform.localPosition = Vector3.zero;
        m_FailPopup.SetActive(!isSuccess);
        m_SuccessPopup.SetActive(isSuccess);
        ItemData data = PlayerData.Instance.GetItemData(id);
        m_ItemImageOnFailed.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id]; //GameUtils.GetItemSprite(id.ToString());
        m_ItemImageOnSuccess.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id];// GameUtils.GetItemSprite(id.ToString());

        int owned = 0;
        string itemName = "";
        if (PlayerData.Instance.data.ownedItemDict.ContainsKey(data.id))
        {
            InventoryItem item = PlayerData.Instance.data.ownedItemDict[id];
            owned = item.quantity;
            itemName = item.data.itemName;
        }
        m_OwnedText.text = owned.ToString();
        m_ItemInfoText.text = data.itemInfo;
        string currency = "";
        if (data.currencyType == CurrencyType.Ticket)
            currency = "<sprite=0>";
        else if (data.currencyType == CurrencyType.Diamond)
            currency = "<sprite=2>";
        else
            currency = "<sprite=1>";
        if (!string.IsNullOrEmpty(itemName))
            FirebaseAnalytics.instance.LogCustomEvent("user_purchase_item", JsonConvert.SerializeObject(new CustomEventWithVariable(itemName)));

        m_ItemInfoFailText.text = "You don't have enough " + currency + (data.price * count);
    }

    public void InitDataForPetSlot(bool isSuccess, CurrencyType currencyType, int price)
    {
        transform.localPosition = Vector3.zero;
        m_FailPopup.SetActive(!isSuccess);
        m_SuccessPopup.SetActive(isSuccess);
        m_ItemImageOnFailed.transform.parent.gameObject.SetActive(false);
        m_ItemImageOnSuccess.transform.parent.gameObject.SetActive(false);
        m_OwnedText.text = "1";
        m_ItemInfoText.text = "You got one more pet slot!";
        m_ItemInfoFailText.text = "You don't have enough " + GetSprite(currencyType) + price.ToString();

    }

    private int changeNumber;

    public void InitDataExchange(bool isSuccess, CurrencyType currencyType, ExchangeInfo info, int changeNumber = 0)
    {
        transform.localPosition = Vector3.zero;
        m_FailPopup.SetActive(!isSuccess);
        m_SuccessPopup.SetActive(isSuccess);
        m_BuyMoreStar.SetActive(currencyType == CurrencyType.Token);
        this.changeNumber = changeNumber;
        m_ItemImageOnFailed.transform.parent.gameObject.SetActive(false);
        m_ItemImageOnSuccess.transform.parent.gameObject.SetActive(false);
        m_ItemInfoText.text = "Successfully converted " + GetSprite(info.currencyExchange) + info.quantityLost + " to " + GetSprite(info.currencyReceive) + info.quantityGot + "!";
        m_ItemInfoFailText.text = "You don't have enough. You need a minimum of " + GetSprite(info.currencyExchange) + info.quantityLost + " to exchange!";
    }

    public void InitStarPurchase(bool isSuccess, ExchangeInfo info)
    {
        transform.localPosition = Vector3.zero;
        m_FailPopup.SetActive(!isSuccess);
        m_SuccessPopup.SetActive(isSuccess);
        m_ItemImageOnFailed.transform.parent.gameObject.SetActive(false);
        m_ItemImageOnSuccess.transform.parent.gameObject.SetActive(false);
        m_ItemInfoText.text = "Successfully purchase " + GetSprite(info.currencyReceive) + info.quantityGot + " by " + GetSprite(info.currencyExchange) + info.quantityLost + "!";
        m_ItemInfoFailText.text = "You don't have enough. You need a minimum of " + GetSprite(info.currencyExchange) + info.quantityLost + " to exchange!";
    }

    private string GetSprite(CurrencyType type)
    {
        string currency = "";
        if (type == CurrencyType.Ticket)
            currency = "<sprite=0>";
        else if (type == CurrencyType.Diamond)
            currency = "<sprite=2>";
        else
            currency = "<sprite=1>";
        return currency;
    }

    public void OnTab()
    {
        SoundManager.Instance.PlayVFX("2. Screen Touch");
        Hide();
    }
}
