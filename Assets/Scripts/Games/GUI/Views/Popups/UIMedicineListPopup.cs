using Game;
using Game.Websocket;
using PathologicalGames;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMedicineListPopup : BaseView
{
    private const int MAX_AVAILABLE_MEDICINE_UI = 3;

    [SerializeField] private Transform m_MedicineContainer;
    [SerializeField] private GameObject m_MedicineUI;
    [SerializeField] private TextMeshProUGUI m_MedicineDetail;
    [SerializeField] private Button m_CancelBtn;
    [SerializeField] private Button m_ConfirmBtn;

    private InventoryItem selectedMedicine;
    private bool goToShop;

    protected override void OnViewShown()
    {
        m_MedicineDetail.text = string.Empty;
        m_ConfirmBtn.onClick.AddListener(ConfirmUseMedicine);
        m_CancelBtn.onClick.AddListener(Close);
        LoadAvailableMedicines();
    }

    protected override void OnViewHidden()
    {
        m_ConfirmBtn.onClick.RemoveListener(ConfirmUseMedicine);
        m_CancelBtn.onClick.RemoveListener(Close);
    }

    private void LoadAvailableMedicines()
    {
        WebSocketRequestHelper.RequestLoadInventory((Dictionary<int, InventoryItem> ownedItemDict) => { 
            PlayerData.Instance.data.ownedItemDict = ownedItemDict;
            List<int> displayedMedicines = new List<int>(); 
            foreach (var item in ownedItemDict)
            {
                if (item.Value.data.category == ItemCategory.Medicine)
                {
                    // Spawn item here.
                    Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_MedicineUI, m_MedicineContainer);
                    trans.GetComponent<InventoryItemHandler>()
                        .SetOnInventoryItemClick(OnSelectMedicine)
                        .InitItem(item.Value.data.id, item.Value.quantity);
                    displayedMedicines.Add(item.Value.data.id);
                }
            }
            if (displayedMedicines.Count < 3)
            {
                foreach (var item in PlayerData.Instance.Items)
                {
                    if (item.category == ItemCategory.Medicine && !displayedMedicines.Contains(item.id))
                    {
                        // Spawn item here.
                        Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_MedicineUI, m_MedicineContainer);
                        trans.GetComponent<InventoryItemHandler>()
                            .SetOnInventoryItemClick(OnSelectMedicine)
                            .InitItem(item.id, 0);
                        displayedMedicines.Add(item.id);
                    }
                }
            }
            OnSelectMedicine(displayedMedicines[0], true);
        });
    }

    private void ConfirmUseMedicine()
    {
        if (goToShop)
        {
            ShowUIView<ShopManager>().ShowTab(ShopButtonTab.Items);
            Close();
        }
        else
        {
            GameManager.Instance.UseItemForPet(selectedMedicine.data.id, ItemCategory.Medicine, true, () => {
                Close();
            });
        }
    }

    private void Close()
    {
        PoolManager.Pools["InventoryItem"].DespawnAll();
        Hide();
    }

    private void OnSelectMedicine(int medicineId, bool isPlus)
    {
        ItemData item = PlayerData.Instance.GetItemData(medicineId);
        if (PlayerData.Instance.data.ownedItemDict.ContainsKey(medicineId))
        {
            selectedMedicine = PlayerData.Instance.data.ownedItemDict[medicineId];
            goToShop = false;
        }
        else
        {
            selectedMedicine = null;
            goToShop = true;
        }

        int ownedCount = 0;
        
        if (selectedMedicine != null)
        {
            ownedCount = selectedMedicine.quantity;
        }

        string msg = $"Owned: {ownedCount}\n{item.itemInfo}";
        m_ConfirmBtn.GetComponentInChildren<TextMeshProUGUI>().text = goToShop ? "Shop" : "Use";

        m_MedicineDetail.text = msg;
    }
}
