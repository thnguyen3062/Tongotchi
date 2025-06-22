using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemPvpImg", menuName = "Data/ItemPvpImg")]
public class ItemPvpImgSO : ScriptableObject
{
    private static ItemPvpImgSO instance;
    public static ItemPvpImgSO Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<ItemPvpImgSO>("Data/ItemPvpImg"); ;
            return instance;
        }
    }

    public ItemPvpImgData[] pvpItems;
    public ItemPvpTierImg[] itemTiers;

    public Sprite GetSpritePvpItem(int id)
    {
        Sprite s = null;
        foreach (ItemPvpImgData img in pvpItems)
            if (id == img.itemID)
                s = img.sprite;
        return s;
    }
    public Sprite GetSpriteItemTier(PvpItemCategory tier)
    {
        Sprite s = null;
        foreach (ItemPvpTierImg img in itemTiers)
        {
            if (tier == img.tier)
                s = img.sprite;
        }
        return s;
    }
}
[Serializable]
public class ItemPvpImgData
{
    public int itemID;
    public Sprite sprite;
}
[Serializable]
public class ItemPvpTierImg
{
    public PvpItemCategory tier;
    public Sprite sprite;
}