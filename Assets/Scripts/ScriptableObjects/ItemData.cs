using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public ItemTab tab;
    public string itemName;
    public ItemCategory category;
    public SpecificItemCategory specificCategory;
    public CurrencyType currencyType;
    public float price;
    public float value;
    public string itemInfo;
    public bool active;
}