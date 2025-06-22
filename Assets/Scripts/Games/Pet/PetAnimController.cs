using Core.Utils;
using DG.Tweening;
using PathologicalGames;
using Spine.Unity;
using System;
using UnityEngine;

public class PetAnimController : MonoBehaviour
{
    public static readonly string BASE_PET_SPINE_SKIN = "Character {0}/Stage {1}/Normal";
    public static readonly string FUSION_PET_SPINE_SKIN = "Fusion character/Character {0}/{1}";
    public static readonly string BASE_EGG_SPINE_SKIN = "egg{0}";

    public static Action OnSleep;
    public static Action OnPetAwake;
    public Action OnEggOpenFinished;

    [SerializeField] private SkeletonAnimation m_SkeletonAnimation;
    [SerializeField] private GameObject m_SleepyEffect;
    [SerializeField] private GameObject m_SleepingEffect;
    [SerializeField] private SkeletonAnimation m_EggSkeletonAnimation;
    [SerializeField] private SkeletonAnimation m_ShowerBathSkeleton;
    [SerializeField] private GameObject m_SparkingGO;
    [SerializeField] private Transform m_FoodAnimGO;

    private PetData pet;
    //private PetPhase petPhase = PetPhase.Hatching;
    private PetState previousState;
    private PetState currentState;
    private PetState previousStatusState = PetState.Normal;

    //public PetPhase PetPhase => petPhase;

    private static string MAIN_GAME_FOLDER = "main_game/";

    private ICallback.CallFunc2<PetPhase> onChangePetPhase;

    //temp
    private int countInteract = 0;

    public PetAnimController SetOnChangePetPhase(ICallback.CallFunc2<PetPhase> func) { onChangePetPhase = func; return this; }

    private PetController controller;

    private void Awake()
    {
        controller = GetComponent<PetController>();
    }

    private void Start()
    {
        controller.onUpdateStatus += OnUpdateStatus;
    }

    private void OnDestroy()
    {
        controller.onUpdateStatus -= OnUpdateStatus;
    }

    public void UnListenEvent()
    {
        controller.onUpdateStatus -= OnUpdateStatus;
    }

    private void OnUpdateStatus(StatusType type, float value)
    {
        if (type != StatusType.Happyness || IsSleeping())
            return;

        PetState state;
        if (value >= 70)
            state = PetState.Happy;
        else if (value >= 30)
            state = PetState.Normal;
        else
            state = PetState.Sad;


        if (state == previousStatusState)
            return;
        previousStatusState = state;

        if (PlayerData.Instance.PetData.isSick)
        {
            ChangePetState(PetState.Sick);
            return;
        }

        ChangePetState(state);
    }

    public void InitPetAnim(int id)
    {
        pet = PetDataSO.Instance.basePetData;
        if (PlayerData.Instance.PetData.petPhase == PetPhase.Hatching)
        {
            m_EggSkeletonAnimation.gameObject.SetActive(true);
            string skinName = string.Format(BASE_EGG_SPINE_SKIN, (id + 1));
            m_EggSkeletonAnimation.initialSkinName = skinName;
            m_EggSkeletonAnimation.Skeleton.SetSkin(skinName);
            onChangePetPhase?.Invoke(PetPhase.Hatching);
            m_SkeletonAnimation.gameObject.SetActive(false);
            m_EggSkeletonAnimation.Initialize(true);
            m_EggSkeletonAnimation.state.SetAnimation(0, "idle", true);
        }
        else
        {
            string skinName = string.Empty;
            int skinId = ClampId(id);
            m_EggSkeletonAnimation.gameObject.SetActive(false);
            m_SkeletonAnimation.gameObject.SetActive(true);
            m_SkeletonAnimation.skeletonDataAsset = pet.skeletonData[skinId];

            if (id < 5)
            {
                skinName = string.Format(BASE_PET_SPINE_SKIN, id + 1, PlayerData.Instance.PetData.petEvolveLevel);
            }
            else
            {
                skinName = string.Format(FUSION_PET_SPINE_SKIN, id + 1, PlayerData.Instance.PetData.PetName);
            }

            LoggerUtil.Logging("INIT_PET_ANIMATION", $"SkinName={skinName}");
            m_SkeletonAnimation.initialSkinName = skinName;
            m_SkeletonAnimation.Initialize(true);
            ChangePetState(PetState.Normal);
            onChangePetPhase?.Invoke(PetPhase.Opened);
        }
    }

    public void PlayEatFoodAnimation(int foodId)
    {
        Transform trans = PoolManager.Pools["FoodAnim"].Spawn(m_FoodAnimGO, new Vector3(0, -0.4f, 0), Quaternion.identity);
        trans.GetComponent<FrameByFrameFoodHandler>().StartAnimation(foodId);
    }

    public bool IsSleeping()
    {
        return currentState == PetState.Sleep;
    }

