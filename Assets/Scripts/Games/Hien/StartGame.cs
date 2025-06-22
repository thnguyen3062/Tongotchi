using DanielLochner.Assets.SimpleScrollSnap;
using PathologicalGames;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    private bool hackFusionStat = false;

    [SerializeField] private GameObject m_ChooseEggUI;
    [SerializeField] private GameObject m_StartUI;
    [SerializeField] private SimpleScrollSnap m_ScrollSnap;
    [SerializeField] private Image m_Background;
    [SerializeField] private Sprite m_StartPetSprite;

    private const int MAX_EGG = 5;

    public void OnClickStart()
    {
        m_StartUI.SetActive(false);
        m_ChooseEggUI.SetActive(true);
    }

    public void OnConfirm()
    {
        StatusData status = new StatusData(GameUtils.START_HAPPYNESS_VALUE, GameUtils.START_HYGIENEV_VALUE, GameUtils.START_HUNGER_VALUE, GameUtils.MAX_HEALTH_VALUE);

        int level = hackFusionStat ? 45 : 0;
        int evovleLevel = hackFusionStat ? 3 : 0;
        int id = hackFusionStat ? 0 : m_ScrollSnap.SelectedPanel % MAX_EGG;

        GamePetData pet = new GamePetData
        {
            petId = id,
            poopCount = 0,
            petPhase = PetPhase.Hatching,
            currentBackgroundIndex = 0,
            status = status,
            targetTime = "",
            petLevel = level,
            petEvolveLevel = evovleLevel,
            spawnTime = null,//DateTime.Now.ToString(),
        };

        PlayerData.Instance.SavePetDataWithID(pet, (success) =>
        {
            PlayerData.Instance.OnSelectPetOnStart(pet);
            GameManager.Instance.SelectPetOnStart();
            GameManager.Instance.LoadTargetTime();
            PlayerData.Instance.SaveData();
            PlayerData.Instance.userInfo.pets.Add(pet.petId);
            LoggerUtil.Logging("ADD_STARTER_PET", $"PetID: {pet.petId}");
            if (!PlayerData.Instance.data.isTutorialDone)
            {
                GameManager.Instance.OnOpenTutorial(() =>
                {
                    //GameManager.instance.UIManager.OnDoingNewBieMission();

                    PlayerData.Instance.data.gameState = GameState.None;
                });
            }
            else
            {
                PlayerData.Instance.data.gameState = GameState.None;
                //GameManager.instance.UIManager.OnDoingNewBieMission();
            }
            PoolManager.Pools["Popup"].Despawn(transform);
        });
    }
}
