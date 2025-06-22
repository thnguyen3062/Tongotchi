using Game;
using Game.Websocket;
using Game.Websocket.Commands.Game;
using PathologicalGames;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendHandler : BaseView
{
#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void ShareInviteLink(string link);
#endif
    [SerializeField] private TextMeshProUGUI m_RewardText;
    [SerializeField] private Transform m_FriendChild;
    [SerializeField] private Transform m_FriendParent;
    [SerializeField] private Button m_InviteButton;
    [SerializeField] private Button m_ClaimBtn;
    [SerializeField] private Image m_RewardFill;
    [SerializeField] private TextMeshProUGUI m_TotalXpText;
    [SerializeField] private TextMeshProUGUI m_TotalFriendCountText;
    [SerializeField] private TextMeshProUGUI m_ModeButtonText;
    [SerializeField] private Button m_SwitchModeBtn;

    private int currentModeIndex;
    private FriendData[] friends;
    private float totalScore;
    List<FriendChildHandler> listFriendChildHandler = new List<FriendChildHandler>();

    private enum FriendTab
    {
        CurrentXP,
        XPEarned,
        TotalXP
    }
    private FriendTab currentTab;

    protected override void OnViewShown()
    {
        m_InviteButton.onClick.AddListener(OnInviteClick);
        m_ClaimBtn.onClick.AddListener(OnClaim);
        m_SwitchModeBtn.onClick.AddListener(OnSwitchMode);
    }

    protected override void OnViewHidden()
    {
        m_InviteButton.onClick.RemoveListener(OnInviteClick);
        m_ClaimBtn.onClick.RemoveListener(OnClaim);
        m_SwitchModeBtn.onClick.RemoveListener(OnSwitchMode);
    }

    public void Init()
    {
        PoolManager.Pools["Friend"].DespawnAll();
        listFriendChildHandler.Clear();
        currentModeIndex = 0;
        //HttpsConnect.instance.GetFriends(true, OnGetFriendCompleted);
        WebSocketRequestHelper.GetFriendListOnce(OnGetFriendCompleted);
        UpdateText();
    }

    private void OnGetFriendCompleted(FriendData[] friends)
    {
        this.friends = friends;
        totalScore = 0;
        currentTab = FriendTab.CurrentXP;
        m_TotalFriendCountText.text = $"{this.friends.Length} " + (this.friends.Length < 2 ? "Friend" : "Friend");

        for (int i = 0; i < this.friends.Length; i++)
        {
            Transform trans = PoolManager.Pools["Friend"].Spawn(m_FriendChild, m_FriendParent);
            trans.GetComponent<FriendChildHandler>().InitFriendChild(this.friends[i].firstName, this.friends[i].summaryPendingExps);
            totalScore += this.friends[i].summaryPendingExps;
            listFriendChildHandler.Add(trans.GetComponent<FriendChildHandler>());
        }

        m_ClaimBtn.interactable = totalScore > 0;
        UpdateText();
    }

    private void OnChangeTab(FriendTab tab)
    {
        if (tab == FriendTab.XPEarned)
        {
            for (int i = 0; i < listFriendChildHandler.Count; i++)
            {
                listFriendChildHandler[i].InitFriendChild(friends[i].firstName, friends[i].summaryRewardExps);
            }
        }
        else if (tab == FriendTab.CurrentXP)
        {
            for (int i = 0; i < listFriendChildHandler.Count; i++)
            {
                listFriendChildHandler[i].InitFriendChild(friends[i].firstName, friends[i].summaryPendingExps);
            }
        }
        else if (tab == FriendTab.TotalXP)
        {
            for (int i = 0; i < listFriendChildHandler.Count; i++)
            {
                listFriendChildHandler[i].InitFriendChild(friends[i].firstName, friends[i].summaryHarvestExps);
            }
        }
        currentTab = tab;
    }

    private void OnInviteClick()
    {
#if !UNITY_EDITOR && !UNITY_ANDROID
        //Application.OpenURL(PlayerData.Instance.InviteLink);
        ShareInviteLink(PlayerData.Instance.userInfo.link);
#endif
    }

    private void OnClaim()
    {
        WebSocketRequestHelper.ClaimScoreOnce((ClaimScoreResponse response) =>
        {
            //if (a == -1)
            //    return;

            PlayerData.Instance.data.currentClaimedRefferalReward = response.referenceExp;
            PlayerData.Instance.data.claimedLevel = response.referenceLv;

            /*
            if (PlayerData.Instance.data.currentClaimedRefferalReward >= GameUtils.MAX_VALUE_EXP_REFERRAL[PlayerData.Instance.data.claimedLevel])
            {
                PlayerData.Instance.data.currentClaimedRefferalReward -= (int)GameUtils.MAX_VALUE_EXP_REFERRAL[PlayerData.Instance.data.claimedLevel];
                // Change to send increase level request.
                PlayerData.Instance.data.claimedLevel++;
            }
            */

            UpdateText();
            if (friends != null)
            {
                for (int i = 0; i < friends.Length; i++)
                {
                    friends[i].summaryRewardExps += friends[i].summaryPendingExps;
                    friends[i].summaryPendingExps = 0;
                }
            }
            OnChangeTab(currentTab);
            //PlayerData.Instance.SaveData();
        });
    }

    private void UpdateText()
    {
        m_RewardText.text = $"{PlayerData.Instance.data.currentClaimedRefferalReward}/{GameUtils.MAX_VALUE_EXP_REFERRAL[PlayerData.Instance.data.claimedLevel]}";
        m_RewardFill.fillAmount = PlayerData.Instance.data.currentClaimedRefferalReward / GameUtils.MAX_VALUE_EXP_REFERRAL[PlayerData.Instance.data.claimedLevel];
        totalScore = 0;
        m_TotalXpText.text = $"XP: {totalScore}";
    }

    private void OnSwitchMode()
    {
        currentModeIndex++;
        OnChangeTab((FriendTab)(currentModeIndex % 3));
        m_ModeButtonText.text = ((FriendTab)(currentModeIndex % 3)).ToString();
    }
}