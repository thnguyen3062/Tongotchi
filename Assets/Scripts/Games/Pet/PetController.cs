using Core.Utils;
using DG.Tweening;
using Game;
using Game.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class PetController : MonoBehaviour
{
    private const bool HAS_DEAD_FEATURE = false;

    private const float poopRate = 2700f;                 // 45 mins
    private const int TIME_2_DAYS = 129600;
    private const int TIME_REQUEST_CHECK_SICK = 60 * 20;
    private const int TAKE_ACTION_TIME = 5 * 60;          // Robot will perform action after 5 minutes when the pet has total stats greater than 35%.
    private const int TAKE_ACTION_WHEN_HAS_BOOST = 30;    // Robot will take action after 30 seconds when the pet has total stats below 35%.
    private const int CHECK_STATS_DURATION = 11 * 60;     // 11 minutes

    public Action<GamePetData> OnReceiveEvolvedPet;

    public Action OnDeadCallback;
    public Action<bool> OnSickCallback;
    public Action<float, float, bool> OnExpChange;
    public Action<int, bool, bool> OnLevelUp;
    public Action OnCanEvolve;

    public Action OnCleanPoop;
    public Action<ActionType> OnInteract;
    public ICallback.CallFunc3<StatusType, float> onUpdateStatus;

    [SerializeField] private bool lockActionsWhenSleep;
    [SerializeField] private PetAnimController _animController;
    [SerializeField] private Collider2D m_Collider;
    [SerializeField] private GameObject m_Tomb;

    private NativeArray<float> statuses;
    private NativeArray<StatusType> statusTypes;

    private bool isInitialized = false;
    private int remainSleepTime;
    private int _robotTakeActionElapsedTime;
    private int _checkStatsElapsedTime;
    private int _nextPoopTime;

    public PetAnimController AnimController => _animController;
    public bool LockActionsWhenSleep => lockActionsWhenSleep;
    public GameObject TombGO => m_Tomb;
    public bool PetNeedToEvolve {  get; set; }

    private void Awake()
    {
        _animController = GetComponent<PetAnimController>();

        GameManager.OnGameTimeChange += OnGameTimeChange;
        OnInteract += _animController.OnInteracted;
        GameManager.Instance.ListPoopsController.OnCleanPoop += () => { OnInteract(ActionType.Clean); };
        _animController.OnEggOpenFinished += OnEggOpened;

        _robotTakeActionElapsedTime = TAKE_ACTION_TIME;
        _checkStatsElapsedTime = CHECK_STATS_DURATION;
    }

    private void OnDestroy()
    {
        GameManager.OnGameTimeChange -= OnGameTimeChange;
        statuses.Dispose();
        statusTypes.Dispose();
    }

    private void Update()
    {
        if (!isInitialized || PlayerData.Instance.data.selectedPetID == -1 || PlayerData.Instance.PetData.petPhase != PetPhase.Opened)
            return;

        statuses[0] = PlayerData.Instance.PetData.status.hunger;
        statuses[1] = PlayerData.Instance.PetData.status.hygiene;
        statuses[2] = PlayerData.Instance.PetData.status.happy;

        //statusTypes[0] = StatusType.Hunger;
        //statusTypes[1] = StatusType.Hygiene;
        //statusTypes[2] = StatusType.Happyness;

        //StatusReductionJob statusJob = new StatusReductionJob
        //{
        //    statuses = statuses,
        //    statusTypes = statusTypes,
        //    deltaTime = Time.deltaTime,
        //    hygieneRate = GetHygieneRate()
        //};

        //JobHandle jobHandle = statusJob.Schedule();

        //jobHandle.Complete();

        UpdateStatusCallback();
        CheckPoop();
    }

    public void InitPet(bool animOnly = false)
    {
        if (PlayerData.Instance.data.selectedPetID  != -1)
        {
            InitPet(PlayerData.Instance.data.selectedPetID, animOnly);
        }
        else
        {
            Debug.LogError("Flow error: Player hasn't selected any pet");
        }
    }

    public void InitPet(int petId, bool animOnly = false)
    {
        _animController.InitPetAnim(petId);
        if (!animOnly)
        {
            InitSelectedPetStats();
        }
    }

    /// <summary>
    /// Called when a pet is selected.
    /// </summary>
    public void InitSelectedPetStats()
    {
        statuses = new NativeArray<float>(3, Allocator.Persistent);
        statusTypes = new NativeArray<StatusType>(3, Allocator.Persistent);

        if (PlayerData.Instance.data.selectedPetID != -1 && PlayerData.Instance.PetData.petPhase == PetPhase.Opened)
        {
            OnInitSelectedPet(); 
            isInitialized = true;
        }
    }

    private void OnInitSelectedPet()
    {
        if (PlayerData.Instance.PetData.pet_status.IsSleeping)
        {
            _animController.Sleep();
        }
        else
        {
            _animController.PetAwake();
        }
        remainSleepTime = PlayerData.Instance.PetData.pet_status.RemainTime;
        LoggerUtil.Logging("SLEEP_STATUS", $"IsSleeping={PlayerData.Instance.PetData.pet_status.IsSleeping}\nTimeToNextState={remainSleepTime}");
        LoadPoops();
        GameManager.Instance.BoostHandler.ReloadBoosts();
        CheckPetStatus();
        if (PlayerData.Instance.PetData.boost.Count > 0)
        {
            RobotAutoFeed(DetermineRobotPerformActionTime);
        }
    }

    private void OnEggOpened()
    {
        GameManager.Instance.UIManager.FadeScreen.gameObject.SetActive(true);
        GameManager.Instance.UIManager.FadeScreen.DOFade(1, 0.2f).OnComplete(() =>
        {
            PlayerData.Instance.PetData.SetPetPhase(PetPhase.Opened);

            StatusData status = new StatusData(GameUtils.START_HAPPYNESS_VALUE, GameUtils.START_HYGIENEV_VALUE, GameUtils.START_HUNGER_VALUE, GameUtils.MAX_HEALTH_VALUE);
            PlayerData.Instance.PetData.status = status;

            PlayerData.Instance.SavePetData((bool success) => {
                WebSocketRequestHelper.LoadPetOnce(PlayerData.Instance.data.selectedPetID, (GamePetData pet) => {
                    PlayerData.Instance.OnSelectPetOnStart(pet);
                    _animController.InitPetAnim(pet.petId);
                    GameManager.Instance.UIManager.FadeScreen.DOFade(0, 0.2f).OnComplete(() => GameManager.Instance.UIManager.FadeScreen.gameObject.SetActive(false));
                    InitPet(PlayerData.Instance.data.selectedPetID);
                });
            });
        });
    }

    public void LoadPoops()
    {
        WebSocketRequestHelper.RequestPetPoop(PlayerData.Instance.PetData.petId, (PetPoopResponse response) => {
            if (response.success)
            {
                _nextPoopTime = response.timeRemaining;
                GameManager.Instance.ListPoopsController.InitPoops(response.poopsSpawned);
            }
        });
    }

    #region Request actions
    public void RequestRevive(Action<RevivePetResponse> callback)
    {
        if (HAS_DEAD_FEATURE && PlayerData.Instance.PetData.isDead)
        {
            int petId = PlayerData.Instance.PetData.petId;
            int diamond = 0;
            switch (PlayerData.Instance.PetData.petEvolveLevel)
            {
                case 0:
                    diamond = 2;
                    break;
                case 1:
                    diamond = 3;
                    break;
                case 2:
                    diamond = 4;
                    break;
                case 3:
                    diamond = 5;
                    break;
                case 4:
                    diamond = 6;
                    break;
            }
            WebSocketRequestHelper.RequestRevivePet(petId, diamond, (RevivePetResponse response) => {
                if (response == null) return;
                PlayerData.Instance.PetData.SetPetStatus(response.status.happy, response.status.hygiene, response.status.hunger);
                m_Tomb.SetActive(response.isDead);
            });
        }
    }

    public void SendEvolveRequest(Action<EvolveResultBody> onCompleted)
    {
        WebSocketRequestHelper.RequestEvolvePet(PlayerData.Instance.PetData.petId, (EvolveResultBody result) => {
            if (result != null)
            {
                LoggerUtil.Logging("SendEvolveRequest.RESPONSE.RESULT", "");
                if (result.evolve)
                {
                    int evolveLevel = result.petEvolveLevel;
                    int petLevel = result.petLevel;
                    PlayerData.Instance.PetData.petLevel = petLevel;
                    PlayerData.Instance.PetData.petEvolveLevel = evolveLevel;
                    OnLevelUp?.Invoke(petLevel, false, true);
                    OnExpChange?.Invoke(PlayerData.Instance.PetData.petExp, PlayerData.Instance.PetData.GetPetMaxExp, false);
                    FirebaseAnalytics.instance.LogCustomEvent($"pet_evolve_from{evolveLevel - 1} to {evolveLevel}");
                }
                onCompleted?.Invoke(result);    
            }
        });
    }

    public void SendFeedRequest(int itemId, Action onCompleted = null)
    {
        WebSocketRequestHelper.FeedPetOnce(PlayerData.Instance.PetData.petId, itemId, (PetStatusesResponse statusResponse, PetLevelExpResponse expResponse) => {
            SetPetStatuses(statusResponse, expResponse);
            _animController.PlayEatFoodAnimation(itemId);
            FirebaseAnalytics.instance.LogCustomEvent("user_care_pet_feed");
            onCompleted?.Invoke();
            OnInteract?.Invoke(ActionType.Feed);
        });
    }

    public void SendShowerRequest(Action onCompleted = null)
    {
        bool isNotDead = !PlayerData.Instance.PetData.isDead;
        bool canShower = PlayerData.Instance.PetData.isSick ? PlayerData.Instance.PetData.status.hygiene < 30 : PlayerData.Instance.PetData.status.hygiene < 90;
        bool isNotSleeping = !PlayerData.Instance.PetData.pet_status.IsSleeping;
        if (isNotDead && canShower && isNotSleeping)
        {
            WebSocketRequestHelper.WashPetOnce(PlayerData.Instance.PetData.petId, (PetStatusesResponse statusResponse, PetLevelExpResponse expResponse) =>
            {
                SetPetStatuses(statusResponse, expResponse);
                OnInteract?.Invoke(ActionType.Shower);
                FirebaseAnalytics.instance.LogCustomEvent("user_care_pet_shower");
                onCompleted?.Invoke();
            });
        }
    }

    public void SendPlayToyRequest(int toyId, Action onCompleted = null)
    {
        WebSocketRequestHelper.JoyPetOnce(PlayerData.Instance.PetData.petId, toyId, (PetStatusesResponse statusResponse, PetLevelExpResponse expResponse) => {
            SetPetStatuses(statusResponse, expResponse);
            FirebaseAnalytics.instance.LogCustomEvent("user_care_pet_play");
            OnInteract?.Invoke(ActionType.Toy);
            onCompleted?.Invoke();
        });
    }

    public void SendUseMedicineRequest(int medicineId, Action onCompleted = null)
    {
        WebSocketRequestHelper.CurePetOnce(PlayerData.Instance.PetData.petId, medicineId, (PetStatusesResponse statusResponse, PetLevelExpResponse expResponse) => {
            PlayerData.Instance.PetData.isSick = statusResponse.isSick;
            SetPetStatuses(statusResponse, expResponse);
            OnInteract?.Invoke(ActionType.Cure);
            FirebaseAnalytics.instance.LogCustomEvent("user_care_pet_cure");
            onCompleted?.Invoke();
            OnSickCallback?.Invoke(statusResponse.isSick);
        });
    }

    public void RequestDie()
    {
        WebSocketRequestHelper.RequestPetDie(PlayerData.Instance.data.selectedPetID, (PetStateResponse state) => {
            if (state.state.Equals("dead"))
            {
                PlayerData.Instance.PetData.isDead = state.isDead;
                OnDeadCallback?.Invoke();
                m_Tomb.SetActive(true);
            }
        });
    }

    public void RequestSick()
    {
        float healthPercent = PlayerData.Instance.PetData.status.healthValue / GameUtils.MAX_HEALTH_VALUE;
        if (!PlayerData.Instance.PetData.isSick && healthPercent < 0.3f)
        {
            WebSocketRequestHelper.RequestPetSick(PlayerData.Instance.data.selectedPetID, (PetStateResponse state) => {
                if (state.state.Equals("sick"))
                {
                    PlayerData.Instance.PetData.isSick = state.isSick;
                    OnSickCallback?.Invoke(state.isSick);
                }
            });
        }
    }
    #endregion

    public void UseItemForPet(int itemId, ItemCategory itemType, bool forcedWhenSleep = false, Action onCompleted = null)
    {
        if (!CarePetCondition(ToActionType(itemType), forcedWhenSleep, out string errorMessage))
        {
            Debug.Log("Some condition is not statisfied!");
            return;
        }
        switch(itemType)
        {
            case ItemCategory.Food:
                SendFeedRequest(itemId, onCompleted);
                break;
            case ItemCategory.Medicine:
                SendUseMedicineRequest(itemId, onCompleted);
                break;
            case ItemCategory.Toy:
                SendPlayToyRequest(itemId, onCompleted);
                break;
        }
    }

    public static ActionType ToActionType(ItemCategory itemType)
    {
        return itemType switch
        {
            ItemCategory.Toy => ActionType.Toy,
            ItemCategory.Food => ActionType.Feed,
            ItemCategory.Special => ActionType.Special,
            ItemCategory.Medicine => ActionType.Cure,
            _ => ActionType.Clean,
        };
    }

    public bool CarePetCondition(ActionType action, bool forceUseWhenSleep, out string errorMessage)
    {
        var pet = PlayerData.Instance.PetData;

        if (pet == null)
        {
            errorMessage = "Error: pet is null";
            return false;
        }

        if (HAS_DEAD_FEATURE && pet.isDead)
        {
            errorMessage = "Pet is dead!";
            return false;
        }

        if (!forceUseWhenSleep && pet.pet_status.IsSleeping)
        {
            errorMessage = "Pet is sleeping";
            return false;
        }

        // Disallow if not sick and stats are too high
        bool notSickDisallowed =
                (action == ActionType.Toy && pet.status.happy >= 90f) ||
                (action == ActionType.Feed && pet.status.hunger >= 90f) ||
                (action == ActionType.Cure && pet.status.healthValue >= 50f);

        // Disallow if sick and stats are too high — except Cure is always allowed
        bool sickDisallowed =
            (action == ActionType.Toy && pet.status.happy >= 30f) ||
            (action == ActionType.Feed && pet.status.hunger >= 30f); 

        bool disallowed = pet.isSick ? sickDisallowed : notSickDisallowed;

        if (disallowed)
        {
            errorMessage = "Condition not suitable for this action.";
            return false;
        }

        errorMessage = "";
        return true;
        //return checkPetNotNull && checkPetNotDead && canPerformActionWhenSleep && (PlayerData.Instance.PetData.isSick ? sickConditions : notSickConditions);
    }

    private void SetPetStatuses(PetStatusesResponse status, PetLevelExpResponse expResponse)
    {
        if (expResponse != null)
        {
            int petCurrentLevel = PlayerData.Instance.PetData.petLevel;
            PlayerData.Instance.PetData.petLevel = expResponse.petLevel;
            PlayerData.Instance.PetData.petExp = expResponse.petExp;
            bool isLevelUp = expResponse.isLevelUp || petCurrentLevel < PlayerData.Instance.PetData.petLevel;
            if (expResponse.needEvolved)
            {
                PetNeedToEvolve = expResponse.needEvolved;
                OnLevelUp?.Invoke(expResponse.petLevel - 1, expResponse.needEvolved, expResponse.isLevelUp);
            }
            else if (isLevelUp)
            {
                OnLevelUp?.Invoke(expResponse.petLevel, false, expResponse.isLevelUp);
            }
            else
            {
                OnLevelUp?.Invoke(expResponse.petLevel, false, false);
            }

            OnExpChange?.Invoke(expResponse.petExp, PlayerData.Instance.PetData.GetPetMaxExp, expResponse.needEvolved);
        }

        if (status != null)
        {
            PlayerData.Instance.PetData.SetPetStatus(status.status.happy, status.status.hygiene, status.status.hunger);

            // If the pet has maid robot, it cannot sick and die.
            if (PlayerData.Instance.PetData.boost.Count == 0)
            {
                if (HAS_DEAD_FEATURE && status.status.healthValue <= 0.001f)
                {
                    RequestDie();
                    return;
                }
                else
                {
                    PlayerData.Instance.PetData.isDead = false;
                }

                RequestSick();
            }
        }
    }

    #region Update Statuses 
    private void UpdateStatusCallback()
    {
        onUpdateStatus?.Invoke(StatusType.Happyness, PlayerData.Instance.PetData.status.happy);
        onUpdateStatus?.Invoke(StatusType.Hunger, PlayerData.Instance.PetData.status.hunger);
        onUpdateStatus?.Invoke(StatusType.Hygiene, PlayerData.Instance.PetData.status.hygiene);
        onUpdateStatus?.Invoke(StatusType.Health, PlayerData.Instance.PetData.status.healthValue);
    }
    #endregion

    #region Maid Robot
    private void RobotAutoFeed(Action onCompleted = null)
    {
        if (!PlayerData.Instance.PetData.boost.Any(item => item.boostId == GameUtils.ROBOT_BOOST))
            return;

        bool needsFeeding = PlayerData.Instance.PetData.status.hunger < 90f;
        bool needsShower = PlayerData.Instance.PetData.status.hygiene < 90f;

        int tasksRemaining = 0;

        if (needsFeeding) ++tasksRemaining;
        if (needsShower) ++tasksRemaining;

        if (tasksRemaining == 0)
        {
            onCompleted?.Invoke(); // nothing to do
            return;
        }

        void OnTaskDone()
        {
            tasksRemaining--;
            if (tasksRemaining <= 0)
                onCompleted?.Invoke();
        }

        if (needsFeeding)
        {
            AutoFeed(ItemCategory.Food, OnTaskDone);
        }

        if (needsShower)
        {
            SendShowerRequest(OnTaskDone);
        }
    }

    private void AutoFeed(ItemCategory category, Action onCompleted)
    {
        Dictionary<int, InventoryItem> useItem = new Dictionary<int, InventoryItem>();

        WebSocketRequestHelper.RequestLoadInventory((Dictionary<int, InventoryItem> items) => {
            foreach (KeyValuePair<int, InventoryItem> item in items) //data.ownedItemDict)
            {
                if (item.Value.data.category == category)
                {
                    useItem.Add(item.Key, item.Value);
                }
            }

            if (useItem.Count <= 0)
                return;

            var sortItems = useItem.OrderBy(pair => pair.Value.data.value);
            SendFeedRequest(sortItems.First().Key, onCompleted);
            //UIManager.instance.OnUseItem(sortItems.First().Key);
        });
    }
    #endregion

    #region Timer update
    public void OnGameTimeChange(int passedTime)
    {
        if (PlayerData.Instance.data.selectedPetID != -1 && PlayerData.Instance.PetData.petPhase == PetPhase.Opened && isInitialized)
        {
            UpdateSleep();
            CheckStatuses();
            UpdateRobot();
            WaitForNextPoop();
        }
    }

    private void CheckStatuses()
    {
        if (_checkStatsElapsedTime <= 0) return;

        --_checkStatsElapsedTime;

        if (_checkStatsElapsedTime <= 0)
        {
            _checkStatsElapsedTime = 0;
            //Request check stats.
            CheckPetStatus();
        }
    }

    private void WaitForNextPoop()
    {
        if (_nextPoopTime <= 0) return;

        --_nextPoopTime;

        if (_nextPoopTime <= 0)
        {
            LoadPoops();
        }
    }

    public void CheckPetStatus(bool resetElapsedTime = true)
    {
        PlayerData.Instance.QueryPetStats((PetStatusesResponse statusResponse, PetLevelExpResponse expResponse) => {
            SetPetStatuses(statusResponse, expResponse);
            if (resetElapsedTime) _checkStatsElapsedTime = CHECK_STATS_DURATION;
        });
    }

    private void UpdateSleep()
    {
        if (remainSleepTime <= 0)
        {
            remainSleepTime = 0;
            return;
        }

        remainSleepTime--;

        if (remainSleepTime <= 0)
        {
            WebSocketRequestHelper.LoadPetOnce(PlayerData.Instance.PetData.petId, (GamePetData petData) =>
            {
                if (petData.pet_status.IsSleeping)
                {
                    _animController.Sleep();
                }
                else
                {
                    _animController.PetAwake();
                }
                remainSleepTime = petData.pet_status.RemainTime;
            });
        }
    }

    private void UpdateRobot()
    {
        if (PlayerData.Instance.PetData.boost.Count > 0)
        {
            if (_robotTakeActionElapsedTime <= 0)
            {
                _robotTakeActionElapsedTime = 0;
                return;
            }

            --_robotTakeActionElapsedTime;

            if (_robotTakeActionElapsedTime <= 0)
            {
                RobotAutoFeed(DetermineRobotPerformActionTime);
            }
        }
    }

    private void DetermineRobotPerformActionTime()
    {
        _robotTakeActionElapsedTime = PlayerData.Instance.PetData.status.healthValue >= 40 ? TAKE_ACTION_TIME : TAKE_ACTION_WHEN_HAS_BOOST;
    }

    private void CheckPoop()
    {
        if (PlayerData.Instance.data.selectedPetID == -1)
            return;

        if (PlayerData.Instance.PetData.petPhase != PetPhase.Opened)
            return;

        PlayerData.Instance.PetData.poopElapedTime += Time.deltaTime;
        //Debug.Log(PlayerData.Instance.PetData.poopElapedTime + " | " + PlayerData.Instance.PetData.nextPoopTime);

        if (PlayerData.Instance.PetData.poopElapedTime >= PlayerData.Instance.PetData.nextPoopTime)
        {
            if (PlayerData.Instance.PetData.poopCount < 6)
            {
                PlayerData.Instance.PetData.ScheduleNextPoop();

                if (PlayerData.Instance.PetData.boost.Any(item => item.boostId == GameUtils.ROBOT_BOOST))
                {
                    //PlayerData.Instance.PetData.SetPetExp(ExpReceiveType.Clean, GameUtils.EXP_CLEAN_POOP);
                    GameManager.Instance.CleanPoops();
                }
                else
                {
                    PlayerData.Instance.PetData.SetPoopCount(PlayerData.Instance.PetData.poopCount + 1);
                    //UpdateHygieneRate();
                }
            }
        }
    }
    #endregion

    public void EnableCollider()
    {
        m_Collider.enabled = true;
    }

    public void DisableCollider()
    {
        m_Collider.enabled = false;
    }

    #region Archive 
    public void AddHealth(float amount)
    {
        float x = PlayerData.Instance.PetData.status.happy;
        float y = PlayerData.Instance.PetData.status.hygiene;
        float z = PlayerData.Instance.PetData.status.hunger;

        x += 0.3f * amount;
        y += 0.4f * amount;
        z += 0.3f * amount;

        PlayerData.Instance.PetData.status.UpdateHappy(x);
        PlayerData.Instance.PetData.status.UpdateHygiene(y);
        PlayerData.Instance.PetData.status.UpdateHunger(z);
        PlayerData.Instance.PetData.status.CalculateHealth();
    }


    private float GetHygieneRate()
    {
        return 0.001f * PlayerData.Instance.PetData.poopCount;
    }

    #region offline calulation
    private void CalculateOfflineProgress()
    {
        if (PlayerData.Instance.data.selectedPetID == -1)
            return;

        if (PlayerData.Instance.PetData.petPhase != PetPhase.Opened)
            return;

        DateTime lastSavedTime = GameUtils.ParseTime(PlayerData.Instance.PetData.lastSavedTime);
        TimeSpan timeDifference = GameUtils.ParseTime(PlayerData.Instance.PetData.currentTime) - lastSavedTime;

        float secondPassed = (float)timeDifference.TotalSeconds;
        float hunger = PlayerData.Instance.PetData.status.hunger;
        float hygiene = PlayerData.Instance.PetData.status.hygiene;
        float happiness = PlayerData.Instance.PetData.status.happy;

        Debug.Log($"Hunger: {hunger} | hygiene: {hygiene} | happiness: {happiness} | lastSavedTime: {lastSavedTime} | timeDifference: {timeDifference}");

        hunger -= CalculateReduction(hunger, secondPassed);
        hygiene -= CalculateReduction(hygiene, secondPassed);
        happiness -= CalculateReduction(happiness, secondPassed);

        Debug.Log($"secondPassed: {secondPassed} | hunger: {hunger} | hygiene: {hygiene} | happiness: {happiness}");

        PlayerData.Instance.PetData.SetPetStatus(happiness, hygiene, hunger);

        //UpdateStatusCallback();

        // Calculate offline poop.
        float totalElapsedTime = PlayerData.Instance.PetData.poopElapedTime + secondPassed;
        int totalOfflinePoop = Mathf.Clamp(Mathf.FloorToInt(totalElapsedTime / poopRate), 0, 6);
        totalElapsedTime %= poopRate;
        PlayerData.Instance.PetData.poopElapedTime = totalElapsedTime;
        int diff = totalOfflinePoop - PlayerData.Instance.PetData.poopCount;

        for (int i = 0; i < diff; i++)
        {
            GameManager.Instance.ListPoopsController.PetDidAnOopsie();
        }
        if (PlayerData.Instance.PetData.boost.Any(item => item.boostId == GameUtils.ROBOT_BOOST))
        {
            //PlayerData.Instance.PetData.SetPetExp(ExpReceiveType.Clean, GameUtils.EXP_CLEAN_POOP);
            GameManager.Instance.CleanPoops();
        }
        else
        {
            PlayerData.Instance.PetData.SetPoopCount(PlayerData.Instance.PetData.poopCount + diff);
        }
    }

    private float CalculateReduction(float status, float secondPassed)
    {
        return 100f / TIME_2_DAYS * secondPassed;
    }
    #endregion
    #endregion
}

[BurstCompile]
public struct StatusReductionJob : IJob
{
    public NativeArray<float> statuses;
    public NativeArray<StatusType> statusTypes;
    public float deltaTime;
    public float hygieneRate;

    public void Execute()
    {
        for (int i = 0; i < statuses.Length; i++)
        {
            float rate = GetReductionRate(statusTypes[i]);
            statuses[i] -= rate * deltaTime;
            statuses[i] = Mathf.Clamp(statuses[i], 0, 100);
        }
    }

    private float GetReductionRate(StatusType type)
    {
        // max point = 100
        // max time require = 36 hours = 129,600 seconds
        // => return pont / time requre = time reduce per second

        float val = 100f / 129600;
        if (type == StatusType.Hygiene)
            val += hygieneRate;

        return val;
    }
}