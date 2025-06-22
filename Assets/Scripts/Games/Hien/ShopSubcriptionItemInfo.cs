using Core.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSubcriptionItemInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_DescriptionText;
    [SerializeField] private TextMeshProUGUI m_PriceText;
    [SerializeField] private Image m_ItemImage;
    [SerializeField] private Button m_Buy;

    private ICallback.CallFunc2<int> onSubcriptionItemCallback;
    public ShopSubcriptionItemInfo SetOnSubcriptionItemCallback(ICallback.CallFunc2<int> func) { onSubcriptionItemCallback = func; return this; }
    private ItemData data;
    private float timeRemain;
    // Start is called before the first frame update
    void Start()
    {
        if (m_Buy != null)
            m_Buy.onClick.AddListener(OnBuy);
    }
    public void InitData(int id)
    {
        data = PlayerData.Instance.GetItemData(id);
        m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id]; ; //GameUtils.GetItemSprite(id.ToString());
        m_NameText.text = data.itemName;
        m_DescriptionText.text = data.itemInfo;
        string currency = "";
        if (data.currencyType == CurrencyType.Ticket)
            currency = "<sprite=0>";
        else if (data.currencyType == CurrencyType.Diamond)
            currency = "<sprite=2>";
        else 
            currency = "<sprite=1>";
        m_PriceText.text = currency + data.price.ToString();

    }    
    private void OnBuy()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        onSubcriptionItemCallback?.Invoke(data.id);
    }
}
