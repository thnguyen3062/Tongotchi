using Game;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameHome : BaseView
{
    public Action onClose;
    private Action onPlayMinigame;

    [SerializeField] private Button m_PlayBtn;
    [SerializeField] private Button m_LeaderboardBtn;
    [SerializeField] private Button m_CloseBtn;

    public UIMinigameHome SetOnPlayMinigameCallback(Action onPlayMinigame)
    {
        this.onPlayMinigame = onPlayMinigame;
        return this;
    }

    protected override void OnViewShown()
    {
        m_PlayBtn.onClick.AddListener(PlayMinigame);
        m_LeaderboardBtn.onClick.AddListener(ShowLeaderboard);
        m_CloseBtn.onClick.AddListener(Back);
    }

    protected override void OnViewHidden()
    {
        m_PlayBtn.onClick.RemoveListener(PlayMinigame);
        m_LeaderboardBtn.onClick.RemoveListener(ShowLeaderboard);
        m_CloseBtn.onClick.RemoveListener(Back);
    }

    private void PlayMinigame()
    {
        onPlayMinigame.Invoke();
    }

    private void ShowLeaderboard()
    {
        ShowUIView<UIMinigameLeaderboard>(ViewOrder.Last);
    }

    private void Back()
    {
        GameManager.Instance.EndMinigame();
        Hide();
    }

    public void SetPlayLabel(string label)
    {
        m_PlayBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = label;
    }
}
