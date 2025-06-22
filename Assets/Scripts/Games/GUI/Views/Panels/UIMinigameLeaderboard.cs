using Game;
using Game.Websocket;
using PathologicalGames;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameLeaderboard : BaseView
{
    [SerializeField] private Transform m_LeaderboardTransform;
    [SerializeField] private Transform m_LeaderboardItem;
    [SerializeField] private Transform m_LeaderboardContainer;
    [SerializeField] private LeaderboardItem yourRank;
    [SerializeField] private Button m_BackBtn;

    protected override void OnViewShown()
    {
        OnOpenLeaderboard();
        m_BackBtn.onClick.AddListener(Hide);
    }

    protected override void OnViewHidden()
    {
        m_BackBtn.onClick.RemoveListener(Hide);
        PoolManager.Pools["Leaderboard"].DespawnAll();
    }

    public void OnOpenLeaderboard()
    {
        yourRank.gameObject.SetActive(false);
        ShowUIView<UILoadingView>();

        WebSocketRequestHelper.ShowLeaderboardOnce(GameManager.Instance.MinigameID, 0, (leaderboard) =>
        {
            HideUIView<UILoadingView>();
            ShowLeaderboard(leaderboard);
        });
    }

    private void ShowLeaderboard(LeaderboardResponse leaderboard)
    {
        if (leaderboard == null || leaderboard.leaderboard == null || leaderboard.leaderboard.Count == 0)
        {
            return;
        }

        int count = 0;
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
            ++count;


            if (count >= leaderboard.leaderboard.Count)
            {
                if (leaderboard.current != null)
                {
                    yourRank.gameObject.SetActive(true);
                    yourRank.InitItem(leaderboard.current.position, leaderboard.current.position + ". You", leaderboard.current.score);
                }
                else
                {
                    LoggerUtil.Logging("SHOW_LEARDERBOARD_ERROR", $"Current is null", TextColor.Red);
                }
            }
        }
        LoggerUtil.Logging("SHOW_LEARDERBOARD", $"SpawnCount={count}\nLeaderboardItemCount={leaderboard.leaderboard.Count}");
    }
}
