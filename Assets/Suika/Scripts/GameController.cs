using PathologicalGames;
using UnityEngine;
using DG.Tweening;
using Game.Websocket.Commands.Storage;
using Game.Websocket;
using Game;

public enum GameMode
{
    Normal,
    Sesame
}

public enum SuikaGameState
{
    GameOver,
    Play,
    Pause,
}

public class GameController : BaseMinigameController
{
    public static readonly Vector3 BALL_STARTER_SCALE = new Vector3(0.2f, 0.2f, 0.2f);
    public static readonly Vector3 BALL_BASE_SCALE = new Vector3(0.35f, 0.35f, 0.35f);

    public static GameController Instance;

    public GameObject ballPrefab;
    public Vector2 spawnPosition = new Vector2(0, 1.5f);
    public Transform ballContainer;
    public Transform despawnContainer;

    public delegate void OnUpdateIndex();
    public static event OnUpdateIndex onUpdateIndex;
    public delegate void OnStartGame();
    public static event OnStartGame onStartGame;
    public delegate void OnScored(int score);
    public static event OnScored onScored;
    public delegate void OnGameOver();
    public static event OnGameOver onGameOver;
    public delegate void OnGamePause();
    public static event OnGamePause onGamePause;
    public delegate void OnUnpauseGame();
    public static event OnUnpauseGame onUnpauseGame;
    public delegate void OnRetry();
    public static event OnRetry onRetry;
    public delegate void OnBackToMenu();
    public static event OnBackToMenu onBackToMenu;

    Transform spawnedBall;
    float timeToSpawn = 0;

    public Sprite[] ballSprite;
    public GameMode gameMode;
    public SuikaGameState gameState;
    public int gameScore;
    public GameObject warningLineAnim;

    Vector2 screenBounce;

    public float colliderHorizontalOffset = 0.15f;
    public Transform leftCollider;
    public Transform rightCollider;
    public Transform groundTransform;
    public Transform deadTriggerTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        Camera cam = Camera.main;

        // Get screen bounds in world units using orthographic camera
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        screenBounce = topRight;

        // Position side colliders based on horizontal limits
        leftCollider.transform.position = new Vector2(bottomLeft.x - colliderHorizontalOffset, leftCollider.transform.position.y);
        rightCollider.transform.position = new Vector2(topRight.x + colliderHorizontalOffset, rightCollider.transform.position.y);

        // Align ground to bottom edge, add custom offset for short screens
        float groundOffset = (Screen.height < 1280) ? (groundTransform.GetComponent<BoxCollider2D>().size.y / cam.orthographicSize) : 0;
        groundTransform.position = new Vector2(
            groundTransform.position.x,
            bottomLeft.y
        );

        // Calculate vertical offset for spawn & death trigger area
        float verticalOffset = (1 / cam.orthographicSize);

        // Position death trigger
        deadTriggerTransform.position = new Vector2(
            deadTriggerTransform.position.x,
            topRight.y - verticalOffset
        );

        // Set spawn position Y
        spawnPosition.y = topRight.y - verticalOffset;
    }

    protected override void OnInitializedComplete()
    {
        base.OnInitializedComplete();
        gameState = SuikaGameState.GameOver;
    }

    void Update()
    {
        if (gameState != SuikaGameState.Play) return;

        if (spawnedBall != null)
        {
            if (spawnedBall.GetComponent<BallController>().state != BallState.Waiting)
            {
                timeToSpawn += Time.deltaTime;
                if (timeToSpawn >= 0.7f)
                {
                    SpawnBall(gameMode == GameMode.Normal ? Random.Range(0, 5) : Random.Range(6, 10));
                    timeToSpawn = 0;
                }
            }
        }
    }

    public void SpawnBall(int id)
    {
        Transform ball = PoolManager.Pools["Ball"].Spawn(ballPrefab, spawnPosition, Quaternion.identity);
        ball.GetComponent<BallController>().Init(id);
        ball.SetParent(ballContainer.GetChild(ball.GetComponent<BallController>().id));
        spawnedBall = ball;
        spawnedBall.localScale = BALL_STARTER_SCALE;
        spawnedBall.transform.DOScale(BALL_BASE_SCALE, 0.1f);
        UpdateIndex();
    }

    public void UpdateIndex()
    {
        onUpdateIndex?.Invoke();
    }

    protected override void OnGameStarted(bool success)
    {
        base.OnGameStarted(success);
        if (success)
        {
            gameMode = GameMode.Normal;
            groundTransform.gameObject.SetActive(true);
            SpawnBall(0);
            Time.timeScale = 1;
            timeToSpawn = 0;
            onStartGame?.Invoke();
            gameState = SuikaGameState.Play;
        }
    }

    public void StartSesame()
    {
        gameMode = GameMode.Sesame;
        groundTransform.gameObject.SetActive(true);
        SpawnBall(9);
        Time.timeScale = 1;
        timeToSpawn = 0;
        onStartGame?.Invoke();
        gameState = SuikaGameState.Play;
    }

    public void GetScore(int score)
    {
        gameScore += score;
        onScored?.Invoke(gameScore);
    }

    public void GameOver()
    {
        SoundManager.Instance.PlayBackgroundMusic("18. Minigame Endgame", false);

        onGameOver?.Invoke();
        Time.timeScale = 0;
        groundTransform.gameObject.SetActive(false);
        PoolManager.Pools["Ball"].DespawnAll();
        gameState = SuikaGameState.GameOver;

        UIGameOver.SetOnReplayCallback(StartGame).OpenGameOver(gameScore, (int)MinigameType.Suika, remainTurn, maxTurn, $"You've scored {gameScore} points");
    }

    #region Button Actions
    public void PauseGame()
    {
        onGamePause?.Invoke();
        Time.timeScale = 0;
        gameState = SuikaGameState.Pause;
    }

    public void UnPauseGame()
    {
        onUnpauseGame?.Invoke();
        Time.timeScale = 1;
        gameState = SuikaGameState.Play;
    }

    public void RetryGame()
    {
        onRetry?.Invoke();
        groundTransform.gameObject.SetActive(false);
        PoolManager.Pools["Ball"].DespawnAll();
        if (gameMode == GameMode.Normal)
        {
            StartGame();
        }
        else
        {
            StartSesame();
        }
    }

    public void BackToMenu()
    {
        groundTransform.gameObject.SetActive(false);
        PoolManager.Pools["Ball"].DespawnAll();
        onBackToMenu?.Invoke();
        gameState = SuikaGameState.GameOver;
    }

    public void ExitSuika()
    {
        groundTransform.gameObject.SetActive(false);
        gameState = SuikaGameState.GameOver;
        Time.timeScale = 1;
        PoolManager.Pools["Minigame"].Despawn(this.transform);
        if (PoolManager.Pools["Ball"].Count > 0 ) PoolManager.Pools["Ball"].DespawnAll();
    }
    #endregion
}
