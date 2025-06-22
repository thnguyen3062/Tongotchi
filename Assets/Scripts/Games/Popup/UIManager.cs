using Core.Utils;
using DG.Tweening;
using Game;
using Game.Websocket;
using Game.Websocket.Model;
using Newtonsoft.Json;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region fields
    [Header("Buttons")]
    //[SerializeField] private InteractButtonHandler m_CareBtn;
    [SerializeField] private InteractButtonHandler m_FeedBtn;
    [SerializeField] private InteractButtonHandler m_ShowerBtn;
    [SerializeField] private InteractButtonHandler m_CureBtn;
    [SerializeField] private InteractButtonHandler m_ToyBtn;
    [SerializeField] private InteractButtonHandler m_SleepBtn;
    [SerializeField] private InteractButtonHandler m_CleanBtn;
    //[SerializeField] private Button m_PlayMinigame;
    [SerializeField] private Button m_ShopButton;
    [SerializeField] private Button m_FriendButton;
    [SerializeField] private Button m_DailyButton;
    [SerializeField] private Button m_EvolveButton;
    [SerializeField] private Button m_SoundBtn;
    [SerializeField] private Button m_RankingBtn;
    [SerializeField] private Button m_InfoGotchiPointsBtn;
    [SerializeField] private Button m_InfoMinigameBtn;
    [SerializeField] private Button m_TaskButton;
    [SerializeField] private Button m_InventoryFoodBtn;
    [SerializeField] private Button m_InventoryMedicineBtn;
    [SerializeField] private Button m_InventoryToyBtn;
    [SerializeField] private Button m_InventorySpecialBtn;
    [SerializeField] private Button m_DropdownBoostBtn;

    //[SerializeField] private TMP_Dropdown m_MenuDropdown;
    [SerializeField] private Button m_ChangeBackgroundBtn;
    [SerializeField] private TMP_InputField m_NameInputField;

    [Header("Fields")]

    [SerializeField] private Transform m_UnderTheTopUI;
    [SerializeField] private Transform m_NewbieMissionUI;
    [SerializeField] private Transform m_BottomField;
    [SerializeField] private Transform m_StatusField;
    [SerializeField] private Transform m_AboveBottomField;
    [SerializeField] private GameObject m_ListItemsField;
    [SerializeField] private Transform m_InventoryItemParent;
    [SerializeField] private Transform m_InventorySpecialItemParent;
    [SerializeField] private Transform m_InventoryItem;
    [SerializeField] private Transform m_InventorySpecialItem;
    [SerializeField] private ShopItemInfo m_InventoryItemInfo;
    [SerializeField] private GameObject m_NotMinigameUI;
    [SerializeField] private DailyRewardManager m_DailyRewardManager;
    [SerializeField] private GameObject m_HealthUI;
    [SerializeField] private GameObject m_TimerUI;
    [SerializeField] private GameObject m_PvpMainUI;

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI m_TokenText;
    [SerializeField] private TextMeshProUGUI m_DiamondText;
    [SerializeField] private TextMeshProUGUI m_TicketText;

    [Header("Status")]
    [SerializeField] private Image m_HappinessIcon;
    [SerializeField] private Sprite[] m_HappninessSprite;
    [SerializeField] private Image m_HappynessBar;
    [SerializeField] private Image m_HyegineBar;
    [SerializeField] private Image m_HungerBar;
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Image m_TimerBar;
    [SerializeField] private Image m_ExpBar;
    [SerializeField] private TextMeshProUGUI m_TimerText;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private TextMeshProUGUI m_MinigameTicketText;
    [SerializeField] private TextMeshProUGUI m_TotalExpPointsText;

    [Header("Popup")]
    [SerializeField] private Transform m_ParentPopup;
    [SerializeField] private Transform m_UnderParentPopup;
    [SerializeField] private Transform m_PopupLevelUpReward;
    [SerializeField] private Transform m_PopupLevelUpRewardFailed;
    [SerializeField] private Transform m_PopupEvovle;
    [SerializeField] private Transform m_PopupEvovleWarning;
    [SerializeField] private Transform m_PopupConfirmPurchase;
    [SerializeField] private Transform m_PopupPurchaseResult;
    [SerializeField] private Transform m_NotifyPopup;
    [SerializeField] private Transform m_PopupGotchiPointsInfo;
    [SerializeField] private Transform m_MinigameInfo;
    //[SerializeField] private Transform m_UnderTheTop;
    [SerializeField] private FriendHandler m_FriendHandler;
    public TutorialHandler TutorialHandler;
    [SerializeField] private GameObject m_Inventory;
    [SerializeField] private Transform m_TaskPopup;

    [Header("Item")]
    [SerializeField] private Transform m_CleanerRobot;
    [SerializeField] private Transform m_CleanAnim;
    [SerializeField] private Image m_TicketBoostProgress;
    [SerializeField] private Image m_XPBoost;
    [SerializeField] private Image m_RobotBoostProgress;
    [SerializeField] private GameObject m_TicketBoost;
    [SerializeField] private GameObject m_RobotBoost;

    [Header("Other")]
    [SerializeField] private Sprite[] m_SoundImg;
    [SerializeField] private GameObject m_BoostIcon;
    #endregion

    private ICallback.CallFunc4<ActionType, ItemData, Action> onSelectAction;
    private ActionType currentType = ActionType.None;

    public UIManager SetOnSelectAction(ICallback.CallFunc4<ActionType, ItemData, Action> func) { onSelectAction = func; return this; }
    public ICallback.CallFunc3<ActionType, bool> onButtonSelected;

    #region unity methods
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        /*
        m_MinigameBtn.onClick.AddListener(OnPlayMinigame);
        m_SocialBtn.onClick.AddListener(() => OnOpenBottomPopup(BottomButtonType.Social));
        m_ClaimBtn.onClick.AddListener(() => OnOpenBottomPopup(BottomButtonType.Reward));
        m_HomeBtn.onClick.AddListener(() => OnOpenBottomPopup(BottomButtonType.Home));
        m_LeaderboardBtn.onClick.AddListener(OnOpenLeaderboard);
        m_PvpBtn.onClick.AddListener(() => OnOpenBottomPopup(BottomButtonType.PVP));
        m_InfoMinigameBtn.onClick.AddListener(OnShowMinigameInfo);

        //m_PlayMinigame.onClick.AddListener(OnPlayMinigame);
        m_ShopButton.onClick.AddListener(OnClickShop);
        m_FriendButton.onClick.AddListener(OnFriendClick);
        m_DailyButton.onClick.AddListener(OnDailyClick);
        m_EvolveButton.onClick.AddListener(OnClickEvolve);
        //m_CareBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_FeedBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_ShowerBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_CureBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_ToyBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_SleepBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_CleanBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        m_SoundBtn.onClick.AddListener(OnClickSound);
        m_RankingBtn.onClick.AddListener(OnRankingClick);
        m_InfoGotchiPointsBtn.onClick.AddListener(OnClickInfoGotchiPoints);
        m_ChangeBackgroundBtn.onClick.AddListener(OnChangeBackground);
        m_TaskButton.onClick.AddListener(OnClickTask);
        m_DropdownBoostBtn.onClick.AddListener(OnClickDropDownBoost);
        
        m_InventoryFoodBtn.onClick.AddListener(() =>
        {
            currentType = ActionType.Feed;
            m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
            currentCategory = ItemCategory.Food;
            SpawnInventoryItem(currentType);
        });
        m_InventoryMedicineBtn.onClick.AddListener(() =>
        {
            currentType = ActionType.Cure;
            m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
            currentCategory = ItemCategory.Medicine;
            SpawnInventoryItem(currentType);
        });
        m_InventoryToyBtn.onClick.AddListener(() =>
        {
            currentType = ActionType.Toy;
            m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
            currentCategory = ItemCategory.Toy;
            SpawnInventoryItem(currentType);
        });
        m_InventorySpecialBtn.onClick.AddListener(() =>
        {
            currentType = ActionType.Special;
            m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
            currentCategory = ItemCategory.Special;
            SpawnInventoryItem(currentType);
        });
        
        if (PlayerData.Instance.data.selectedPetID != -1)
            m_NameInputField.text = PlayerData.Instance.PetData.PetName;

        OnOpenBottomPopup(BottomButtonType.Home);

        //OnSetDropdownCallback();
        // You have <b>5</b> out of <b>5</b> tickets to play the minigame!
        m_MinigameTicketText.text = $"You have <b>{PlayerData.Instance.data.turnPlayMinigameRemain}</b> out of <b>{PlayerData.Instance.data.turnPlayerMinigameMax}</b> tickets to play the minigame!";
        */
    }

    private void OnDestroy()
    {
        PlayerData.Instance.OnCurrencyChange -= OnUpdateCurrency;                   // Done
        PlayerData.Instance.onUpdateExp -= OnUpdateExp;                             // Done
        PlayerData.Instance.onLevelUp -= OnPetLevelup;                             
        PlayerData.Instance.onUpdateBoost -= OnUpdateBoost;                     
        GameManager.Instance.PetController.onUpdateStatus -= OnUpdateStatusUI;      // Done
        //GameManager.instance.BoostHandler.onUpdateProgress -= OnUpdateBoostProgress;
    }
    #endregion

    #region Start Game
    public void InitUI()
    {
        /*
        //PlayerData.Instance.onUpdateCurrency += OnUpdateCurrency;
        PlayerData.Instance.OnCurrencyChange += OnUpdateCurrency;
        PlayerData.Instance.onUpdateExp += OnUpdateExp;
        PlayerData.Instance.onLevelUp += OnPetLevelup;
        PlayerData.Instance.onUpdateBoost += OnUpdateBoost;
        GameManager.instance.PetManager.StatusManager.onUpdateStatus += OnUpdateStatusUI;
        GameManager.instance.BoostHandler.onUpdateProgress += OnUpdateBoostProgress;
         

        if (PlayerData.Instance.data.selectedPetID != -1)
        {
            if (PlayerData.Instance.PetData.petPhase == PetPhase.Opening)
                PlayerData.Instance.PetData.petPhase = PetPhase.Opened;
            m_HealthUI.SetActive(PlayerData.Instance.PetData.petPhase == PetPhase.Opened);
            m_TimerUI.SetActive(PlayerData.Instance.PetData.petPhase != PetPhase.Opened);
            //m_UnderTheTop.gameObject.SetActive(PlayerData.Instance.PetData.petPhase == PetPhase.Opened);
            m_LevelText.text = "LEVEL " + (PlayerData.Instance.PetData.petLevel);
            FadeScreen.DOFade(0, 0.5f).OnComplete(() =>
            {
                FadeScreen.gameObject.SetActive(false);
            });
            if (PlayerData.Instance.PetData.boost.Any(item => item.id == GameUtils.ROBOT_BOOST))
                OnUpdateBoost(true, GameUtils.ROBOT_BOOST);
            if (PlayerData.Instance.data.boost.Any(item => item.id == GameUtils.TICKET_POTION_BOOST))
                OnUpdateBoost(true, GameUtils.TICKET_POTION_BOOST);
        }
        m_SoundBtn.image.sprite = m_SoundImg[PlayerData.Instance.data.isSoundOn ? 1 : 0];

        OnLoadTutorial();
        */
        InitViews();
    }

    #endregion

    [HideInInspector] public Transform newBieTransform;

    public void OnDoingNewBieMission()
    {
        if (PlayerData.Instance.data.socialQuestCompleted || PlayerData.Instance.data.isTutorialDone || PlayerData.Instance.PetData.petPhase == PetPhase.Opened)
            return;

        if (PlayerData.Instance.data.isComeFromBlum)
            return;

        newBieTransform = PoolManager.Pools["Popup"].Spawn(m_NewbieMissionUI, m_ParentPopup);
        newBieTransform.GetComponent<NewbieMission>().InitNewBie();
        newBieTransform.SetAsFirstSibling();
    }

    public void OnHatching()
    {
        m_BottomField.gameObject.SetActive(false);
        m_StatusField.gameObject.SetActive(false);
        m_HealthUI.SetActive(false);
        m_TimerUI.SetActive(true);
        //m_UnderTheTop.gameObject.SetActive(false);
        m_AboveBottomField.gameObject.SetActive(false);
    }

    public void OnOpened()
    {
        m_BottomField.gameObject.SetActive(true);
        m_StatusField.gameObject.SetActive(true);
        m_HealthUI.SetActive(true);
        //m_UnderTheTop.gameObject.SetActive(true);
        m_TimerUI.SetActive(false);
        m_AboveBottomField.gameObject.SetActive(true);
        m_NameInputField.text = PlayerData.Instance.PetData.PetName;

        if (!PlayerData.Instance.data.isTutorialDone)
        {
            if (!TutorialHandler.gameObject.activeSelf)
                TutorialHandler.gameObject.SetActive(true);
            TutorialHandler.SetTutorial(TutorialPhase.HatchingCompleted);
        }
    }

    public void UpdateTimerText(string text, float fillAmout)
    {
        m_TimerBar.fillAmount = fillAmout;
        m_TimerText.text = text;
    }

    #region button clicks
    private void OnShowMinigameInfo()
    {
        m_MinigameInfo.gameObject.SetActive(true);
    }

    //public void OnDailyClick()
    //{
    //    m_DailyRewardManager.InitRewardData();
    //}

    // action when click button needs
    private void OnActionButtonClicked(ActionType type)
    {
        if (type == ActionType.Shower)
        {
            if (PlayerData.Instance.PetData.status.hygiene >= 90f)
            {
               LoggerUtil.Logging($"Pet {PlayerData.Instance.PetData.petId} hygiene is above 90!", TextColor.Green);
                return;
            }

            onSelectAction?.Invoke(type, null, null);
            m_ListItemsField.SetActive(false);
            onButtonSelected?.Invoke(type, true);
            currentType = ActionType.None;
        }
        else if (type == ActionType.Clean)
        {
            onSelectAction?.Invoke(type, null, null);
            m_ListItemsField.SetActive(false);
            onButtonSelected?.Invoke(type, true);
            currentType = ActionType.None;
        }
        else
        {
            currentType = type;
            //SpawnInventoryItem(type);
        }
    }


    private void OnClickEvolve()
    {
        //Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupEvovle, m_ParentPopup);
        //trans.GetComponent<PopupEvolve>()
        //    .SetOnConfirmEvolveCallback(OnConfirmEvolve).OnInitDataEvolve();
    }

    private void OnRankingClick()
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_NotifyPopup, m_ParentPopup);
        trans.GetComponent<PopupNotify>().Init(
            "NOTICE",
            "Something might hatch here soon",
            0, false,
            false
            );
    }

    private void OnClickInfoGotchiPoints()
    {
        m_PopupGotchiPointsInfo.gameObject.SetActive(true);
    }

    private void OnClickDropDownBoost()
    {
        m_BoostIcon.SetActive(!m_BoostIcon.activeSelf);
    }
    #endregion

    #region inventory
    [SerializeField] private Image[] m_InventoryTabs;
    [SerializeField] private GameObject m_ItemView;
    [SerializeField] private GameObject m_SpecialView;
    [SerializeField] private Sprite m_ActiveTabSprite;
    [SerializeField] private Sprite m_InactiveTabSprite;
    /*
     * 
    private ItemCategory currentCategory;
    
    // Transfer to UINormalInventoryView.cs
    private void SpawnInventoryItem(ActionType type)
    {
        PoolManager.Pools["InventoryItem"].DespawnAll();

        WebSocketRequestHelper.RequestLoadInventory((Dictionary<int, InventoryItem> ownedItemDict) => {
            if (type != ActionType.Special)
            {
                m_ItemView.SetActive(true);
                m_SpecialView.SetActive(false);
                foreach (var kvp in ownedItemDict)
                {
                    InventoryItem item = kvp.Value; // Get the InventoryItem from the KeyValuePair
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
                            .SetOnInventoruItemClick(OnInventoryItemClick)
                            .InitItem(item.data.id, item.quantity);

                        currentCategory = item.data.category;
                    }
                }
                Transform transPlus = PoolManager.Pools["InventoryItem"].Spawn(m_InventoryItem, m_InventoryItemParent);
                transPlus.GetComponent<InventoryItemHandler>()
                    .SetOnInventoruItemClick(OnInventoryItemClick)
                    .InitItem(0, 0, true);
                transPlus.SetAsLastSibling();
            }
            else
            {
                m_SpecialView.SetActive(true);
                m_ItemView.SetActive(false);
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
                        .SetOnInventoruItemClick(OnInventoryItemClick)
                        .InitSpecialItem(item.data.id, item.quantity, -1);
                }


                if (PlayerData.Instance.data.boost.Count > 0)
                {
                    for (int i = 0; i < PlayerData.Instance.data.boost.Count; i++)
                    {
                        Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_InventorySpecialItem, m_InventorySpecialItemParent);
                        trans.GetComponent<InventoryItemHandler>()
                            .SetOnInventoruItemClick(OnInventoryItemClick)
                            .InitSpecialItem(PlayerData.Instance.data.boost[i].id, -1, PlayerData.Instance.data.boost[i].remainTimeBoost);
                    }
                }

                if (PlayerData.Instance.PetData.boost.Count > 0)
                {
                    for (int i = 0; i < PlayerData.Instance.PetData.boost.Count; i++)
                    {
                        Transform trans = PoolManager.Pools["InventoryItem"].Spawn(m_InventorySpecialItem, m_InventorySpecialItemParent);
                        trans.GetComponent<InventoryItemHandler>()
                            .SetOnInventoruItemClick(OnInventoryItemClick)
                            .InitSpecialItem(PlayerData.Instance.PetData.boost[i].id, -1, PlayerData.Instance.PetData.boost[i].remainTimeBoost);
                    }
                }
            }
        }); 

        

        m_InventoryTabs[(int)currentCategory].sprite = m_ActiveTabSprite;
        m_Inventory.SetActive(true);
    }
    // Transfer to UINormalInventoryView.cs
    public void OnInventoryNext()
    {
        m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
        int currentTab = (int)currentCategory;
        currentTab++;

        if (currentTab > 3)
            currentTab = 0;
        currentCategory = (ItemCategory)currentTab;

        SpawnInventoryItem(GetActionType(currentTab));
    }
    // Transfer to UINormalInventoryView.cs
    public void OnInventoryPrevious()
    {
        m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
        int currentTab = (int)currentCategory;
        currentTab--;
        if (currentTab < 0)
            currentTab = 3;
        currentCategory = (ItemCategory)currentTab;
        SpawnInventoryItem(GetActionType(currentTab));
    }

    private ActionType GetActionType(int currentTab)
    {
        if (currentTab == (int)ItemCategory.Food)
            return ActionType.Feed;
        else if (currentTab == (int)ItemCategory.Toy)
            return ActionType.Toy;
        else if (currentTab == (int)ItemCategory.Medicine)
            return ActionType.Cure;
        else
            return ActionType.Special;
    }
    // Transfer to UINormalInventoryView.cs
    private void OnInventoryItemClick(int id, bool isPlus)
    {
        if (isPlus)
        {
            //ShopManager.instance.Show(ShopButtonTab.Items);
            return;
        }

        if (id == GameUtils.ROBOT_BOOST || id == GameUtils.TICKET_POTION_BOOST || id == GameUtils.EVOLVE_POTION)
            return;

        ItemData item = PlayerData.Instance.GetItemData(id);
        InventoryItem inventItem = PlayerData.Instance.data.ownedItemDict[id];
        int ownedCount = inventItem.quantity;

        /*
        m_InventoryItemInfo.gameObject.SetActive(true);
        m_InventoryItemInfo.Show(id, item.itemName, item.category, ownedCount, item.itemInfo, item.price, item.value, () =>
        {
            m_InventoryItemInfo.gameObject.SetActive(false);
            OnClickShop();
        });
        
    }
        

    */
    // Transfer to UINormalInventoryView.cs
    public void OnUseItem(int id)
    {
        ItemData item = PlayerData.Instance.GetItemData(id);
        ActionType type = ActionType.None;
        if (item.category == ItemCategory.Food)
            type = ActionType.Feed;
        else if (item.category == ItemCategory.Medicine)
            type = ActionType.Cure;
        else if (item.category == ItemCategory.Toy)
            type = ActionType.Toy;

        onSelectAction?.Invoke(type, item, () => {
            m_ListItemsField.SetActive(false);
            onButtonSelected?.Invoke(type, true);
            currentType = ActionType.None;
            m_InventoryItemInfo.gameObject.SetActive(false);
            FirebaseAnalytics.instance.LogCustomEvent("user_use_item", JsonConvert.SerializeObject(new CustomEventWithVariable(item.itemName)));

            if (m_Inventory.activeSelf)
            {
                //SpawnInventoryItem(GetActionType((int)currentCategory));
            }
        });
    }
    #endregion
    private void OnUpdateBoost(bool isAdd, int id)
    {
        if (isAdd)
        {
            if (id == GameUtils.ROBOT_BOOST)
            {
                m_CleanerRobot.gameObject.SetActive(true);

                m_RobotBoost.SetActive(true);

                // auto clean after purchase robot
                OnCleanPoops();
            }

            if (id == GameUtils.TICKET_POTION_BOOST)
            {
                if (!m_TicketBoost.activeSelf)
                    m_TicketBoost.SetActive(true);
            }
        }
        else
        {
            if (id == GameUtils.ROBOT_BOOST)
            {
                m_CleanerRobot.gameObject.SetActive(false);
                m_RobotBoost.SetActive(false);
            }
            if (id == GameUtils.TICKET_POTION_BOOST)
            {
                m_TicketBoost.SetActive(false);
            }
        }
    }

    private void OnUpdateBoostProgress(int id, float progress)
    {
        switch (id)
        {
            case GameUtils.ROBOT_BOOST:
                m_RobotBoostProgress.fillAmount = progress;
                break;
            case GameUtils.TICKET_POTION_BOOST:
                m_TicketBoostProgress.fillAmount = progress;
                break;
        }
    }

    public void OnCleanPoops()
    {
        m_CleanAnim.gameObject.SetActive(true);
    }

    #region minigame
    private void OnPlayMinigame()
    {
        m_MinigameTicketText.text = $"You have <b>{PlayerData.Instance.data.turnPlayMinigameRemain}</b> out of <b>{PlayerData.Instance.data.turnPlayerMinigameMax}</b> tickets to play the minigame!";
        m_NotMinigameUI.SetActive(false);
        m_BottomField.gameObject.SetActive(false);
        m_StatusField.gameObject.SetActive(false);
        onSelectAction?.Invoke(ActionType.PlayMinigame, null, null);
    }

    public bool CheckTicket(Transform parentPopup)
    {
        if (PlayerData.Instance.data.turnPlayMinigameRemain > 0)
            return true;
        /*
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_NotifyPopup, parentPopup);
        trans.GetComponent<PopupNotify>().SetOnConfirmBuyMinigame((success) =>
        {
            if (success)
            {
                m_MinigameTicketText.text = $"You have <b>{PlayerData.Instance.data.turnPlayMinigameRemain}</b> out of <b>{PlayerData.Instance.data.turnPlayerMinigameMax}</b> tickets to play the minigame!";
                PoolManager.Pools["Popup"].Despawn(trans);
            }
        }).Init(
            "OUT OF MINIGAME TICKETS",
            "You've out of minigame tickets!\nYour tickets will re-fill tomorrow!\nOr buy 5 tickets with only 1<sprite=2>",
            1, false
            );
        */
        return false;
    }

    public void SpawnNotifyPopup(string title, string content)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_NotifyPopup, m_ParentPopup);
        trans.GetComponent<PopupNotify>().Init(title, content, 0, false, false);
    }

    public void SpawnBuyAttackCountNotify(bool canBuyMore)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_NotifyPopup, m_ParentPopup);
        trans.GetComponent<PopupNotify>().Init("OUT OF ATTACK!",
            "You've out of attack count today!\n" + (canBuyMore ? "Do you want to use 1<sprite=2> to reset?" : ""),
            1, false,
            canBuyMore,
            2);
    }

    public void OnMinigameOver()
    {
        m_BottomField.gameObject.SetActive(true);
        m_StatusField.gameObject.SetActive(true);
        m_NotMinigameUI.SetActive(true);
        m_MinigameTicketText.text = $"You have <b>{PlayerData.Instance.data.turnPlayMinigameRemain}</b> out of <b>{PlayerData.Instance.data.turnPlayerMinigameMax}</b> tickets to play the minigame!";
    }
    #endregion

    #region status update
    private void LockAllInteract()
    {
        if (GameManager.Instance.PetController.LockActionsWhenSleep)
        {
            m_CleanBtn.Interactable(false);
            m_FeedBtn.Interactable(false);
            m_ShowerBtn.Interactable(false);
        }
    }

    private void UnlockAllInteract()
    {
        m_CleanBtn.Interactable(true);
        m_FeedBtn.Interactable(true);
        m_ShowerBtn.Interactable(true);
    }

    private void OnUpdateStatusUI(StatusType type, float value)
    {
        float currentValue;
        switch (type)
        {
            case StatusType.Happyness:
                currentValue = (float)value / GameUtils.MAX_HAPPYNESS_VALUE;
                m_HappynessBar.fillAmount = currentValue;

                if (currentValue >= 0.7f)
                {
                    m_HappynessBar.color = GameUtils.HexToColor("#42bd41");
                    m_HappinessIcon.sprite = m_HappninessSprite[0];
                }
                else if (currentValue >= 0.3f)
                {
                    m_HappynessBar.color = GameUtils.HexToColor("#ffc107");
                    m_HappinessIcon.sprite = m_HappninessSprite[1];
                }
                else
                {
                    m_HappynessBar.color = GameUtils.HexToColor("#e51c23");
                    m_HappinessIcon.sprite = m_HappninessSprite[2];
                }
                break;

            case StatusType.Hunger:
                currentValue = (float)value / GameUtils.MAX_HUNGER_VALUE;
                m_HungerBar.fillAmount = currentValue;

                if (currentValue >= 0.7f)
                    m_HungerBar.color = GameUtils.HexToColor("#42bd41");
                else if (currentValue >= 0.3f)
                    m_HungerBar.color = GameUtils.HexToColor("#ffc107");
                else
                    m_HungerBar.color = GameUtils.HexToColor("#e51c23");
                break;

            case StatusType.Hygiene:
                currentValue = (float)value / GameUtils.MAX_HYGIENEV_VALUE;
                m_HyegineBar.fillAmount = currentValue;

                if (currentValue >= 0.7f)
                    m_HyegineBar.color = GameUtils.HexToColor("#42bd41");
                else if (currentValue >= 0.3f)
                    m_HyegineBar.color = GameUtils.HexToColor("#ffc107");
                else
                    m_HyegineBar.color = GameUtils.HexToColor("#e51c23");
                break;

            case StatusType.Health:
                currentValue = (float)value / GameUtils.MAX_HEALTH_VALUE;
                m_HealthBar.fillAmount = currentValue;
                break;

        }
    }

    private void OnUpdateExp(float value, float maxValue)
    {
        m_ExpBar.fillAmount = value / maxValue;
    }

    private void OnPetLevelup(bool isEvolve, bool canUpdateUI)
    {
        if (isEvolve && canUpdateUI)
        {
            m_LevelText.text = "Evolve!!";
            m_EvolveButton.interactable = true;
        }
        else
        {
            m_LevelText.text = "LEVEL " + (PlayerData.Instance.PetData.petLevel);
            m_EvolveButton.interactable = false;
            OnLevelupReward();
        }
    }

    public void SetExpBarTextOnStart(bool isEvolve)
    {
        //m_LevelText.text = isEvolve ? "Evolve!!" : "Level " + (PlayerData.Instance.PetData.petLevel);
        //m_EvolveButton.interactable = isEvolve;
    }

    public void SetTotalExpPointsText()
    {
        m_TotalExpPointsText.text = PlayerData.Instance.data.gotchipoint.ToString();

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_TotalExpPointsText.transform.parent.GetComponent<RectTransform>());
    }
    #endregion

    #region currency

    private void OnUpdateCurrency(CurrencyType type, int value)
    {
        switch (type)
        {
            //case CurrencyType.Token:
            //    m_TokenText.text = GameUtils.FormatCurrency(value);
            //    break;
            case CurrencyType.Diamond:
                m_DiamondText.text = GameUtils.FormatCurrency(value);
                break;
            case CurrencyType.Ticket:
                m_TicketText.text = GameUtils.FormatCurrency(value);
                break;
        }
    }
    #endregion

    #region evolve
    private void OnConfirmEvolve(bool canEvolve, bool isSuccess)
    {
        if (!canEvolve)
        {
            Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupEvovleWarning, m_ParentPopup);
            trans.GetComponent<PopupEvolveFailed>().OnInitDataEvolveFailed();
        }
        else
        {
            if (!isSuccess)
                OnEvolveFail();
        }
    }

    private void OnLevelupReward()
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupLevelUpReward, m_ParentPopup);
        trans.GetComponent<PopupLevelupReward>().InitDataLevelUpReward();
    }

    private void OnEvolveFail()
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupLevelUpRewardFailed, m_ParentPopup);
        trans.GetComponent<PopupLevelUpFailed>().SetTryAgainCallback(OnClickEvolve).InitDataEvolveFailed();
    }
    #endregion

    #region purchase
    #region Archive
    //public void OnBuyItem(int id, int count)
    //{
    //    Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupConfirmPurchase, Vector3.zero, Quaternion.identity, m_ParentPopup);
    //    trans.GetComponent<PopupConfirmPurchase>()
    //        .SetOnConfirmPurchaseItemCallback(OnConfirmPurchaseItem).InitData(PurchaseType.Item, id, count);
    //}

    //public void OnBuySubcription(int id)
    //{
    //    Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupConfirmPurchase, Vector3.zero, Quaternion.identity, m_ParentPopup);
    //    trans.GetComponent<PopupConfirmPurchase>()
    //        .SetOnConfirmPurchaseItemCallback(OnConfirmPurchaseItem).InitData(PurchaseType.Item, id);
    //}

    private void OnConfirmPurchaseItem(int id, bool isSuccess, int count)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupPurchaseResult, Vector3.zero, Quaternion.identity, m_ParentPopup);
        trans.GetComponent<PopupPurchaseResult>().InitDataForItem(isSuccess, id, count);
    }

    public void OnBuySlotSelectPet()
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupConfirmPurchase, Vector3.zero, Quaternion.identity, m_ParentPopup);
        trans.GetComponent<PopupConfirmPurchase>()
            .SetOnConfirmPurchaseSlotCallback(OnConfirmPurchasePetSlot).InitDataPurchaseSlotPet(PurchaseType.Slot, CurrencyType.Diamond, 4);
    }

    private void OnConfirmPurchasePetSlot(bool isSuccess)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupPurchaseResult, Vector3.zero, Quaternion.identity, m_ParentPopup);
        trans.GetComponent<PopupPurchaseResult>().InitDataForPetSlot(isSuccess, CurrencyType.Diamond, 4);
    }
    public void OnClickExchangeTicket(int quantity)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupConfirmPurchase, Vector3.zero, Quaternion.identity, m_ParentPopup);
        trans.GetComponent<PopupConfirmPurchase>()
            .SetOnConfirmExchangeCallback(OnConfirmExchange).InitDataExchange(PurchaseType.Exchange, ExchangeType.Ticket, quantity, quantity * 920);
    }

    public void OnClickExchangeDiamond(int quantity)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupConfirmPurchase, Vector3.zero, Quaternion.identity, m_ParentPopup);
        trans.GetComponent<PopupConfirmPurchase>()
            .SetOnConfirmExchangeCallback(OnConfirmBuyDiamond).InitDataExchange(PurchaseType.Exchange, ExchangeType.Diamon, diamondValueDict[quantity], quantity);
        FirebaseAnalytics.instance.LogCustomEvent("purchase_star_clicked");
    }

    private void OnConfirmExchange(bool isSuccess, CurrencyType currencyType, ExchangeInfo info, int changeAmount = 0)
    {
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupPurchaseResult, m_ParentPopup);
        trans.GetComponent<PopupPurchaseResult>().InitDataExchange(isSuccess, currencyType, info, changeAmount);
    }

    private void OnConfirmBuyDiamond(bool isSuccess, CurrencyType currencyType, ExchangeInfo info, int amount = 0)
    {
        OpenBuyStar((int)info.quantityGot, (int)info.quantityLost, (success) =>
        {
            Transform trans = PoolManager.Pools["Popup"].Spawn(m_PopupPurchaseResult, m_ParentPopup);
            trans.GetComponent<PopupPurchaseResult>().InitStarPurchase(success, info);
        });
    }

    public void OpenBuyStar(int count, int price, Action<bool> onCompleted)
    {
        TelegramPayment.instance.BuyStar(count, price, onCompleted);
    }

    public Dictionary<int, int> diamondValueDict = new Dictionary<int, int>
    {
        {1, 230 },
        {5, 1115 },
        {10, 2154 },
        {15, 3077 },
        {20, 3846 }
    };
    #endregion

    #endregion

    #region new ui
    [Header("New UI")]
    [SerializeField] private Transform m_WhiteBoardTransform;
    [SerializeField] private Transform m_RewardTransform;
    [SerializeField] private Transform m_SocialTransform;
    [SerializeField] private Transform m_MinigameTransform;
    [SerializeField] private Transform m_LeaderboardTransform;
    [SerializeField] private Transform m_LeaderboardItem;
    [SerializeField] private Transform m_LeaderboardContainer;
    [SerializeField] private LeaderboardItem yourRank;

    [SerializeField] private Transform m_BoardStart;
    [SerializeField] private Transform m_BoardEnd;
    [SerializeField] private Transform m_RewardBoardEnd;

    [SerializeField] private Transform m_ChooseFactionPopup;
    [SerializeField] private Transform m_PvpHome;
    [SerializeField] private Transform m_FindMatchPvpPopup;
    [SerializeField] private Transform m_BeforeMatchPopup;
    [Header("New UI Button")]
    [SerializeField] private Button m_HomeBtn;
    [SerializeField] private Button m_ClaimBtn;
    [SerializeField] private Button m_SocialBtn;
    [SerializeField] private Button m_MinigameBtn;
    [SerializeField] private Button m_LeaderboardBtn;
    [SerializeField] private Button m_PvpBtn;
    [SerializeField] private bool requireLv15 = false;

    [SerializeField] private Sprite[] m_BottomButtonSprite;
    // -586
    // -1000

    private bool isWhiteboardOpened;
    private BottomButtonType currentBottomButtonType;
    private List<Transform> leaderboards = new List<Transform>();

    public void OnOpenBottomPopup(BottomButtonType buttonType)
    {
        foreach (Transform t in m_UnderParentPopup)
        {
            t.gameObject.SetActive(false);
        }
        //m_InventoryTabs[(int)currentCategory].sprite = m_InactiveTabSprite;
        currentType = ActionType.None;

        if (currentBottomButtonType == buttonType)
        {
            if (currentBottomButtonType == BottomButtonType.Home)
            {
                return;
            }
            else
            {
                OnOpenBottomPopup(BottomButtonType.Home);
                return;
            }
        }

        m_HomeBtn.GetComponent<Image>().sprite = m_BottomButtonSprite[buttonType == BottomButtonType.Home ? 1 : 0];
        // m_ClaimBtn.GetComponent<Image>().sprite = m_BottomButtonSprite[buttonType == BottomButtonType.Reward ? 1 : 0];
        m_SocialBtn.GetComponent<Image>().sprite = m_BottomButtonSprite[buttonType == BottomButtonType.Social ? 1 : 0];
        m_MinigameBtn.GetComponent<Image>().sprite = m_BottomButtonSprite[buttonType == BottomButtonType.Minigame ? 1 : 0];
        m_PvpBtn.GetComponent<Image>().sprite = m_BottomButtonSprite[buttonType == BottomButtonType.PVP ? 1 : 0];

        m_RewardTransform.gameObject.SetActive(false);
        m_SocialTransform.gameObject.SetActive(false);
        m_MinigameTransform.gameObject.SetActive(false);

        switch (buttonType)
        {
            case BottomButtonType.Home:
                OnClosePvP();
                OnCloseWhiteBoard();
                break;
            case BottomButtonType.Reward:
                OnClosePvP();
                OnOpenBoard(() =>
                {
                    m_RewardTransform.gameObject.SetActive(true);
                }, true);
                break;
            case BottomButtonType.Social:
                OnClosePvP();
                OnOpenBoard(() =>
                {
                    m_SocialTransform.gameObject.SetActive(true);
                    SetTotalExpPointsText();
                });
                break;
            case BottomButtonType.Minigame:
                OnClosePvP();
                OnOpenBoard(() =>
                {
                    m_MinigameTransform.gameObject.SetActive(true);
                });
                break;
            case BottomButtonType.PVP:
                OnCloseWhiteBoard();
                if (CheckPvPAvailable())
                {
                    StartCoroutine(DelayOpenPvp());
                }
                else
                {
                    Toast.Show("      Must have at least 1 pet level 15 min to play PvP     ");
                }
                break;
        }

        currentBottomButtonType = buttonType;
    }

    private IEnumerator DelayOpenPvp()
    {
        //ShowFadeScreen();

        yield return new WaitForSeconds(0.5f);
        
        OnOpenPvP();

        yield return new WaitForSeconds(0.5f);

        //HideFadeScreen();
    }

    private void OnOpenBoard(ICallback.CallFunc onComplete, bool isRewardBoard = false)
    {
        m_WhiteBoardTransform.DOKill();
        m_WhiteBoardTransform.localPosition = new Vector3(m_WhiteBoardTransform.localPosition.x, m_BoardStart.GetComponent<RectTransform>().localPosition.y, m_WhiteBoardTransform.localPosition.z);

        isWhiteboardOpened = true;
        float destinationY = isRewardBoard ? m_RewardBoardEnd.GetComponent<RectTransform>().localPosition.y : m_BoardEnd.GetComponent<RectTransform>().localPosition.y;
        m_WhiteBoardTransform.DOLocalMoveY(destinationY, 0.5f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private void OnCloseWhiteBoard()
    {
        if (!isWhiteboardOpened)
            return;

        m_WhiteBoardTransform.DOKill();
        m_WhiteBoardTransform.DOLocalMoveY(m_BoardStart.GetComponent<RectTransform>().localPosition.y, 0.5f).OnComplete(() =>
        {
            isWhiteboardOpened = false;
        });
    }

    private void ShowLeaderboard(LeaderboardResponse leaderboard)
    {
        if (leaderboard == null || leaderboard.leaderboard == null || leaderboard.leaderboard.Count == 0)
        {
            LoadingScreen.SetActive(false);
            return;
        }

        int count = 0;
        leaderboards.Clear();
        foreach (var item in leaderboard.leaderboard)
        {
            Transform trans = PoolManager.Pools["Leaderboard"].Spawn(m_LeaderboardItem, m_LeaderboardContainer);
            string username = item.position + ". " + item.displayName;
            if (leaderboard.current != null)
            {
                if (item.telegramCode == leaderboard.current.telegramCode)
                {
                    username = item.position + ". You";
                }
            }
            trans.GetComponent<LeaderboardItem>().InitItem(item.position, username, item.score);
            leaderboards.Add(trans);
            count++;


            if (count >= leaderboard.leaderboard.Count)
            {
                if (leaderboard.current != null)
                {
                    yourRank.gameObject.SetActive(true);
                    yourRank.InitItem(leaderboard.current.position, leaderboard.current.position + ". You", leaderboard.current.score);
                }
                MiniGameLoading.SetActive(false);
            }
        }
    }

    public void OnOpenPvP()
    {
        m_PvpMainUI.SetActive(true);

        WebSocketRequestHelper.GetPvpProfileOnce((profile) =>
        {
            if (!string.IsNullOrEmpty(profile.error_code))
            {
                if (profile.error_code.Equals("912"))
                {
                    m_PvpHome.gameObject.SetActive(false);
                    m_ChooseFactionPopup.gameObject.SetActive(true);
                }
            }
            else
            {
                m_PvpHome.gameObject.SetActive(true);
                m_PvpMainUI.GetComponent<PVPController>().InitDataMainPvp();
                m_ChooseFactionPopup.gameObject.SetActive(false);
                PlayerData.Instance.SetPvpProfile(profile);
            }
        });

        m_UnderTheTopUI.gameObject.SetActive(false);
        m_StatusField.gameObject.SetActive(false);
        m_AboveBottomField.gameObject.SetActive(false);
        m_DropdownBoostBtn.gameObject.SetActive(false);
    }

    private void OnClosePvP()
    {
        if (!m_PvpMainUI.activeSelf)
            return;

        m_PvpMainUI.SetActive(false);
        m_UnderTheTopUI.gameObject.SetActive(true);
        m_StatusField.gameObject.SetActive(true);
        m_AboveBottomField.gameObject.SetActive(true);
        m_DropdownBoostBtn.gameObject.SetActive(true);
        m_DropdownBoostBtn.gameObject.SetActive(true);
    }

    private bool CheckPvPAvailable()
    {
        if (!requireLv15) return true;

        List<SavedPetData> ownedPetList = PlayerData.Instance.data.listOwnedPet;
        int petCount = PlayerData.Instance.userInfo?.pets?.Count ?? 0;

        if (ownedPetList == null || ownedPetList.Count == 0)
        {
            if (PlayerData.Instance.data.selectedPetID == -1)
            {
                return false;
            }
            else if (PlayerData.Instance.PetData != null && PlayerData.Instance.PetData.petLevel >= 15)
            {
                return true;
            }
        }

        bool hasAvailablePet = ownedPetList.Any(pet => pet.petLevel >= 15);

        return hasAvailablePet;
    }

    public void OnCloseLeaderboard()
    {
        foreach (var item in leaderboards)
        {
            if (item != null)
            {
                PoolManager.Pools["Leaderboard"].Despawn(item);
            }
        }
        m_LeaderboardTransform.gameObject.SetActive(false);
    }
    #endregion

    #region UI Manager
    [Header("Specific UI")]
    [SerializeField] private Transform m_StoryPopup;
    [SerializeField] private Transform m_StartGameUI;
    public Image FadeScreen;
    public GameObject LoadingScreen;
    public GameObject MiniGameLoading;
    [Header("UI List")]
    [SerializeField] private Transform m_PanelContainer;
    [SerializeField] private Transform m_OverlayPanelContainer;
    [SerializeField] private Transform m_PopupContainer;
    [SerializeField] private Transform m_OverlayPopupContainer;
    [Tooltip("View list which will display at the beginning")]
    [SerializeField] private List<BaseView> prefabViews = new List<BaseView>();
    [SerializeField] private List<BaseView> starterViews = new List<BaseView>();
    [SerializeField] private List<BaseView> viewList = new List<BaseView>();

    private Dictionary<Type, BaseView> activeViewsCache = new Dictionary<Type, BaseView>();

    public void OnStartGame()
    {
        PoolManager.Pools["Popup"].Spawn(m_StartGameUI, m_OverlayPopupContainer);
        Transform trans = PoolManager.Pools["Popup"].Spawn(m_StoryPopup, m_OverlayPopupContainer);
        trans.GetComponent<StoryHandler>().SetOnCompleted(() =>
        {
            FadeScreen.DOFade(0, 0.5f).OnComplete(() => FadeScreen.gameObject.SetActive(false));
        }).InitStory();
    }

    private void InitViews()
    {
        if (viewList.Count > 0)
        {
            viewList.ForEach(view => RegisterView(view));
            if (starterViews.Count > 0)
            {
                starterViews.ForEach(_view => _view.Show(ViewOrder.None, true));
            }
        }
    }

    private void RegisterView(BaseView view)
    {
        Type viewType = view.GetType();
        if (!activeViewsCache.ContainsKey(viewType))
        {
            activeViewsCache.Add(viewType, view);
            view.Setup(this);
        }
        else
        {
            Debug.LogWarning($"View {viewType.ToString()} has already exist");
        }
    }

    private T SpawnView<T>(T view, Transform container) where T : BaseView
    {
        T trans = Instantiate(view, container);
        trans.gameObject.name = view.gameObject.name;
        return trans;
    }

    public T ShowView<T>(ViewOrder order = ViewOrder.None) where T : BaseView
    {
        T view = GetView<T>();
        if (view != null)
        {
            view.Show(order);
        }
        else
        {
            Debug.LogError($"UI View error: view {typeof(T)} is null!");
        }
        return view;
    }

    public void HideView<T>() where T : BaseView
    {
        T view = GetView<T>();
        if (view != null)
        {
            view.Hide();
        }
        else
        {
            Debug.LogError($"View {typeof(T)} is not registered!");
        }
    }

    public T GetView<T>() where T : BaseView
    {
        if (activeViewsCache.TryGetValue(typeof(T), out BaseView view))
        {
            return view as T;
        }
        Debug.LogWarning($"Scene view {typeof(T)} is not registered. Start finding on prefab list!");
        T prefabView = GetPrefabView<T>();
        if (prefabView != null)
        {
            Transform container = prefabView.IsPopup ? (prefabView.IsOverlayUI ? m_OverlayPopupContainer : m_PopupContainer) : (prefabView.IsOverlayUI ? m_OverlayPanelContainer : m_PanelContainer);
            T spawnedView = SpawnView(prefabView, container);
            RegisterView(spawnedView);
            return spawnedView;
        }
        Debug.LogError($"UI view {typeof(T)} is not registered on both scene and prefab list!");
        return null;
    }

    public T GetPrefabView<T>() where T : BaseView
    {
        foreach (var view in prefabViews)
        {
            if (view is T _view)
            {
                return _view;
            }
        }
        Debug.LogError($"Prefab view {typeof(T)} is not registered");
        return null;
    }

    public static T ShowUIView<T>() where T : BaseView
    {
        return Instance.ShowView<T>();
    }

    public static void HideUIView<T>() where T : BaseView
    {
        Instance.HideView<T>();
    }

    public static T GetUIView<T>() where T : BaseView
    {
        return Instance.GetView<T>();
    }
    #endregion
}
