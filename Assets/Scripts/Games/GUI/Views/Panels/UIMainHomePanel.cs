using Game;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainHomePanel : UINormalInventory
{
    public Action OnInvokeClean;
    [Header("UI.HOME")]
    [Header("Containers")]
    [SerializeField] private GameObject m_TimerGO;
    [SerializeField] private Button m_TimerBtn;
    [SerializeField] private GameObject m_NormalFieldsGO;

    [Header("Header.DropDown")]
    [SerializeField] private GameObject m_DropdownGO;
    [SerializeField] private Button m_DropDownBtn;
    [SerializeField] private Button m_InventoryBtn;
    [SerializeField] private Button m_ShopBtn;
    [SerializeField] private Button m_BGToggleBtn;
    [SerializeField] private TextMeshProUGUI m_TimerText;
    [SerializeField] private Image m_TimerBar;

    [Header("Header.Boosts")]
    [SerializeField] private UIBoostField m_RobotBoost;
    [SerializeField] private UIBoostField m_TicketBoost;

    [Header("Above footer (Statuses)")]
    [SerializeField] private GameObject m_StatusField;
    [SerializeField] private UIStatusField m_HappynessField;
    [SerializeField] private UIStatusField m_HygieneField;
    [SerializeField] private UIStatusField m_HungerField;

    [Header("Sub-Views")]
    [SerializeField] private UISocialBoard m_SocialBoard;

    [Header("Board-Fields.Normal")]
    [SerializeField] private Button m_ClaimBtn;
    [SerializeField] private Button m_FriendBtn;
    [SerializeField] private Button m_RankingBtn;
    [SerializeField] private Button m_TasksBtn;

    [Header("Board-Fields.Minigame")]
    [SerializeField] private Button m_GotchiDropBtn;
    [SerializeField] private Button m_SuikaBtn;

    [Header("Pet actions")]
    [SerializeField] private Transform m_ActionField;
    [SerializeField] private Button m_CleanBtn;
    [SerializeField] private Button m_CureBtn;
    [SerializeField] private Button m_EvolveBtn;
    [SerializeField] private Transform m_ReviveField;
    [SerializeField] private Button m_ReviveBtn;

    private UIHeaderFooterOnly m_HeaderFooter;
    private Dictionary<int, InventoryItem> inventoryItems;

    protected UIHeaderFooterOnly UIHeaderFooter
    {
        get
        {
            if (m_HeaderFooter == null)
            {
                m_HeaderFooter = GetUIView<UIHeaderFooterOnly>();
            }
            return m_HeaderFooter;
        }
    }

    protected override void OnSetup()
    {
        base.OnSetup();
        GameManager.Instance.PetController.onUpdateStatus += OnUpdateStatusUI;
        GameManager.Instance.PetController.OnLevelUp += OnPetLevelUp;
        GameManager.Instance.PetController.OnSickCallback += ShowCureButton;

        if (PlayerData.Instance.PetData != null)
        {
            GameManager.Instance.ListPoopsController.OnPoop += () =>
            {
                if (!m_CleanBtn.gameObject.activeSelf)
                    m_CleanBtn.gameObject.SetActive(true);
            };
            GameManager.Instance.BoostHandler.OnBoostsLoaded += UpdateLoadedUIBoosts;
            GameManager.Instance.BoostHandler.OnUpdateProgress += OnUpdateBoostTime;
        }
        AdjustReviveButton();
    }

    protected override void OnViewShown()
    {
        GameManager.Instance.PetController.OnDeadCallback += ShowReviveBtn;
        m_DropDownBtn.onClick.AddListener(ToggleDropdown);
        m_InventoryBtn.onClick.AddListener(ShowInventory);
        m_ShopBtn.onClick.AddListener(ShowShop);
        m_BGToggleBtn.onClick.AddListener(OnChangeBackground);
        m_TimerBtn.onClick.AddListener(ShowChangePet);

        m_HappynessField.AddListener(ShowToys);
        m_HungerField.AddListener(ShowFood);
        m_HygieneField.AddListener(Shower);

        m_ClaimBtn.onClick.AddListener(ShowAFK);
        m_FriendBtn.onClick.AddListener(ShowFriends);
        m_RankingBtn.onClick.AddListener(ShowRanking);
        m_TasksBtn.onClick.AddListener(ShowTasks);

        m_GotchiDropBtn.onClick.AddListener(PlayGotchiDropMinigame);
        m_SuikaBtn.onClick.AddListener(PlaySuikaMinigame);

        m_CleanBtn.onClick.AddListener(CleanPoops);
        m_CureBtn.onClick.AddListener(ShowCurePopup);
        m_EvolveBtn.onClick.AddListener(ShowEvolvePopup);

        m_ReviveBtn.onClick.AddListener(Revive);
        AdjustActionFields();
        SetStatusFieldState(true);
        if (PlayerData.Instance.PetData != null)
        {
            if (GameManager.Instance.BoostHandler.OnBoostsLoaded == null)
                GameManager.Instance.BoostHandler.OnBoostsLoaded += UpdateLoadedUIBoosts;
            if (GameManager.Instance.BoostHandler.OnUpdateProgress == null)
                GameManager.Instance.BoostHandler.OnUpdateProgress += OnUpdateBoostTime;
        }
    }

    protected override void OnViewHidden()
    {
        GameManager.Instance.PetController.OnDeadCallback -= ShowReviveBtn;

        m_DropDownBtn.onClick.RemoveListener(ToggleDropdown);
        m_TimerBtn.onClick.RemoveListener(ShowChangePet);

        m_InventoryBtn.onClick.RemoveListener(ShowInventory);
        m_ShopBtn.onClick.RemoveListener(ShowShop);
        m_BGToggleBtn.onClick.RemoveListener(OnChangeBackground);

        m_HappynessField.RemoveListener(ShowToys);
        m_HungerField.RemoveListener(ShowFood);
        m_HygieneField.RemoveListener(Shower);

        m_ClaimBtn.onClick.RemoveListener(ShowAFK);
        m_FriendBtn.onClick.RemoveListener(ShowFriends);
        m_RankingBtn.onClick.RemoveListener(ShowRanking);
        m_TasksBtn.onClick.RemoveListener(ShowTasks);

        m_GotchiDropBtn.onClick.RemoveListener(PlayGotchiDropMinigame);
        m_SuikaBtn.onClick.RemoveListener(PlaySuikaMinigame);

        m_CleanBtn.onClick.RemoveListener(CleanPoops);
        m_CureBtn.onClick.RemoveListener(ShowCurePopup);
        m_EvolveBtn.onClick.RemoveListener(ShowEvolvePopup);

        m_ReviveBtn.onClick.RemoveListener(Revive);

        m_DropdownGO.SetActive(false);

        HideUIView<ShopManager>();
        HideUIView<UINormalInventory>();
        HideItems(); 
    }

    private void ShowReviveBtn()
    {
        m_ReviveField.gameObject.SetActive(true);
    }

    private void HideReiveBtn()
    {
        m_ReviveField.gameObject.SetActive(false);
    }

    private void ShowChangePet()
    {
        if (PlayerData.Instance.data.isTutorialDone && NeedMoreThan1Pet())
        {
            ShowUIView<UISelectPetPopup>().LoadPetsPopup(UISelectPetPopup.UIPetType.All);
        }
    }

    private bool NeedMoreThan1Pet()
    {
        return PlayerData.Instance.userInfo.pets.Count > 1;
    }

    private void AdjustReviveButton()
    {
        m_ReviveField.position = GameManager.Instance.PetController.TombGO.transform.position;
    }

    private void Revive()
    {
        GameManager.Instance.RevivePet((RevivePetResponse response) => { 
            if (response != null)
            {
                if (!response.isDead)
                {
                    HideReiveBtn();
                }
                else
                {
                    Debug.Log("<Color=red>Cannot revive pet</color>");
                }
            }
        });
    }

    private void AdjustActionFields()
    {
        if (m_ActionField == null) return;

        if (Screen.height < 1280)
        {
            Vector3 worldPosition = GameManager.Instance.PetController.transform.position + new Vector3(0, 0.6f, 0);

            // Convert world position to screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // Convert screen position to local position relative to the UI canvas
            RectTransform canvasRect = m_ActionField.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                Camera.main,
                out Vector2 localPoint
            );

            // Set the local position of the UI element
            m_ActionField.localPosition = localPoint;
        }
    }

    private Vector3 GetActionFieldInWorldSpace()
    {
        Vector3 uiScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, m_ActionField.position);
        Vector3 bottomUIWorldPosition = Camera.main.ScreenToWorldPoint(uiScreenPosition);
        return bottomUIWorldPosition;
    }

    protected override void OnBuyMoreItem()
    {
        base.OnBuyMoreItem();
        HideItems();
    }

    public void ShowHomeTab()
    {
        HideItems();
        m_SocialBoard.HideBoard();
    }

    public void ShowSocialTab(UISocialBoard.BoardContentType contentType = UISocialBoard.BoardContentType.Normal)
    {
        m_SocialBoard.ShowBoard(contentType);
    }

    private void HideItems()
    {
        SetItemViewState(false);
        m_HungerField.Deselect();
        m_HappynessField.Deselect();
        PoolManager.Pools["InventoryItem"].DespawnAll();
    }

    private void ShowToys(bool isActive)
    {
        if (!CarePetCondition(ActionType.Toy, out string errorMessage))
        {
            Toast.Show(errorMessage);
            return;
        }
        if (isActive)
        {
            LoadItems(ActionType.Toy);
            m_HungerField.Deselect();
            m_HappynessField.Select();
        }
        else
        {
            HideItems();
        }
    }

    private void ShowFood(bool isActive)
    {
        if (!CarePetCondition(ActionType.Feed, out string errorMessage)) 
        {
            Toast.Show(errorMessage);
            return;
        }
        if (isActive)
        {
            LoadItems(ActionType.Feed);
            m_HappynessField.Deselect();
            m_HungerField.Select();
        }
        else
        {
            HideItems();
        }
    }

    private void OnUpdateStatusUI(StatusType type, float value)
    {
        switch (type)
        {
            case StatusType.Happyness:
                m_HappynessField.SetStatValue(value, GameUtils.MAX_HAPPYNESS_VALUE);
                break;

            case StatusType.Hunger:
                m_HungerField.SetStatValue(value, GameUtils.MAX_HUNGER_VALUE);
                break;

            case StatusType.Hygiene:
                m_HygieneField.SetStatValue(value, GameUtils.MAX_HYGIENEV_VALUE);
                break;
            case StatusType.Health:
                //m_HealthField.SetStatValue(value, GameUtils.MAX_HEALTH_VALUE);
                break;
        }
    }

    private void OnPetLevelUp(int petLevel, bool isEvolve, bool isLevelUp)
    {
        if (isEvolve)
        {
            //m_LevelText.text = "Evolve!!";
            SetEvolveButtonState(true);
            m_EvolveBtn.gameObject.SetActive(true);
        }
        else if (!isEvolve && isLevelUp)
        {
            FirebaseAnalytics.instance.LogCustomEvent($"pet_level_up_from{petLevel - 1} to {petLevel}");

            //m_LevelText.text = "LEVEL " + (PlayerData.Instance.PetData.petLevel);
            SetEvolveButtonState(false);

            if (petLevel < 15)
                ShowUIView<PopupLevelupReward>().InitDataLevelUpReward();
            else
                ShowUIView<PopupLevel15Up>().InitDataLevelUpReward();
        }
        else
        {
            SetEvolveButtonState(false);
        }
        UIHeaderFooter.SetUILevel(petLevel);
    }

    public void OnHatchingEggs()
    {
        m_TimerGO.SetActive(true);
        m_NormalFieldsGO.SetActive(false);
        HideUIView<UIHeaderFooterOnly>();
        m_SocialBoard.HideBoard();
        UpdateUIBoost(false, GameUtils.TICKET_POTION_BOOST);
        UpdateUIBoost(false, GameUtils.ROBOT_BOOST);
    }

    protected override void RefreshInventory()
    {
        SetItemViewState(false);
        m_HappynessField.Deselect();
        m_HungerField.Deselect();
    }

    public void OnEggOpened()
    {
        m_TimerGO.SetActive(false);
        m_NormalFieldsGO.SetActive(true);
        ShowUIView<UIHeaderFooterOnly>();
        if (PlayerData.Instance.PetData != null)
        {
            if (GameManager.Instance.BoostHandler.OnBoostsLoaded == null)
                GameManager.Instance.BoostHandler.OnBoostsLoaded += UpdateLoadedUIBoosts;
            if (GameManager.Instance.BoostHandler.OnUpdateProgress == null)
                GameManager.Instance.BoostHandler.OnUpdateProgress += OnUpdateBoostTime;
        }
    }

    public void SetStatusFieldState(bool state)
    {
        m_StatusField.SetActive(state);
    }

    private bool CarePetCondition(ActionType action, out string errorMessage)
    {
        return GameManager.Instance.PetController.CarePetCondition(action, false, out errorMessage);
    }

    #region Hatching Egg Timers
    public void UpdateTimerText(string text, float fillAmout)
    {
        m_TimerBar.fillAmount = fillAmout;
        m_TimerText.text = text;
    }
    #endregion

    #region Dropdown actions
    private void ToggleDropdown()
    {
        m_DropdownGO.SetActive(!m_DropdownGO.activeSelf);
    }

    private void ShowInventory()
    {
        m_SocialBoard.HideBoard();
        ShowUIView<UINormalInventory>();
    }

    private void ShowShop()
    {
        m_SocialBoard.HideBoard();
        ShowUIView<ShopManager>().ShowTab(ShopButtonTab.Items);
    }

    private void OnChangeBackground()
    {
        GameManager.Instance.NextBackground();
    }
    #endregion

    #region Board actions
    public void ShowAFK()
    {
        m_SocialBoard.ShowBoard(UISocialBoard.BoardContentType.AFK);
    }

    private void ShowFriends()
    {
        ShowUIView<FriendHandler>().Init();
    }

    private void ShowRanking()
    {

    }

    private void ShowTasks()
    {
        ShowUIView<TaskHandler>().InitTask();
    }

    private void PlayGotchiDropMinigame()
    {
        LoggerUtil.Logging("PLAY_GOTCHI_DROP");
        GameManager.Instance.PlayMinigame(MinigameType.GotchiDrop);
    }

    private void PlaySuikaMinigame()
    {
        LoggerUtil.Logging("PLAY_SUIKA");
        GameManager.Instance.PlayMinigame(MinigameType.Suika);
    }
    #endregion

    #region Pet UI Actions
    public void ShowCureButton(bool isActive)
    {
        m_CureBtn.gameObject.SetActive(isActive);
    }

    private void Shower(bool isActive)
    {
        m_HungerField.Deselect();
        m_HappynessField.Deselect();
        GameManager.Instance.ShowerPet();
    }

    public void SetEvolveButtonState(bool active)
    {
        m_EvolveBtn.gameObject.SetActive(active);
    }

    private void CleanPoops()
    {
        GameManager.Instance.CleanPetPoops();
        m_CleanBtn.gameObject.SetActive(false); 
    }

    private void ShowCurePopup()
    {
        ShowUIView<UIMedicineListPopup>();
    }

    private void ShowEvolvePopup()
    {
        ShowUIView<PopupEvolve>().SetOnConfirmEvolveCallback(ShowEvolveResult).InitDataEvolve();
    }

    private void ShowEvolveResult(EvolveResultBody result)
    {
        if (result.evolve)
        {
            GameManager.Instance.PetController.PetNeedToEvolve = false;
            ShowUIView<PopupLevel15Up>().InitDataLevelUpReward();
            GameManager.Instance.PetController.InitPet(PlayerData.Instance.data.selectedPetID, true);
        }
        else
        {
            if (result.msg.Equals("item"))
            {
                ShowUIView<PopupEvolveFailed>().OnInitDataEvolveFailed();
            }
            else if (result.msg.Equals("dice"))
            {
                ShowEvolveResultFail();
            }
        }
    }

    private void ShowEvolveResultFail()
    {
        ShowUIView<PopupLevelUpFailed>().SetTryAgainCallback(ShowEvolvePopup).InitDataEvolveFailed();
    }
    #endregion

    #region Boosts
    private void UpdateLoadedUIBoosts(List<BoostItem> playerBoosts, List<BoostItem> petBoosts)
    {
        if (petBoosts.Any(item => item.boostId == GameUtils.ROBOT_BOOST))
        {
            UpdateUIBoost(true, GameUtils.ROBOT_BOOST);
        }
        else
        {
            UpdateUIBoost(false, GameUtils.ROBOT_BOOST);
        }

        if (playerBoosts.Any(item => item.boostId == GameUtils.TICKET_POTION_BOOST))
        {
            UpdateUIBoost(true, GameUtils.TICKET_POTION_BOOST);
        }
        else
        {
            UpdateUIBoost(false, GameUtils.TICKET_POTION_BOOST);
        }
    }

    private void UpdateUIBoost(bool isAdd, int id)
    {
        if (isAdd)
        {
            if (id == GameUtils.ROBOT_BOOST)
            {
                GameManager.Instance.ShowRobot();
                m_RobotBoost.gameObject.SetActive(true);
                CleanPoops();
            }

            if (id == GameUtils.TICKET_POTION_BOOST)
            {
                if (!m_TicketBoost.gameObject.activeSelf)
                {
                    m_TicketBoost.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (id == GameUtils.ROBOT_BOOST)
            {
                GameManager.Instance.HideRobot();
                m_RobotBoost.gameObject.SetActive(false);
            }
            if (id == GameUtils.TICKET_POTION_BOOST)
            {
                m_TicketBoost.gameObject.SetActive(false);
            }
        }
    }

    private void OnUpdateBoostTime(int id, float remainTime)
    {
        switch (id)
        {
            case GameUtils.ROBOT_BOOST:
                m_RobotBoost.SetTime(remainTime);
                break;
            case GameUtils.TICKET_POTION_BOOST:
                m_TicketBoost.SetTime(remainTime);
                break;
        }
    }
    #endregion
}
