using Game;
using Game.Websocket;
using PathologicalGames;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPvp : BaseView
{
    [System.Serializable]
    public struct TeamImgs
    {
        public Sprite flag;
        public Sprite background;
    }

    [SerializeField] private Transform m_LeaderboardFactionParent;
    [SerializeField] private Transform m_LeaderboardSeasonalParent;
    [SerializeField] private Transform m_LeaderboardChild;

    [SerializeField] private Transform m_LeaderboardFaction;
    [SerializeField] private Transform m_LeaderboardSeasonal;
    [SerializeField] private TextMeshProUGUI m_TimeFaction, m_TimeSeasonal;
    [SerializeField] private Image m_TeamImg;
    [SerializeField] private Image m_TeamBG;
    [SerializeField] private TeamImgs[] m_Teams;
    [SerializeField] private Button m_CloseBtn;
    TimeSpan timeSpan;
    private const int LEADERBOARD_FACTION = 0;
    private const int LEADERBOARD_SEASONAL = 1;
    private bool factionShown;
    private bool seasonShown;

    protected override void OnViewShown()
    {
        m_CloseBtn.onClick.AddListener(OnClickClose);
    }

    protected override void OnViewHidden()
    {
        m_CloseBtn.onClick.RemoveListener(OnClickClose);
    }

    // lúc nào bật leaderboard lên thì auto cho type = 0 nhé

    public void OpenLeaderboard(int type)
    {
        InitLeaderboard(type);
    }

    public void InitLeaderboard(int type)
    {
        if (type == LEADERBOARD_FACTION)
        {
            InitLeaderboardFaction();
        }
        else
        {
            InitLeaderboardSeason();
        }
    }

    //Transform[] leaderboards = new Transform[0];
    private void InitLeaderboardFaction()
    {
        //if (leaderboards.Length > 0)
        //{
        //    foreach (var item in leaderboards)
        //    {
        //        PoolManager.Pools["Leaderboard"].Despawn(item);
        //    }
        //    leaderboards = new Transform[0];
        //}

        m_LeaderboardFaction.gameObject.SetActive(true);
        m_LeaderboardSeasonal.gameObject.SetActive(false);
        if (factionShown)
            return;

        //UIManager.instance.LoadingScreen.SetActive(true);
        ShowUIView<UILoadingView>();
        WebSocketRequestHelper.GetPvpTeamLeaderboardOnce((leaderboard) =>
        {
            if (leaderboard == null)
            {
                //UIManager.instance.LoadingScreen.SetActive(false);
                HideUIView<UILoadingView>();
                return;
            }

            //leaderboards = new Transform[leaderboard.current == null ? 10 : 11];
            int count = 0;

            foreach (var item in leaderboard.leaderboard)
            {
                Transform trans = PoolManager.Pools["Leaderboard"].Spawn(m_LeaderboardChild, m_LeaderboardFactionParent);
                trans.GetComponent<LeaderboardItem>().InitItem(item.position, item.displayName, item.match_count);
                //leaderboards[count] = trans;
                count++;
                if (count >= leaderboard.leaderboard.Length)
                {
                    Transform curTrans = PoolManager.Pools["Leaderboard"].Spawn(m_LeaderboardChild, m_LeaderboardFactionParent);
                    //leaderboards[count] = curTrans;
                    curTrans.SetAsLastSibling();
                }
                HideUIView<UILoadingView>();
                //UIManager.instance.LoadingScreen.SetActive(false);
                m_LeaderboardFaction.gameObject.SetActive(true);
                m_LeaderboardSeasonal.gameObject.SetActive(false);

                int team = PlayerData.Instance.PVPProfile.faction.Equals("tongo") ? 0 : 1;
                m_TeamImg.sprite = m_Teams[team].flag;
                m_TeamBG.sprite = m_Teams[team].background;

                TimeSpan timeSpan = PlayerData.Instance.PVPProfile.end_date - PlayerData.Instance.PVPProfile.start_date;
                float timeElapsed = (float)timeSpan.TotalSeconds;
                int hours = (int)timeElapsed / 3600;
                int minutes = ((int)timeElapsed % 3600) / 60;

                if (timeSpan.Days > 0)
                {
                    m_TimeFaction.text = string.Format("{0}d : {1:D2}h", timeSpan.Days, hours);
                }
                else
                {
                    m_TimeFaction.text = string.Format("{0:D2}h : {1:D2}m", hours, minutes);
                }

            }
            factionShown = true;
        });
    }

    private void InitLeaderboardSeason()
    {
        //// code giống trên, nhưng không dùng h mà dùng ranking_point để show điểm
        //if (leaderboards.Length > 0)
        //{
        //    foreach (var item in leaderboards)
        //    {
        //        PoolManager.Pools["Leaderboard"].Despawn(item);
        //    }
        //    leaderboards = new Transform[0];
        //}
        m_LeaderboardFaction.gameObject.SetActive(false);
        m_LeaderboardSeasonal.gameObject.SetActive(true);

        if (seasonShown)
            return;


        WebSocketRequestHelper.GetPvpLeaderboardOnce((leaderboard) =>
        {
            if (leaderboard == null)
            {
                HideUIView<UILoadingView>();
                //UIManager.instance.LoadingScreen.SetActive(false);
                return;
            }

            //leaderboards = new Transform[leaderboard.current == null ? 10 : 11];
            int count = 0;

            foreach (var item in leaderboard.leaderboard)
            {
                Transform trans = PoolManager.Pools["Leaderboard"].Spawn(m_LeaderboardChild, m_LeaderboardSeasonalParent);
                trans.GetComponent<LeaderboardItem>().InitItem(item.position, item.displayName, item.ranking_point);
                //leaderboards[count] = trans;
                count++;
                if (count >= leaderboard.leaderboard.Length)
                {
                    Transform curTrans = PoolManager.Pools["Leaderboard"].Spawn(m_LeaderboardChild, m_LeaderboardSeasonalParent);
                    //leaderboards[count] = curTrans;
                    curTrans.SetAsLastSibling();
                }
                HideUIView<UILoadingView>();
                //UIManager.instance.LoadingScreen.SetActive(false);
                m_LeaderboardFaction.gameObject.SetActive(false);
                m_LeaderboardSeasonal.gameObject.SetActive(true);
                TimeSpan timeSpan = PlayerData.Instance.PVPProfile.end_date - PlayerData.Instance.PVPProfile.start_date;
                float timeElapsed = (float)timeSpan.TotalSeconds;
                int hours = (int)timeElapsed / 3600;
                int minutes = ((int)timeElapsed % 3600) / 60;
                if (timeSpan.Days > 0)
                {
                    m_TimeSeasonal.text = string.Format("{0}d : {1:D2}h", timeSpan.Days, hours);
                }
                else
                {
                    m_TimeSeasonal.text = string.Format("{0:D2}h : {1:D2}m", hours, minutes);
                }
            }
            seasonShown = true;
        });
    }
    private void OnClickClose()
    {
        PoolManager.Pools["Leaderboard"].DespawnAll();
        seasonShown = false;
        factionShown = false;
        this.gameObject.SetActive(false);
    }
}
