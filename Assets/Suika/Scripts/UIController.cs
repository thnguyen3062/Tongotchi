using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Menu Fields")]
    public Transform startNormalBtn;
    public Transform startSesameBtn;
    [SerializeField] private Button m_ExitBtn;

    [Header("In-game Fields")]
    [SerializeField] private GameObject m_IngameField;
    public Text scoreText;
    public Transform gameOverTransform;
    public Transform pauseTransform;
    public Text gameOverScoreText;
    public Transform pauseBtn;

    private delegate void OnOpenPopup();
    private OnOpenPopup onOpenPopup;
    private delegate void OnClosePopup();
    private OnClosePopup onClosePopup;


    private void Start()
    {
        GameController.onStartGame += StartGame;
        GameController.onScored += GetScore;
        GameController.onGameOver += GameOver;
        GameController.onGamePause += GamePause;
        GameController.onUnpauseGame += UngamePause;
        GameController.onRetry += Retry;
        GameController.onBackToMenu += BackToMenu;
    }

    void StartGame()
    {
        if (startNormalBtn.gameObject.activeSelf)
        {
            startNormalBtn.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.1f).SetUpdate(true).onComplete += delegate
            {
                startNormalBtn.gameObject.SetActive(false);
                m_ExitBtn.gameObject.SetActive(false);
            };
        }
        if (startSesameBtn.gameObject.activeSelf)
        {
            startSesameBtn.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.1f).SetUpdate(true).onComplete += delegate
            {
                startSesameBtn.gameObject.SetActive(false);
                m_ExitBtn.gameObject.SetActive(false);
            };
        }
        m_IngameField.SetActive(true);
        scoreText.gameObject.SetActive(true);
        scoreText.text = "0";
        pauseBtn.gameObject.SetActive(true);
    }

    void GetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void BackToMenu()
    {
        scoreText.gameObject.SetActive(false);
        pauseBtn.gameObject.SetActive(false);
        if (gameOverTransform.gameObject.activeSelf)
        {
            ClosePopup(gameOverTransform, gameOverTransform.GetChild(0), delegate
            {
                startNormalBtn.gameObject.SetActive(true);
                startNormalBtn.DOScale(Vector3.one, 0.1f).SetUpdate(true);

                startSesameBtn.gameObject.SetActive(true);
                startSesameBtn.DOScale(Vector3.one, 0.1f).SetUpdate(true);
            });
        }
        if (pauseTransform.gameObject.activeSelf)
        {
            ClosePopup(pauseTransform, pauseTransform.GetChild(0), delegate
            {
                startNormalBtn.gameObject.SetActive(true);
                startNormalBtn.DOScale(Vector3.one, 0.1f).SetUpdate(true);

                startSesameBtn.gameObject.SetActive(true);
                startSesameBtn.DOScale(Vector3.one, 0.1f).SetUpdate(true);

                m_ExitBtn.gameObject.SetActive(true);
                m_ExitBtn.transform.DOScale(Vector3.one, 0.1f).SetUpdate(true);
            });
        }
    }

    public void Retry()
    {
        //pauseTransform.gameObject.SetActive(false);
        //gameOverTransform.gameObject.SetActive(false);
        if (gameOverTransform.gameObject.activeSelf)
        {
            ClosePopup(gameOverTransform, gameOverTransform.GetChild(0));
        }
        if (pauseTransform.gameObject.activeSelf)
        {
            ClosePopup(pauseTransform, pauseTransform.GetChild(0));
        }
    }

    void GameOver()
    {
        //gameOverTransform.gameObject.SetActive(true);
        //OpenPopup(gameOverTransform, gameOverTransform.GetChild(0));
        //gameOverScoreText.text = scoreText.text;
        pauseBtn.gameObject.SetActive(false);
        m_IngameField.SetActive(false);
    }

    void GamePause()
    {
        //pauseTransform.gameObject.SetActive(true);
        OpenPopup(pauseTransform, pauseTransform.GetChild(0));
    }

    void UngamePause()
    {
        //pauseTransform.gameObject.SetActive(false);
        ClosePopup(pauseTransform, pauseTransform.GetChild(0));
    }

    void OpenPopup(Transform trans, Transform popup, OnOpenPopup openPopupEvent = null)
    {
        trans.gameObject.SetActive(true);
        popup.DOScale(Vector3.one, 0.1f).SetUpdate(true).onComplete += delegate
        {
            openPopupEvent?.Invoke();
        };
    }

    void ClosePopup(Transform trans, Transform popup, OnClosePopup closePopupEvent = null)
    {
        popup.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.1f).SetUpdate(true).onComplete += delegate
        {
            closePopupEvent?.Invoke();
            trans.gameObject.SetActive(false);
        };
    }
}
