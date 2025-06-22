using Core.Utils;
using Game;
using Newtonsoft.Json;
using PathologicalGames;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigame.Scrambler
{
    public class ScramblerManager : BaseMinigameController
    {
        public static ScramblerManager Instance;

        [SerializeField] private GameObject[] m_Character;
        [SerializeField] private TMP_Text scoreText;


        [SerializeField] private GameObject topTrap;
        [SerializeField] private TMP_Text endGameScore;
        public ICallback.CallFunc2<int> onMoveScreen;

        [SerializeField] private GameObject m_CountingBackground;
        [SerializeField] private Image m_CountingImage;
        [SerializeField] private Sprite[] m_CoutingSprite;

        [SerializeField] Transform parentPopup;
        [SerializeField] Transform m_NotifyPopup;

        private Transform character;
        private Transform lastSpawnedPlatform;
        private int m_SideIndex;
        private float gameTime;
        private int score;
        private int count = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            m_SideIndex = 0;
        }

        protected override void OnInitializedComplete()
        {
            base.OnInitializedComplete();
            topTrap.SetActive(false);
        }

        protected override void OnGameStarted(bool success)
        {
            base.OnGameStarted(success);
            if (success)
            {
                m_SideIndex = 0;
                gameTime = 0;
                count = 0;
                ResetDifficulty();
                character = PoolManager.Pools["Platform"].Spawn(m_Character[PlayerData.Instance.PetData.petId].transform, new Vector3(0, 0, 0), Quaternion.identity);
                character.GetComponent<ScramblerCharacterManager>().SetOnGameOver(GameOver).InitCharacter();
                InitPlatform();

                m_CountingBackground.SetActive(true);
                StartCoroutine(StartTimeRoutine());
            }
        }

        private IEnumerator StartTimeRoutine()
        {
            m_CountingImage.sprite = m_CoutingSprite[count];
            yield return new WaitForSeconds(1);

            if (count == 2)
            {
                topTrap.SetActive(true);
                m_CountingBackground.SetActive(false);
                onMoveScreen += character.GetComponent<ScramblerCharacterManager>().OnMoveCharacter;
                GameStarted = true;
            }
            else
            {
                m_SideIndex = 0;
                count++;
                StartCoroutine(StartTimeRoutine());
            }
        }

        #region Core gameplay
        private void InitPlatform()
        {
            if (lastSpawnedPlatform == null)
            {
                // Initial platform spawn
                GeneratePlatform(new Vector3(0, 0.2f), false);
            }

            float distance = Vector2.Distance(lastSpawnedPlatform.position, new Vector2(lastSpawnedPlatform.position.x, -1.8f));

            if (distance >= 0.6f)
            {
                // Spawn new platform if distance criteria is met
                GeneratePlatform(new Vector3(Random.Range(-0.34f, 0.34f), lastSpawnedPlatform.position.y - 0.6f), false);
                InitPlatform();
            }
        }

        private void Update()
        {
            if (!GameStarted)
            {
                m_SideIndex = 0;
                return;
            }

            gameTime += Time.deltaTime;
            score = (int)gameTime;

            OnInputHandle();
            OnSpawnPlatform();

            scoreText.text = score.ToString();

            IncreaseDifficulty();
        }

        private void OnInputHandle()
        {
            // touch screen
            OnMouseTouchInput();

            // keyboard
            OnKeyboardInput();
            if (!GameStarted) return;

            onMoveScreen?.Invoke(m_SideIndex);
        }

        private void OnMouseTouchInput()
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.x < Screen.width / 2)
                    m_SideIndex = -1;
                else if (Input.mousePosition.x > Screen.width / 2)
                    m_SideIndex = 1;

                return;
            }
            m_SideIndex = 0;
        }

        private void OnKeyboardInput()
        {
            if (Input.GetMouseButton(0))
                return;

            if (Input.GetAxisRaw("Horizontal") > 0)
                m_SideIndex = 1;
            else if (Input.GetAxisRaw("Horizontal") < 0)
                m_SideIndex = -1;
            else
                m_SideIndex = 0;
        }

        private void OnSpawnPlatform()
        {
            float distance = Vector2.Distance(lastSpawnedPlatform.position, new Vector2(lastSpawnedPlatform.position.x, -1.8f));
            if (distance < 0.6f)
                return;

            GeneratePlatform(new Vector3(Random.Range(-0.68f, 0.68f), -1.8f));
        }
        #endregion

        public void GameOver()
        {
            SoundManager.Instance.PlayBackgroundMusic("18. Minigame Endgame", false);
            onMoveScreen -= character.GetComponent<ScramblerCharacterManager>().OnMoveCharacter;

            // change state back to end game
            PoolManager.Pools["Enemy"].DespawnAll();
            PoolManager.Pools["Platform"].DespawnAll();
            this.GameStarted = false;

            UIGameOver.SetOnReplayCallback(StartGame).OpenGameOver(score, (int)MinigameType.GotchiDrop, remainTurn, maxTurn, $"You've survived {score} seconds\n\nYou're rewarded with {score}<sprite=0>");
            topTrap.SetActive(false);
            lastSpawnedPlatform = null;

            // set endgame score
            FirebaseAnalytics.instance.LogCustomEvent("user_get_score_minigame", JsonConvert.SerializeObject(new CustomEventWithVariable(score.ToString())));
            scoreText.text = "";

            // upload score and show leaderboard
            PlayerData.Instance.AddCurrency(CurrencyType.Ticket, score);
        }


        #region Generate platform
        [HideInInspector]
        public enum PlatformType
        {
            Normal,
            MovingEnemy,
            Trap,
            Fire
        }

        [HideInInspector]
        public enum Difficulty
        {
            Easy,   // Long platform
            Medium, // Normal platform
            Hard    // Short platform
        }

        public class Platform
        {
            public PlatformType Type { get; set; }
            public Difficulty Level { get; set; }
            public bool HasEnemy { get; set; }
        }

        [Header("Platforms")]
        [SerializeField] private Transform[] m_NormalPlatforms;
        [SerializeField] private Transform[] m_MovingEnemyPlatforms;
        [SerializeField] private Transform[] m_TrapPlatforms;
        [SerializeField] private Transform[] m_FirePlatforms;
        [SerializeField] private Transform enemyPrefab;

        private float timeElapsed = 0f;
        private const float UPDATE_INTERVALS = 15f; // Increase difficulty every 20 seconds

        // Initial probabilities
        private float normalProbability = 1f;
        private float movingEnemyProbability = 0f;
        private float trapProbability = 0f;
        private float fireProbability = 0f;

        // Probability increment values
        private const float INCREASE_MOVING_ENEMY = 0.05f;
        private const float INCREASE_TRAP = 0.025f;
        private const float INCREASE_FIRE = 0.02f;

        // Initial probabilities for difficulty levels
        private float easyProbability = 0.8f;
        private float mediumProbability = 0.15f;
        private float hardProbability = 0.05f;

        // Probability increment values for difficulty levels
        private const float INCREASE_MEDIUM = 0.05f;
        private const float INCREASE_HARD = 0.05f;

        private void ResetDifficulty()
        {
            normalProbability = 1f;
            movingEnemyProbability = 0f;
            trapProbability = 0f;
            fireProbability = 0f;

            easyProbability = 0.8f;
            mediumProbability = 0.15f;
            hardProbability = 0.05f;

            timeElapsed = 0;
        }

        private void IncreaseDifficulty()
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < UPDATE_INTERVALS)
                return;

            timeElapsed = 0;

            // Update platform type probabilities
            if (normalProbability > 0.3f) // Ensuring normal probability doesn't drop too low
            {
                normalProbability -= INCREASE_MOVING_ENEMY + INCREASE_TRAP + INCREASE_FIRE;
            }

            if (score >= 20f)
            {
                if (movingEnemyProbability < 0.5f) // Cap the probability increase
                {
                    movingEnemyProbability += INCREASE_MOVING_ENEMY;
                }
            }

            if (score >= 40f)
            {
                if (trapProbability < 0.3f) // Cap the probability increase
                {
                    trapProbability += INCREASE_TRAP;
                }
            }

            if (score >= 60f)
            {
                if (fireProbability < 0.3f) // Cap the probability increase
                {
                    fireProbability += INCREASE_FIRE;
                }
            }

            // Update difficulty level probabilities
            if (easyProbability > 0.3f) // Ensuring easy probability doesn't drop too low
            {
                easyProbability -= INCREASE_MEDIUM + INCREASE_HARD;
            }

            if (mediumProbability < 0.5f) // Cap the probability increase
            {
                mediumProbability += INCREASE_MEDIUM;
            }

            if (hardProbability < 0.3f) // Cap the probability increase
            {
                hardProbability += INCREASE_HARD;
            }
        }

        private void GeneratePlatform(Vector3 position, bool isInit = false)
        {
            Platform platform = new Platform();

            if (!isInit)
            {
                platform.Type = GetRandomPlatformType();
                platform.Level = GetRandomDifficulty();
                platform.HasEnemy = platform.Type == PlatformType.MovingEnemy;
            }
            else
            {
                platform.Type = PlatformType.Normal;
                platform.Level = Difficulty.Easy;
                platform.HasEnemy = false;
            }

            SpawnPlatform(platform, position);
        }

        private void SpawnPlatform(Platform platform, Vector3 position)
        {
            Transform prefab = null;
            int difficultyIndex = (int)platform.Level;

            switch (platform.Type)
            {
                case PlatformType.Normal:
                    prefab = m_NormalPlatforms[difficultyIndex];
                    break;
                case PlatformType.MovingEnemy:
                    prefab = m_MovingEnemyPlatforms[difficultyIndex];
                    break;
                case PlatformType.Trap:
                    prefab = m_TrapPlatforms[difficultyIndex];
                    break;
                case PlatformType.Fire:
                    prefab = m_FirePlatforms[difficultyIndex];
                    break;
            }

            if (prefab == null)
                return;

            SpawnPlatform(prefab, position, platform.HasEnemy);
        }

        private void SpawnPlatform(Transform prefab, Vector3 position, bool hasEnemy)
        {
            lastSpawnedPlatform = PoolManager.Pools["Platform"].Spawn(prefab, position, Quaternion.identity);

            if (!hasEnemy)
                return;

            lastSpawnedPlatform.GetComponent<ScramblerPlatformManager>().SpawnEnemy(enemyPrefab);
        }

        private PlatformType GetRandomPlatformType()
        {
            float rand = Random.Range(0f, 1f);

            if (rand < normalProbability)
            {
                return PlatformType.Normal;
            }
            else if (rand < normalProbability + movingEnemyProbability)
            {
                return PlatformType.MovingEnemy;
            }
            else if (rand < normalProbability + movingEnemyProbability + trapProbability)
            {
                return PlatformType.Trap;
            }
            else
            {
                return PlatformType.Fire;
            }
        }

        private Difficulty GetRandomDifficulty()
        {
            float rand = Random.Range(0f, 1f);

            if (rand < easyProbability)
            {
                return Difficulty.Easy;
            }
            else if (rand < easyProbability + mediumProbability)
            {
                return Difficulty.Medium;
            }
            else
            {
                return Difficulty.Hard;
            }
        }
        #endregion
    }
}