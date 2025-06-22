using Core.Utils;
using Game;
using Game.Websocket;
using Game.Websocket.Model;
using Minigame.Scrambler;
using PathologicalGames;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SpaceCommand
{
    Diamond = 0,
    Exp = 1,
    ItemPvp = 2,
    Test_AFK = 3,
    Test_DailyReward = 4,
    IncreaseTicket = 5,
    DecreaseTicket = 6,
    GetTicket = 7,
    CheckPetStats = 8,
}

public enum MinigameType
{
    GotchiDrop = 0,
    Suika = 1,
}

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Inoke every seconds when game runs.
    /// </summary>
    public static Action<int> OnGameTimeChange;

    public static GameManager Instance;

    [SerializeField] private UIManager m_UIManager;
    [SerializeField] private PetController m_Pet;
    [SerializeField] private TutorialHandler m_TutorialHandler;
    [SerializeField] private DailyRewardManager m_DailyRewardManager;
    [SerializeField] private GameObject m_ScramblerGO;
    [SerializeField] private FrameByFrameBackgroundHandler m_BackgroundHandler;
    [SerializeField] private ListPoopsController m_ListPoopsController;
    [SerializeField] private BoostHandler m_BoostHandler;
    [SerializeField] private CombatHandler m_CombatHandler;
    [SerializeField] private GameObject m_RobotGO;
    [SerializeField] private Transform m_SuikaMinigame;

    [Tooltip("The amount of seconds passed before perform a save action")]
    [SerializeField] private int saveInterval = 60;
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI m_VersionText;
    [SerializeField] private SpaceCommand cheatCommand;
    [SerializeField] private int intValue1 = 6;

    private MinigameType currentMinigame;
    private UIMainHomePanel m_MainHome;

    private Transform gotchiDropInstance;
    private Transform suikaInstance;

    public UIMainHomePanel MainHome
    {
        get
        {
            if (m_MainHome == null)
            {
                m_MainHome = m_UIManager.GetView<UIMainHomePanel>();
            }
            return m_MainHome;
        }
    }

    public PetController PetController => m_Pet;
    public UIManager UIManager => m_UIManager;
    public ListPoopsController ListPoopsController => m_ListPoopsController;
    public BoostHandler BoostHandler => m_BoostHandler;
    public TutorialHandler TutorialHandler => m_TutorialHandler;
    public DailyRewardManager DailyRewardManager => m_DailyRewardManager;
    public CombatHandler PvpCombatHandler => m_CombatHandler;
    public MinigameType CurrentMinigame => currentMinigame;
    public string MinigameID
    {
        get
        {
            return $"G0{((int)CurrentMinigame) + 1}";
        }
    }

    public int OnlineTime { get; private set; }
    public bool IsInitialized { get; set; }

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 45;
        PlayerData.Instance.LoadData(() =>
        {
            PlayerData.Instance.onLevelUp += OnPetLevelUp;

            m_UIManager.InitUI();
            OnLoadTutorial();
            if (PlayerData.Instance.data.selectedPetID != -1)
            {
                PlayerData.Instance.PetData.ScheduleNextPoop();

                PetController.AnimController.SetOnChangePetPhase(OnChangePetPhase);
                PetController.InitPet(PlayerData.Instance.data.selectedPetID);

                m_UIManager.SetExpBarTextOnStart(PlayerData.Instance.PetData.petExp >= PlayerData.Instance.PetData.GetPetMaxExp);
                LoadTargetTime();
                //m_UIManager.OnDoingNewBieMission();
                if (!PlayerData.Instance.userInfo.isCompensation)
                {
                    m_UIManager.ShowView<PopupNotify>().Init("CONGRATULATION", "You received 3 diamonds and 400 gotchipoints", 0, false, false);
                    PlayerData.Instance.AddCurrency(CurrencyType.Diamond, 3);
                    WebSocketRequestHelper.CollectCompensationOnce();
                }
                PlayerData.Instance.GetCurrency(CurrencyType.Diamond);
                PlayerData.Instance.GetCurrency(CurrencyType.Ticket);

                SetBG(PlayerData.Instance.PetData.currentBackgroundIndex);
                DailyRewardManager.InitRewardData();
            }
            else
            {
                m_UIManager.OnStartGame();
            }

            IsInitialized = true;

        });
        
        StartCoroutine(TimerCoroutine());
    }

    private void OnDestroy()
    {
        PlayerData.Instance.onLevelUp -= OnPetLevelUp;
    }

    private void FixedUpdate()
    {
        if (PlayerData.Instance.data.selectedPetID != -1 && IsInitialized)
        {
            if (PlayerData.Instance.PetData.petPhase == PetPhase.Hatching && PlayerData.Instance.data.gameState != GameState.Hatching_Wait)
            {
                UpdateCountdown();
            }
        }
    }

    private void Update()
    {
        if (PlayerData.Instance.data.selectedPetID == -1 || !IsInitialized)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //SoundManager.Instance.PlayVFX("2. Screen Touch");

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (PlayerData.Instance.PetData != null && !PlayerData.Instance.PetData.pet_status.IsSleeping)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    PetController.AnimController.OnInteracted(ActionType.Interact);
                }
            }
        }
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            OnlineTime++;
            OnGameTimeChange?.Invoke(OnlineTime);
        }
    }
    #endregion

    public void StartPvpCombat(PvpCombat combat)
    {
        m_CombatHandler.gameObject.SetActive(true);
        m_CombatHandler.InitCombat(combat);
    }

    public void SelectPetOnStart()
    {
        PetController.AnimController.SetOnChangePetPhase(OnChangePetPhase).InitPetAnim(PlayerData.Instance.PetData.petId);
        SetBG(PlayerData.Instance.PetData.currentBackgroundIndex);
    }

    public void NextBackground()
    {
        PlayerData.Instance.PetData.currentBackgroundIndex++;
        SetBG(PlayerData.Instance.PetData.currentBackgroundIndex);
    }

    public void SetBG(int index)
    {
        if (index >= PlayerData.Instance.data.ownedBackgroundIds.Count)
            index = 0;
        PlayerData.Instance.PetData.currentBackgroundIndex = index;
        m_BackgroundHandler.SetBackgroundIndex(index);
        PlayerData.Instance.ChangeBG(index);
    }

    public void CleanPoops()
    {
        m_ListPoopsController.CleanPoops();
    }

    #region Pet Commands
    public void RevivePet(Action<RevivePetResponse> callback)
    {
        PetController.RequestRevive(callback);
    }

    public void UseItemForPet(int itemId, ItemCategory itemType, bool forcedWhenSleep, Action onCompleted)
    {
        PetController.UseItemForPet(itemId, itemType, forcedWhenSleep, onCompleted);
    }

    public void ShowerPet(Action onCompleted = null)
    {
        PetController.SendShowerRequest(onCompleted);
    }

    public void CleanPetPoops()
    {
        ListPoopsController.CleanPoops();
    }

    public void EvolvePet(Action<EvolveResultBody> onCompleted = null)
    {
        PetController.SendEvolveRequest(onCompleted);
    }
    #endregion

    #region Timer
    private DateTime targetTime;
    private DateTime currentTime;
    private TimeSpan countdownDuration = TimeSpan.FromSeconds(1800);
    private bool targetTimeLoaded = false;
    private float addMoreSecond = 0;

    public void LoadTargetTime()
    {
        if (PlayerData.Instance.data.selectedPetID == -1 || PlayerData.Instance.PetData.petPhase == PetPhase.Opened)
            return;
        targetTimeLoaded = false;
        if (!string.IsNullOrEmpty(PlayerData.Instance.PetData.targetTime))
        {
            LoggerUtil.Logging("Load_Target_Time", $"PetId={PlayerData.Instance.PetData.petId}");
            WebSocketRequestHelper.GetTimeOnce((time) =>
            {
                targetTime = GameUtils.ParseTime(PlayerData.Instance.PetData.targetTime);
                currentTime = GameUtils.ParseTime(time);
                elapsedTime = targetTime - currentTime;
                targetTimeLoaded = true;
                LoggerUtil.Logging("LoadTargetTime", $"elapsedTime={elapsedTime}");
            });
        }
        else
        {
            WebSocketRequestHelper.GetTimeOnce((time) =>
            {
                countdownDuration = TimeSpan.FromSeconds(GameUtils.TIME_EGG_OPENED_START);
                targetTime = GameUtils.ParseTime(time).Add(countdownDuration);
                currentTime = GameUtils.ParseTime(time);
                elapsedTime = targetTime - currentTime;
                PlayerData.Instance.PetData.targetTime = GameUtils.GetSaveDateString(targetTime);
                targetTimeLoaded = true;
            });
        }
    }

    private TimeSpan elapsedTime;

    public void ReduceTimeWhenCompleteTask()
    {
        addMoreSecond += 600;
    }

    private void UpdateCountdown()
    {
        if (targetTime == DateTime.MinValue || !targetTimeLoaded)
            return;

        elapsedTime -= TimeSpan.FromSeconds(Time.fixedDeltaTime + addMoreSecond);
        //currentTime = currentTime.Add(TimeSpan.FromSeconds(Time.deltaTime + addMoreSecond));
        addMoreSecond = 0;
        //TimeSpan remainingTime = targetTime - currentTime;
        if (elapsedTime <= TimeSpan.Zero)
        {
            elapsedTime = TimeSpan.Zero;
            MainHome.UpdateTimerText("00:00", 1);
            // Handle the countdown completion logic here
            if (m_UIManager.newBieTransform != null)
            {
                m_UIManager.newBieTransform.gameObject.SetActive(false);
            }
            PetController.AnimController.OpenEgg();
        }
        else
        {
            if (elapsedTime.Hours < 1)
                MainHome.UpdateTimerText($"{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}", (float)(elapsedTime.TotalSeconds / countdownDuration.TotalSeconds));
            else
                MainHome.UpdateTimerText($"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}", (float)(elapsedTime.TotalSeconds / countdownDuration.TotalSeconds));
        }
    }
    #endregion

    #region Tutorial
    private void OnLoadTutorial()
    {
        if (PlayerData.Instance.data.isTutorialDone || PlayerData.Instance.data.selectedPetID == -1)
        {
            return;
        }

        switch (PlayerData.Instance.data.completedPhase)
        {
            case TutorialPhase.None:
                OnOpenTutorial(() =>
                {
                    //OnDoingNewBieMission();
                });
                break;
            case TutorialPhase.LookAtEgg:
                if (PlayerData.Instance.data.tutorialPhase == TutorialPhase.CongrateToHatchingEgg)
                {

                    TutorialHandler.gameObject.SetActive(true);
                    TutorialHandler.LoadTutorialFromSave();
                    TutorialHandler.TutorialContainer.SetActive(false);
                    //OnDoingNewBieMission();
                }
                break;
            case TutorialPhase.CongrateToHatchingEgg:
                TutorialHandler.gameObject.SetActive(true);
                TutorialHandler.LoadTutorialFromSave();
                TutorialHandler.TutorialContainer.SetActive(false);
                break;
        }
    }

    public void OnOpenTutorial(ICallback.CallFunc callback)
    {
        TutorialHandler.gameObject.SetActive(true);
        TutorialHandler.InitTutorial(callback);
    }
    #endregion

    private void OnPetLevelUp(bool isEvolve, bool canUpdateUI)
    {
        if (!isEvolve)
            return;

        if (canUpdateUI)
            return;

        PetController.InitPet(PlayerData.Instance.PetData.petId);
    }

    private void OnChangePetPhase(PetPhase phase)
    {
        switch (phase)
        {
            case PetPhase.Hatching:
                MainHome.OnHatchingEggs();
                break;
            case PetPhase.Opened:
                MainHome.OnEggOpened();
                // Continue tutorial.
                if (!PlayerData.Instance.data.isTutorialDone)
                {
                    if (!TutorialHandler.gameObject.activeSelf)
                        TutorialHandler.gameObject.SetActive(true);
                    TutorialHandler.SetTutorial(TutorialPhase.HatchingCompleted);
                }
                break;
        }
    }


    public void ShowRobot()
    {
        m_RobotGO.SetActive(true);
    }

    public void HideRobot()
    {
        m_RobotGO.SetActive(false);
    }

    #region Minigame
    public void PlayMinigame(MinigameType contentType)
    {
        currentMinigame = contentType;
        SoundManager.Instance.PlayBackgroundMusic("17.MinigameBG");
        HideAllNormalViews();
        PetController.DisableCollider();
        m_BackgroundHandler.gameObject.SetActive(false);

        switch (contentType)
        {
            case MinigameType.GotchiDrop:
                gotchiDropInstance = PoolManager.Pools["Minigame"].Spawn(m_ScramblerGO.transform, Vector3.zero, Quaternion.identity);
                gotchiDropInstance.GetComponent<ScramblerManager>().InitializeMinigame();
                break;
            case MinigameType.Suika:
                suikaInstance = PoolManager.Pools["Minigame"].Spawn(m_SuikaMinigame, Vector3.zero, Quaternion.identity);
                suikaInstance.GetComponent<GameController>().InitializeMinigame();
                break;
        }
        FirebaseAnalytics.instance.LogCustomEvent("user_play_minigame");
    }

    public void EndMinigame()
    {
        SoundManager.Instance.PlayBackgroundMusic("01. Background");
        m_BackgroundHandler.gameObject.SetActive(true);
        PetController.EnableCollider();

        switch (currentMinigame)
        {
            case MinigameType.GotchiDrop:
                OnMinigameGameOver();
                break;
            case MinigameType.Suika:
                suikaInstance.GetComponent<GameController>().ExitSuika();
                break;
        }
        m_UIManager.HideView<UIMinigameHome>();
        m_UIManager.HideView<UIMinigameLeaderboard>();

        m_UIManager.ShowView<UIMainHomePanel>();
        m_UIManager.ShowView<UIHeaderFooterOnly>();
    }

    public void OnMinigameGameOver()
    {
        if (PoolManager.Pools["Platform"].Count > 0)
            PoolManager.Pools["Platform"].DespawnAll();
        if (PoolManager.Pools["Enemy"].Count > 0)
            PoolManager.Pools["Enemy"].DespawnAll();
        if (PoolManager.Pools["Leaderboard"].Count > 0)
            PoolManager.Pools["Leaderboard"].DespawnAll();

        PoolManager.Pools["Minigame"].Despawn(gotchiDropInstance);
    }

    private void HideAllNormalViews()
    {
        m_UIManager.HideView<UIMainHomePanel>();
        m_UIManager.HideView<UIHeaderFooterOnly>();
        m_UIManager.HideView<PVPController>();
    }
    #endregion
}