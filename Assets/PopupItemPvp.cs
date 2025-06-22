using Core.Utils;
using Game;
using Game.Websocket;
using Game.Websocket.Model;
using PathologicalGames;
using TMPro;
using UnityEngine;

public class PopupItemPvp : BaseView
{
    [SerializeField] private Transform m_SelectedItemInfoPopup;
    [SerializeField] private ItemPvp m_SelectItem;
    [SerializeField] private TextMeshProUGUI m_NameItem;
    [SerializeField] private TextMeshProUGUI m_Desc;

    [SerializeField] private GameObject m_UpgradeBtn;
    [SerializeField] private GameObject m_UpgradeNotAvailableBtn;
    [SerializeField] private GameObject m_EquippedBtn;
    [SerializeField] private GameObject m_EqquippedNotAvailableBtn;

    [SerializeField] private Transform m_ListItemContent;
    [SerializeField] private Transform m_ItemInventoryPref;

    //private InventoryPvpItemData curItemInfo;       // The item which player is equipped.
    private InventoryPvpItemData selectedItem;      // The item which player is selecting.

    private ICallback.CallFunc2<string> onEquippedItemCallback;
    private ICallback.CallFunc onUpgradeEquippedItemCallback;
    public PopupItemPvp SetOnEquipedItemCallback(ICallback.CallFunc2<string> func) { onEquippedItemCallback = func; return this; }
    public PopupItemPvp SetOnUpgradeEquipedItemCallback(ICallback.CallFunc func) { onUpgradeEquippedItemCallback = func; return this; }

    protected override void OnViewShown()
    {
        
    }

    protected override void OnViewHidden()
    {
        
    }

    public void InitData(InventoryPvpItemData info)
    {
        if (info == null)
        {
            m_SelectedItemInfoPopup.gameObject.SetActive(false);
        }
        else
        {
            m_SelectedItemInfoPopup.gameObject.SetActive(true);
            m_SelectItem.InitData(info);
            m_NameItem.text = info.itemName;
            m_Desc.text = info.itemInfo;
        }    

        //curItemInfo = info;
        selectedItem = info;
        UpdateButtonsState();

        PoolManager.Pools["InventoryItems"].DespawnAll();
        foreach (InventoryPvpItemData item in PlayerData.Instance.LstUserItemsInventory)
        {
            Transform trans = PoolManager.Pools["InventoryItems"].Spawn(m_ItemInventoryPref, m_ListItemContent);
            trans.gameObject.SetActive(true);
            trans.GetComponent<ItemPvp>().SetOnPvPItemCallback((data) =>
            {
                OnChangeItem(data);
            }).InitData(item);
        }
    }

    public void InitEquipedItem(InventoryPvpItemData info) { }

    private void UpdateButtonsState()
    {
        if (selectedItem == null)
            return;
        m_UpgradeBtn.SetActive(selectedItem.current_level < 5 ? true : false);
        m_UpgradeNotAvailableBtn.SetActive(selectedItem.current_level >= 5 ? true : false);
        InventoryPvpItemData petItem = PlayerData.Instance.PetData.equipe;

        if (petItem != null)
        {
            bool equipCondition = petItem._id != selectedItem._id;
            m_EquippedBtn.SetActive(equipCondition ? true : false);
            m_EqquippedNotAvailableBtn.SetActive(!equipCondition ? true : false);
        }
        else
        {
            m_EquippedBtn.SetActive(true);
            m_EqquippedNotAvailableBtn.SetActive(false);
        }    
    }

    public void OnChangeItem(InventoryPvpItemData item)
    {
        this.selectedItem = item;
        m_SelectedItemInfoPopup.gameObject.SetActive(true);
        m_SelectItem.InitData(item);
        m_NameItem.text = item.itemName;
        m_Desc.text = item.itemInfo;
        UpdateButtonsState();
    }

    public void OnClickEquipped()
    {
        WebSocketRequestHelper.EquipPetItem(PlayerData.Instance.PetData.petId, selectedItem._id);
        PlayerData.Instance.PetData.equipe = selectedItem;
        PlayerData.Instance.PetData.item_equipe = selectedItem._id;
        UpdateButtonsState();
        onEquippedItemCallback?.Invoke(this.selectedItem._id);
    }

    public void OnClickUpgradeItem()
    {
        if (selectedItem == null)
            return;

        ShowUIView<PopupUgradeItemPvp>().SetOnUpgradeItemInventoryCallback(OnConfirmUpgrade).InitData(selectedItem);
    }

    private void OnConfirmUpgrade(string _id)
    {
        WebSocketRequestHelper.UpgradePvpItemOnce(_id, (id) =>
        {
            WebSocketRequestHelper.QueryPvpInventoryOnce((Data, response) =>
            {
                PlayerData.Instance.LstUserItemsInventory.Clear();
                PlayerData.Instance.LstUserItemsInventory.AddRange(Data);
                InventoryPvpItemData upgradeItem = new InventoryPvpItemData();

                foreach (InventoryPvpItemData item in PlayerData.Instance.LstUserItemsInventory)
                {
                    if (item._id.Equals(id._id))
                    {
                        upgradeItem = item;
                        LoggerUtil.Logging("OnConfirmUpgrade", $"Upgraded item\nId: {upgradeItem.id}\nName: {upgradeItem.itemName}");
                        break;
                    }
                }

                if (PlayerData.Instance.PetData.item_equipe != null)
                {
                    if (PlayerData.Instance.PetData.item_equipe.Equals(_id))
                    {
                        PlayerData.Instance.PetData.item_equipe = upgradeItem._id;
                        PlayerData.Instance.PetData.equipe = upgradeItem;
                    }
                    else
                    {

                    }
                }

                InitData(upgradeItem);
                onUpgradeEquippedItemCallback?.Invoke();
            });
        });
    }

    public void OnClickClose()
    {
        Hide();
    }
}
