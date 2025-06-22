using Game.Websocket;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardManager : MonoBehaviour
{
    [SerializeField] private GameObject m_DailyRewardPopup;

    [SerializeField] private Image[] m_DailyRewardImage;
    [SerializeField] private TextMeshProUGUI[] m_DayText;
    [SerializeField] private TextMeshProUGUI[] m_CountText;
    [SerializeField] private GameObject[] m_Stamp;
    [SerializeField] private Sprite m_ActiveSprite;
    [SerializeField] private Sprite m_DeactiveSprite;
    [SerializeField] private Sprite m_D7ActiveSprite;
    [SerializeField] private Sprite m_D7DeactiveSprite;

    [SerializeField] private TextMeshProUGUI m_DayCount;
    [SerializeField] private TextMeshProUGUI m_RewardCount;
    [SerializeField] private Button m_CollectReward;
    private int todayReward = -1;

    private UIDailyRewardPopup m_DailyPopup;

    protected UIDailyRewardPopup DailyRewardPopup
    {
        get
        {
            if (m_DailyPopup == null) m_DailyPopup = UIManager.Instance.GetView<UIDailyRewardPopup>();
            return m_DailyPopup;
        }
    }

    public void InitRewardData()
    {
        if (!PlayerData.Instance.data.isTutorialDone)
            return;


        WebSocketRequestHelper.RequestDailyReward(PlayerData.Instance.userInfo.telegramCode, (DailyRewardResponse response) =>
        {
            if (response != null && response.success)
            {
                int index = response.current_day - 1;
                todayReward = DailyRewardSO.Instance.datas[index].count;
                DailyRewardPopup.Show();
                DailyRewardPopup.SetUI(response.current_day, DailyRewardSO.Instance.datas[index].count);
                //m_DayCount.text = $"DAY\n{response.current_day}";
                //m_RewardCount.text = $"<size=60>You got</size>\n{DailyRewardSO.Instance.datas[index].count}<sprite=0>";
                //m_DailyRewardPopup.SetActive(true);
                //PlayerData.Instance.LoadFromCloud(null);
            }
        });

        /*
        WebSocketRequestHelper.GetTimeOnce((time) =>
        {
            bool canCollectReward = CanCollectReward(GameUtils.ParseTime(time), out int totalDayPassed);
            if (canCollectReward)
            {
                int index = totalDayPassed;

                todayReward = DailyRewardSO.Instance.datas[index].count;
                m_DayCount.text = $"DAY\n{totalDayPassed + 1}";
                m_RewardCount.text = $"<size=60>You got</size>\n{DailyRewardSO.Instance.datas[index].count}<sprite=0>";
                m_DailyRewardPopup.SetActive(true);
                StartCoroutine(ActiveButtonRountine());
            }
        });
        */
    }

    #region Goodbye
    public void CollectReward()
    {
        m_DailyRewardPopup.SetActive(false);
        /*
        WebSocketRequestHelper.GetTimeOnce((time) =>
        {
            PlayerData.Instance.data.lastClaimedDailyReward = GameUtils.GetSaveDateString(GameUtils.ParseTime(time));

            if (string.IsNullOrEmpty(PlayerData.Instance.data.firstClaimedDailyReward))
                PlayerData.Instance.data.firstClaimedDailyReward = GameUtils.GetSaveDateString(GameUtils.ParseTime(time));

            PlayerData.Instance.data.dayCollected++;
            Debug.Log("Today reward: " + todayReward);
            m_DailyRewardPopup.SetActive(false);

            PlayerData.Instance.SaveData(() => {
                PlayerData.Instance.AddCurrency(CurrencyType.Ticket, todayReward);
            });
        });
        */
    }

    private bool CanCollectReward(DateTime nowDate, out int totalDaysPassed)
    {
        string lastCollectedDateStr = PlayerData.Instance.data.lastClaimedDailyReward;
        string firstCollectedDateStr = PlayerData.Instance.data.firstClaimedDailyReward;
        int daysPassed;

        if (string.IsNullOrEmpty(firstCollectedDateStr) && string.IsNullOrEmpty(lastCollectedDateStr))
        {
            totalDaysPassed = 0;
            return true;
        }

        DateTime firstCollectedDate = GameUtils.ParseTime(firstCollectedDateStr);
        DateTime lastColelctedDate = GameUtils.ParseTime(lastCollectedDateStr);

        totalDaysPassed = (nowDate.Date - firstCollectedDate.Date).Days;
        daysPassed = (nowDate.Date - lastColelctedDate.Date).Days;


        if (daysPassed == 1 && totalDaysPassed < 7)
        {
            return true;
        }
        else if (daysPassed > 1 || totalDaysPassed >= 7)
        {
            ResetDailyReward();
            totalDaysPassed = 0;
            return true;
        }
        return false;
    }

    private void ResetDailyReward()
    {
        PlayerData.Instance.data.lastClaimedDailyReward = "";
        PlayerData.Instance.data.firstClaimedDailyReward = "";
        PlayerData.Instance.data.dayCollected = -1;
        //PlayerData.Instance.SaveData();
    }
    #endregion
}