    public void OnInteracted(ActionType type)
    {
        if (PlayerData.Instance.PetData.petPhase == PetPhase.Opening)
            return;

        switch (type)
        {
            case ActionType.Interact:
                if (PlayerData.Instance.PetData.petPhase == PetPhase.Hatching)
                {
                    m_EggSkeletonAnimation.state.SetAnimation(0, "care", false).Complete += delegate
                    {
                        m_EggSkeletonAnimation.state.SetAnimation(0, "idle", true);
                        countInteract++;
                    };
                    SoundManager.Instance.PlayVFX("03. Egg Interact");
                }
                else
                {
                    ChangePetState(PetState.Excited);
                    SoundManager.Instance.PlayVFX("4. Pet Interact");
                }
                break;
            case ActionType.Feed:
                ChangePetState(PetState.Excited);
                SoundManager.Instance.PlayVFX("6. Eating");
                break;
            case ActionType.Shower:
                OnTakeShower();
                SoundManager.Instance.PlayVFX("8. Shower");
                break;
            case ActionType.Cure:
                ChangePetState(PetState.Excited);
                SoundManager.Instance.PlayVFX("10. Medicine");
                break;
            case ActionType.Toy:
                ChangePetState(PetState.Excited);
                SoundManager.Instance.PlayVFX("4. Pet Interact");
                break;
            case ActionType.Sleep:
                ChangePetState(PetState.Sleep);
                break;
            case ActionType.PlayMinigame:
                ChangePetState(PetState.Excited);
                SoundManager.Instance.PlayVFX("4. Pet Interact");
                break;
            case ActionType.Clean:
                ChangePetState(PetState.Excited);
                SoundManager.Instance.PlayVFX("09. Cleaning");
                break;
        }
    }

    private void OnTakeShower()
    {
        m_ShowerBathSkeleton.gameObject.SetActive(true);
        m_ShowerBathSkeleton.state.SetAnimation(0, "shower", false).Complete += delegate
        {
            ChangePetState(PetState.Excited);
            SoundManager.Instance.PlayVFX("4. Pet Interact");
            m_ShowerBathSkeleton.gameObject.SetActive(false);
        };
    }

    public void Sleep()
    {
        OnSleep?.Invoke();
        ChangePetState(PetState.Sleep);
    }

    public void PetAwake()
    {
        OnPetAwake?.Invoke();
        ChangePetState(PetState.Normal);
    }

    /// <summary>
    /// This only runs when player select pet first.
    /// </summary>
    public void OpenEgg()
    {
        if (!m_EggSkeletonAnimation.gameObject.activeSelf)
            InitPetAnim(PlayerData.Instance.data.selectedPetID);

        PlayerData.Instance.PetData.SetPetPhase(PetPhase.Opening);

        onChangePetPhase?.Invoke(PetPhase.Opening);
        if (m_EggSkeletonAnimation != null)
        {
            m_EggSkeletonAnimation.state.SetAnimation(0, "hatching", false).Complete += delegate
            {
                OnEggOpenFinished?.Invoke();
                onChangePetPhase?.Invoke(PetPhase.Opened);
            };
        }
    }

    public static int ClampId(int id)
    {
        int _id = id;
        if (_id > 5)
        {
            _id = 5;
        }
        return _id;
    }

    private void ChangePetState(PetState state)
    {
        if (currentState == state)
        {
            Debug.Log($"Same state: {currentState.ToString()}");
            return;
        }

        if (m_SleepyEffect.activeSelf)
            m_SleepyEffect.SetActive(false);
        if (m_SleepingEffect.activeSelf)
            m_SleepingEffect.SetActive(false);
        currentState = state;
        LoggerUtil.Logging("CHANGE_PET_STATE", $"NewState={currentState.ToString()}");
        string animName = "";
        bool loop = false;

        switch (state)
        {
            case PetState.Normal:
                animName = MAIN_GAME_FOLDER + "normal";
                loop = true;
                break;
            case PetState.Happy:
                animName = MAIN_GAME_FOLDER + "happy";
                loop = true;
                break;
            case PetState.Hungry:
                animName = MAIN_GAME_FOLDER + "hungry";
                loop = true;
                break;
            case PetState.Sleepy:
                animName = MAIN_GAME_FOLDER + "sleepy";
                m_SleepyEffect.SetActive(true);
                loop = true;
                break;
            case PetState.Bored:
                animName = MAIN_GAME_FOLDER + "bored";
                loop = true;
                break;
            case PetState.Die:
                animName = MAIN_GAME_FOLDER + "die";
                loop = true;
                break;
            case PetState.Sick:
                animName = MAIN_GAME_FOLDER + "sick";
                loop = true;
                break;
            case PetState.Excited:
                animName = MAIN_GAME_FOLDER + "excited";
                loop = false;
                break;
            case PetState.Sleep:
                animName = MAIN_GAME_FOLDER + "sleep";
                m_SleepingEffect.SetActive(true);
                loop = true;
                break;
            case PetState.Sad:
                animName = MAIN_GAME_FOLDER + "sad";
                loop = true;
                break;
        }

        if (state != PetState.Excited)
            previousState = state;

        if (state == PetState.Excited)
            m_SparkingGO.SetActive(true);

        OnChangeAnimation(animName, loop);
    }

    private void OnChangeAnimation(string animName, bool loop)
    {
        if (m_SkeletonAnimation == null)
            return;

        if (animName.Equals(MAIN_GAME_FOLDER + "excited"))
        {
            m_SkeletonAnimation.state.SetAnimation(0, animName, false).Complete += delegate
            {
                ChangePetState(previousState);
            };
        }
        else
        {
            if (m_SkeletonAnimation.state != null)
            {
                m_SkeletonAnimation.state.SetAnimation(0, animName, loop);
            }
        }
    }
}
