using Game.Websocket;
using PathologicalGames;
using System.Collections.Generic;
namespace Game
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UINormalInventory : BaseView
    {
        [Header("UI.INVENTORY")]
        [SerializeField] private UIActionButton[] tabs;
        [SerializeField] private Color m_SelectedColor = Color.white;
        [SerializeField] private Color m_DeselectColor = Color.gray;
        //[SerializeField] private Image[] m_InventoryTabs;
        [SerializeField] private GameObject m_ItemView;
        [SerializeField] private GameObject m_SpecialView;
        [SerializeField] private Sprite m_ActiveTabSprite;
        [SerializeField] private Sprite m_InactiveTabSprite;
        [SerializeField] private bool showAddItemBtn = false;
        [SerializeField] private bool showLoadingView = true;
        [Header("Normal Item References")]
        [SerializeField] private Transform m_InventoryItem;
        [SerializeField] private Transform m_InventoryItemParent;
        [Header("Special Item References")]
        [SerializeField] private Transform m_InventorySpecialItem;
        [SerializeField] private Transform m_InventorySpecialItemParent;
        [SerializeField] private Button m_NextBtn;
        [SerializeField] private Button m_PrevBtn;

        private ItemCategory currentCategory;
        private int currentTabIndex;

        protected override void OnViewShown()
        {
            base.OnViewShown();
            HideUIView<UISetupFusionPopup>();
            m_NextBtn.onClick.AddListener(OnInventoryNext);
            m_PrevBtn.onClick.AddListener(OnInventoryPrevious);
            foreach (var tab in tabs)
            {
                tab.OnClick += ShowItems;
            }
            ShowItems(ActionType.Feed);
        }

        protected override void OnViewHidden()
        {
            base.OnViewHidden();
            m_NextBtn.onClick.RemoveListener(OnInventoryNext);
            m_PrevBtn.onClick.RemoveListener(OnInventoryPrevious);

            foreach (var tab in tabs)
            {
                tab.OnClick -= ShowItems;
            }
            PoolManager.Pools["InventoryItem"].DespawnAll();
        }

        #region Inventory
        public void OnInventoryNext()
        {
            int currentTab = (int)currentTabIndex;
            currentTab++;

            if (currentTab > 3)
                currentTab = 0;

            currentTabIndex = currentTab;

            ShowItems(tabs[currentTab].Action);
        }

        // Transfer to UINormalInventoryView.cs
        public void OnInventoryPrevious()
        {
            int currentTab = (int)currentTabIndex;
            currentTab--;
            if (currentTab < 0)
                currentTab = 3;
            currentTabIndex = currentTab;
            ShowItems(tabs[currentTabIndex].Action);
        }

        private void ShowItems(ActionType action)
        {
            LoadItems(action);
        }

        protected void LoadItems(ActionType type)
        {
            if (tabs.Length > 0)
            {
                foreach (var tab in tabs)
                    tab.SetColor(m_DeselectColor);
                tabs[GetTabIndex(type)].SetColor(m_SelectedColor);
            }

            PoolManager.Pools["InventoryItem"].DespawnAll();
            if (showLoadingView) ShowUIView<UILoadingView>();

            WebSocketRequestHelper.RequestLoadInventory((Dictionary<int, InventoryItem> ownedItemDict) =>
            {
                if (showLoadingView) HideUIView<UILoadingView>();

                PlayerData.Instance.data.ownedItemDict = ownedItemDict;
                if (type != ActionType.Special)
                {
                    SetItemViewState(true);
                    SetSpecialItemViewState(false);
                    foreach (var kvp in ownedItemDict)
                    {
                        InventoryItem item = kvp.Value; // Get the InventoryItem from the KeyValuePair
                        if (item.quantity <= 0) continue;
                        bool shouldSpawn = false;

                        shouldSpawn = item.data.active;
                        switch (type)
                        {
                            case ActionType.Feed:
                                shouldSpawn = item.data.category == ItemCategory.Food;
                                break;
                            case ActionType.Cure:
                                shouldSpawn = item.data.category == ItemCategory.Medicine;
                                break;
                            case ActionType.Toy:
                                shouldSpawn = item.data.category == ItemCategory.Toy;
                                break;
                            case ActionType.Special:
                                shouldSpawn = false;
                                break;
                        }
                        if (shouldSpawn)
                        {
                            Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_InventoryItem, m_InventoryItemParent);
                            trans.GetComponent<InventoryItemHandler>()
                                .SetOnInventoryItemClick(OnInventoryItemClick)
                                .InitItem(item.data.id, item.quantity);

                            currentCategory = item.data.category;
                        }
                    }

                    if (showAddItemBtn)
                    {
                        Transform transPlus = PoolManager.Pools["InventoryItem"].Spawn(m_InventoryItem, m_InventoryItemParent);
                        transPlus.GetComponent<InventoryItemHandler>()
                            .SetOnInventoryItemClick(OnInventoryItemClick)
                            .InitItem(0, 0, true);
                        transPlus.SetAsLastSibling();
                    }
                }
                else
                {
                    SetItemViewState(false);
                    SetSpecialItemViewState(true);
                    currentCategory = ItemCategory.Special;
                    bool shouldSpawn = false;
                    foreach (var kvp in ownedItemDict)
                    {
                        InventoryItem item = kvp.Value;
                        shouldSpawn = item.data.active && item.data.category == ItemCategory.Special;

                        if (!shouldSpawn)
                            continue;

                        Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_InventorySpecialItem, m_InventorySpecialItemParent);
                        trans.GetComponent<InventoryItemHandler>()
                            .SetOnInventoryItemClick(OnInventoryItemClick)
                            .InitSpecialItem(item.data.id, item.quantity, -1);
                    }


                    if (PlayerData.Instance.data.boost.Count > 0)
                    {
                        for (int i = 0; i < PlayerData.Instance.data.boost.Count; i++)
                        {
                            Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_InventorySpecialItem, m_InventorySpecialItemParent);
                            trans.GetComponent<InventoryItemHandler>()
                                .SetOnInventoryItemClick(OnInventoryItemClick)
                                .InitSpecialItem(PlayerData.Instance.data.boost[i].boostId, -1, PlayerData.Instance.data.boost[i].remainingTime);
                        }
                    }

                    if (PlayerData.Instance.PetData.boost.Count > 0)
                    {
                        for (int i = 0; i < PlayerData.Instance.PetData.boost.Count; i++)
                        {
                            Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_InventorySpecialItem, m_InventorySpecialItemParent);
                            trans.GetComponent<InventoryItemHandler>()
                                .SetOnInventoryItemClick(OnInventoryItemClick)
                                .InitSpecialItem(PlayerData.Instance.PetData.boost[i].boostId, -1, PlayerData.Instance.PetData.boost[i].remainingTime);
                        }
                    }
                }
            });
        }

        private void OnInventoryItemClick(int id, bool isPlus)
        {
            if (isPlus)
            {
                ShowUIView<ShopManager>().ShowTab(ShopButtonTab.Items);
                OnBuyMoreItem();
                return;
            }

            if (id == GameUtils.ROBOT_BOOST || id == GameUtils.TICKET_POTION_BOOST || id == GameUtils.EVOLVE_POTION)
                return;

            ItemData item = PlayerData.Instance.GetItemData(id);
            InventoryItem inventItem = PlayerData.Instance.data.ownedItemDict[id];
            int ownedCount = inventItem.quantity;


            ShowUIView<InventoryItemInfo>().SetUseItemSuccessCallback(RefreshInventory).SetInfo(id, item.itemName, item.category, ownedCount, item.itemInfo, item.price, item.value, () =>
            {
                HideUIView<InventoryItemInfo>();
                ShowUIView<ShopManager>();
            });
        }

        protected virtual void RefreshInventory()
        {
            LoadItems(ActionType.Feed);
        }

        protected void SetItemViewState(bool enable)
        {
            if (m_ItemView) m_ItemView.SetActive(enable);
        }

        private void SetSpecialItemViewState(bool enable)
        {
            if (m_SpecialView) m_SpecialView.SetActive(enable);
        }

        protected virtual void OnBuyMoreItem() { }
        #endregion

        private int GetTabIndex(ActionType action)
        {
            if (action == ActionType.Feed) return 0;
            else if (action == ActionType.Toy) return 1;
            else if (action == ActionType.Cure) return 2;
            else return 3;
        }
    }
}