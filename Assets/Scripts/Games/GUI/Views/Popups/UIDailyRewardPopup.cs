using Game;
using System;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyRewardPopup : BaseView
{
    public Action OnCollectReward;

    [SerializeField] private TextMeshProUGUI m_DayCount;
    [SerializeField] private TextMeshProUGUI m_RewardCount;
    [SerializeField] private Button m_CollectReward;

    protected override void OnViewShown()
    {
        m_CollectReward.onClick.AddListener(Collect);
        m_CollectReward.interactable = false;
    }

    protected override void OnViewHidden()
    {
        m_CollectReward.onClick.RemoveListener(Collect);
    }

    public void SetUI(int dayCount, int rewardCount)
    {
        m_DayCount.text = $"DAY\n{dayCount}";
        m_RewardCount.text = $"<size=60>You got</size>\n{rewardCount}<sprite=0>";
        StartCoroutine(ActiveButtonRountine());
    }

    private void Collect()
    {
        OnCollectReward?.Invoke();
        Hide();
    }

    private IEnumerator ActiveButtonRountine()
    {
        yield return new WaitForSeconds(2);
        m_CollectReward.interactable = true;
    }
}
