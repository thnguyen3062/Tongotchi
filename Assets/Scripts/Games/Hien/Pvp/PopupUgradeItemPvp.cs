using Core.Utils;
using Game.Websocket.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Game;

public class PopupUgradeItemPvp : BaseView
{
    [SerializeField] private ItemPvp item;
    [SerializeField] private TextMeshProUGUI m_NameItem;
    [SerializeField] private TextMeshProUGUI m_CopyTxt;
    [SerializeField] private TextMeshProUGUI m_RequireTxt;
    [SerializeField] private TextMeshProUGUI m_BoostTxt;

    [SerializeField] private Button m_CancelBtn;
    [SerializeField] private Button m_UpgradeBtn;
    private InventoryPvpItemData curItemInfo;
    private ICallback.CallFunc2<string> onUpgradeItemInventoryCallback;
    public PopupUgradeItemPvp SetOnUpgradeItemInventoryCallback(ICallback.CallFunc2<string> func) { onUpgradeItemInventoryCallback = func; return this; }
    public void InitData(InventoryPvpItemData info)
    {
        // Dung: In here, we must remove the original item from the copy because when we own an item, the default quanity is already 1.
        int copyCount = --info.quantity;

        if (copyCount < 0) copyCount = 0;

        if (copyCount < info.GetRequiredCopy(info.current_level + 1))
            m_UpgradeBtn.interactable = false;
        else
            m_UpgradeBtn.interactable = true;
        curItemInfo = info;
        item.InitData(info);
        m_NameItem.text = info.itemName;
        
        m_CopyTxt.text = "Copy: " + copyCount.ToString() ;
        m_RequireTxt.text = "Required gears: " + info.GetRequiredCopy(curItemInfo.current_level +1);
        info.GetValues(curItemInfo.current_level + 1, out Dictionary<PvpSpecificItemCategory, float> stats);

        string boostFields = "";

        uint index = 0;
        foreach (var stat in stats)
        {
            boostFields += $"{stat.Key.ToString()}: {Math.Round(stat.Value, 3)}(+{Math.Round(info.GetUpgradeParam(index, curItemInfo.current_level), 3)})\n";
            index++;
        }
        boostFields.Trim();
        m_BoostTxt.text = $"{boostFields}";
    }

    protected override void OnViewShown()
    {
        m_UpgradeBtn.onClick.AddListener(OnClickUpdate);
        m_CancelBtn.onClick.AddListener(OnClickClose);
    }

    protected override void OnViewHidden()
    {
        m_UpgradeBtn.onClick.RemoveListener(OnClickUpdate);
        m_CancelBtn.onClick.RemoveListener(OnClickClose);
    }

    private void OnClickUpdate()
    {
        onUpgradeItemInventoryCallback?.Invoke(curItemInfo._id);
        OnClickClose();
    }

    private void OnClickClose()
    {
        Hide();
    }
}
