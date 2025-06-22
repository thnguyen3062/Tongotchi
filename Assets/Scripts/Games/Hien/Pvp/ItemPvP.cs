using Core.Utils;
using Game.Websocket.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPvp : MonoBehaviour
{
    [SerializeField] private Image m_ItemFrame;
    [SerializeField] private Image m_ItemImg;
    [SerializeField] private GameObject[] m_Stars;
    private InventoryPvpItemData data;
    private ICallback.CallFunc2<InventoryPvpItemData> onClickItemInventoryCallback;
    public ItemPvp SetOnPvPItemCallback(ICallback.CallFunc2<InventoryPvpItemData> func) { onClickItemInventoryCallback = func; return this; }
    public void InitData(InventoryPvpItemData item)
    {
        data = item;
        m_ItemFrame.sprite = ItemPvpImgSO.Instance.GetSpriteItemTier(data.category);
        m_ItemImg.sprite = ItemPvpImgSO.Instance.GetSpritePvpItem(data.id);
        foreach (GameObject g in m_Stars)
            g.SetActive(false);
        for (int i = 0; i < data.current_level;i++)
            m_Stars[i].SetActive(true);
    }   
    public void OnChangeItemInventory()
    {
        onClickItemInventoryCallback?.Invoke(data);
    }    
}