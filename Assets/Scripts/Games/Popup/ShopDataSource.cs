using Core.Utils;
using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemInfo
{
    public int id;
    public CurrencyType type;
    public float price;
    public ICallback.CallFunc2<int> callback;
}

public class ShopDataSource : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField] private RecyclableScrollRect m_RecycleScrollRect;

    private List<ItemInfo> lstItems = new List<ItemInfo>();

    private void Awake()
    {
        m_RecycleScrollRect.DataSource = this;
    }

    public void AddItem(ItemData item, ICallback.CallFunc2<int> callback)
    {
        ItemInfo info = new ItemInfo
        {
            id = item.id,
            type = item.currencyType,
            price = item.price,
            callback = callback
        };

        lstItems.Add(info);
    }

    public int GetItemCount()
    {
        return lstItems.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        var item = cell as ShopItemHandler;
        item.InitItem(lstItems[index].id, lstItems[index].type, lstItems[index].price, lstItems[index].callback);
    }
}
