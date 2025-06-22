using Core.Utils;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemHandler : MonoBehaviour, ICell
{
    private int id;
    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_PriceText;

    private Button shopItemButton;
    private ICallback.CallFunc2<int> onShopItemCallback;
    public ShopItemHandler SetOnShopItemCallback(ICallback.CallFunc2<int> func) { onShopItemCallback = func; return this; }

    private void Start()
    {
        shopItemButton = GetComponent<Button>();
        shopItemButton.onClick.AddListener(OnShowShopItemDetail);
    }

    public void InitItem(int id, CurrencyType currencyType, float price, ICallback.CallFunc2<int> callback)
    {
        this.id = id;
        m_Icon.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id];
        m_PriceText.text = currencyType == CurrencyType.Diamond ? "<sprite=2>" + price.ToString() : "<sprite=0>" + price.ToString();
        onShopItemCallback = callback;
    }

    private void OnShowShopItemDetail()
    {
        onShopItemCallback?.Invoke(id);
    }
}
