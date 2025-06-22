using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemDataSO : ScriptableObject
{
    //private static ItemDataSO instance;
    //public static ItemDataSO Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //            return instance = Resources.Load<ItemDataSO>("Data/ItemData");
    //        return instance;
    //    }
    //}

    //[SerializeField] private ItemData[] items;

    //public ItemData GetItemData(int id)
    //{
    //    foreach(var item in items)
    //    {
    //        if(item.itemId == id) return item;
    //    }
    //    return null;
    //}

    //public ItemData[] GetItems()
    //{
    //    return items;
    //}
}
