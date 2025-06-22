using Core.Utils;
using Game.Websocket.Model;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopPvpItemInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ItemNameText;
    [SerializeField] private TextMeshProUGUI m_TierTxt;
    //[SerializeField] private TextMeshProUGUI m_OwnTxt;
    [SerializeField] private TextMeshProUGUI m_ItemInfo;
    [SerializeField] private TextMeshProUGUI m_PriceText;
    [SerializeField] private TextMeshProUGUI m_BuffText;
    [SerializeField] private Image m_ItemFrame;
    [SerializeField] private Image m_ItemImg;
    [SerializeField] private Button m_Buy;
    [SerializeField] private UITextField[] tmpStatArray;

    private ICallback.CallFunc2<int> onPvPItemCallback;
    public ShopPvpItemInfo SetOnPvPItemCallback(ICallback.CallFunc2<int> func) { onPvPItemCallback = func; return this; }
    private PVPShopItemData data;
    // Start is called before the first frame update
    void Start()
    {
        if (m_Buy != null)
            m_Buy.onClick.AddListener(OnBuy);
    }
    public void InitData(int id)
    {
        HideAllStats();
        data = PlayerData.Instance.GetItemPvpData(id);
        m_ItemFrame.sprite = ItemPvpImgSO.Instance.GetSpriteItemTier(data.category);
        m_ItemImg.sprite = ItemPvpImgSO.Instance.GetSpritePvpItem(id);
        m_ItemNameText.text = data.itemName;
        string[] infoParts = data.itemInfo.Split(',');

        string finalInfo = "";

        foreach (var  part in infoParts)
        {
            finalInfo += part.Trim() + "\n";
        }

        m_ItemInfo.text = finalInfo;

        //  fix cung la item level 1  , tra bang gem

        //if (data.currencyType == CurrencyType.Ticket)
        //    currency = "<sprite=0>";
        //else if (data.currencyType == CurrencyType.Diamond)
        //    currency = "<sprite=2>";
        //else
        //    currency = "<sprite=1>";
        m_PriceText.text = $"<sprite=0> {data.GetPrice()}";// currency + price.ToString() ;

        data.GetStatsValue(out Dictionary<PvpSpecificItemCategory, float> statDictionary);

        int index = 0;
        foreach (var pair in statDictionary)
        {
            string str = $"<sprite={(int)pair.Key}> {(pair.Key != PvpSpecificItemCategory.Speed ? "+" : "-")}{Math.Round(pair.Value, 3)}";
            SetStat(index, str);
            index++;
        }
    }
    private void OnBuy()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        onPvPItemCallback?.Invoke(data.id);
    }

    private void HideAllStats()
    {
        foreach (var tmpStat in tmpStatArray)
        {
            tmpStat.Hide();
        }
    }

    private void SetStat(int index, string value)
    {
        if (index >= 0 && index < tmpStatArray.Length)
        {
            tmpStatArray[index].SetString(value);
            tmpStatArray[index].Show();
        }
        else
        {
            Debug.LogError($"Index {index} is out of range!");
        }
    }
}
