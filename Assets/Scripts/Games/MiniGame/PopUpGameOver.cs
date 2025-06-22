using Core.Utils;
using Game;
using Game.Websocket;
using PathologicalGames;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpGameOver : BaseView
{
    private Action onReplayMinigame;

    [SerializeField] private Transform m_LeaderboardItem;
    [SerializeField] private Transform m_LeaderboardParent;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button replayBtn;
    [SerializeField] private LeaderboardItem yourRank;
    //public ScramblerManager scramblerManager;
    [SerializeField] private Transform m_NotifyPopup;

    // Start is called before the first frame update
    private ICallback.CallFunc3<ActionType, ItemData> onSelectAction;

    public PopUpGameOver SetOnReplayCallback(Action onReplayCallback)
    {
        onReplayMinigame = onReplayCallback;
        return this;
    }

    private void OnEnable()
    {
        LoggerUtil.Logging("UI_GAME_OVER_SHOW");
        continueBtn.onClick.AddListener(Continue);
        replayBtn.onClick.AddListener(Replay);
    }

    private void OnDisable()
    {
        continueBtn.onClick.RemoveListener(Continue);
        replayBtn.onClick.RemoveListener(Replay);
        onReplayMinigame = null;
    }

    public void OpenGameOver(int score, int minigameId, int remainTurn, int maxTurn, string msg)
    {
        scoreText.text = score.ToString();
        SetRemainTicket(remainTurn, maxTurn);
        Show();
        ShowUIView<PopupNotify>().Init("CONGRATULATION", $"<size=40>{msg}</size>", 0, false, false);
        yourRank.gameObject.SetActive(false);
        UploadAndShowLeaderboard(score, minigameId);
    }

    public void SetRemainTicket(int remainTicket, int max)
    {
        replayBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Replay\n({remainTicket}/{max})";
    }

    public void Continue()
    {
        Hide();
        GameManager.Instance.EndMinigame();
    }

    private void UploadAndShowLeaderboard(int score, int minigameID)
    {
        if (PoolManager.Pools["Leaderboard"].Count > 0)
            PoolManager.Pools["Leaderboard"].DespawnAll();
        UIManager.Instance.ShowView<UILoadingView>();
        string id = $"G0{minigameID + 1}";
        WebSocketRequestHelper.SubmitScoreLeaderboardOnce(score, id, (bool status) =>
        {
            if (status)
            {
                WebSocketRequestHelper.ShowLeaderboardOnce(GameManager.Instance.MinigameID, 0, (latestLeaderboard) =>
                {
                    SubmitScoreLeaderboard(latestLeaderboard);
                });
            }
        });
    }

    private void SubmitScoreLeaderboard(LeaderboardResponse leaderboard)
    {
        if (leaderboard == null || leaderboard.leaderboard == null || leaderboard.leaderboard.Count == 0)
        {
            UIManager.Instance.HideView<UILoadingView>();
            return;
        }

        foreach (var item in leaderboard.leaderboard)
        {
            Transform trans = PoolManager.Pools["Leaderboard"].Spawn(m_LeaderboardItem, m_LeaderboardParent);
            string username = item.displayName;
            if (leaderboard.current != null && item.telegramCode == leaderboard.current.telegramCode)
            {
                username = "You";
            }
            trans.GetComponent<LeaderboardItem>().InitItem(item.position, username, item.score);
        }

        if (leaderboard.current != null)
        {
            yourRank.gameObject.SetActive(true);
            yourRank.InitItem(leaderboard.current.position, "You", leaderboard.current.score);
        }

        UIManager.Instance.HideView<UILoadingView>();
    }


    public void Replay()
    {
        SoundManager.Instance.PlayBackgroundMusic("17.MinigameBG");
        onReplayMinigame?.Invoke();
    }

    public bool CheckTicket()
    {
        if (PlayerData.Instance.data.turnPlayMinigameRemain > 0)
            return true;

        Transform trans = PoolManager.Pools["Popup"].Spawn(m_NotifyPopup, transform);
        trans.GetComponent<PopupNotify>().SetOnConfirmBuyMinigame((success, remain, max) =>
        {
            if (success)
            {
                replayBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Replay\n({remain}/{max})";
                //PoolManager.Pools["Popup"].Despawn(trans);
            }
        }).Init(
            "OUT OF MINIGAME TICKETS",
            "You've out of minigame tickets!\nYour tickets will re-fill tomorrow!\nOr buy 5 tickets with only 1<sprite=2>",
            1, false
            );

        return false;
    }
}
