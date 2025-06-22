using Core.Utils;
using Game;
using Game.Websocket;
using PathologicalGames;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : BaseView
{
    [SerializeField] private TaskItemHandler[] m_TaskItems;
    [SerializeField] private TextMeshProUGUI m_InviteFriendTextContent;
    [SerializeField] private TextMeshProUGUI m_InviteFriendTextContent2;

    private FriendData[] friends = new FriendData[0];

    private void OnDisable()
    {
        friends = new FriendData[0];
    }

    public void InitTask()
    {
        if (friends.Length == 0)
        {
            WebSocketRequestHelper.GetFriendListOnce((friends) =>
            {
                this.friends = friends;
                InitTask();
            });
        }
        else
        {
            InitTask();
        }

        void InitTask()
        {
            for (int i = 0; i < m_TaskItems.Length; i++)
            {
                OnTaskInit(m_TaskItems[i].TaskType);
            }
        }
    }

    private void OnTaskInit(TaskType type)
    {

        Debug.Log(type.ToString());
        switch (type)
        {
            case TaskType.Video:
                {
                    int reward = 0;
                    int count = PlayerData.Instance.data.socialTasks[(int)TaskType.Video].count;
                    if (count < 10)
                        reward = 20;
                    else if (count < 20)
                        reward = 15;
                    else if (count < 30)
                        reward = 10;
                    else
                        reward = 5;

                    m_TaskItems[(int)TaskType.Video].InitTaskItem($"Watch Short Video\n<size=35>{reward} GP</size>", ButtonClaimSocialState.CanClaim, (task, btnState) =>
                    {
                        OnTaskClick(task, btnState, reward);
                    });
                    break;
                }
            case TaskType.InviteFriend_1:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;
                    if (friends.Length >= 1)
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.InviteFriend_1].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            m_InviteFriendTextContent.text = $"Invite 1 Friend ({friends.Length})\n<size=35>1 Evolve Potion</size>";
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        m_InviteFriendTextContent.text = $"Invite 1 Friend ({friends.Length})\n<size=35>1 Evolve Potion</size>";
                        btnState = ButtonClaimSocialState.Waiting;
                    }
                    m_TaskItems[(int)TaskType.InviteFriend_1].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });
                    break;
                }
            case TaskType.InviteFriend_2:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;
                    if (friends.Length >= 3)
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.InviteFriend_2].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            m_InviteFriendTextContent2.text = $"Invite 3 Friends ({friends.Length})\n<size=35>300 GP</size>";
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        m_InviteFriendTextContent2.text = $"Invite 3 Friends ({friends.Length})\n<size=35>300 GP</size>";
                        btnState = ButtonClaimSocialState.Waiting;
                    }
                    m_TaskItems[(int)TaskType.InviteFriend_2].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });

                    break;
                }
            case TaskType.FollowX:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;
                    if (PlayerData.Instance.data.starterTask.Contains(GameUtils.SOCIAL_X_ID))
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowX].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowX].count == 1)
                        {
                            if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowX].isClaimed)
                            {
                                btnState = ButtonClaimSocialState.Completed;
                            }
                            else
                            {
                                btnState = ButtonClaimSocialState.CanClaim;
                            }
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.Waiting;
                        }
                    }

                    m_TaskItems[(int)TaskType.FollowX].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });
                    break;
                }
            case TaskType.FollowYoutube:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;
                    if (PlayerData.Instance.data.starterTask.Contains(GameUtils.SOCIAL_YOUTUBE))
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowYoutube].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowYoutube].count == 1)
                        {
                            if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowYoutube].isClaimed)
                            {
                                btnState = ButtonClaimSocialState.Completed;
                            }
                            else
                            {
                                btnState = ButtonClaimSocialState.CanClaim;
                            }
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.Waiting;
                        }
                    }

                    m_TaskItems[(int)TaskType.FollowYoutube].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });
                    break;
                }
            case TaskType.FollowInstagram:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;
                    if (PlayerData.Instance.data.starterTask.Contains(GameUtils.SOCIAL_INSTAGRAM))
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowInstagram].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowInstagram].count == 1)
                        {
                            if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowInstagram].isClaimed)
                            {
                                btnState = ButtonClaimSocialState.Completed;
                            }
                            else
                            {
                                btnState = ButtonClaimSocialState.CanClaim;
                            }
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.Waiting;
                        }
                    }

                    m_TaskItems[(int)TaskType.FollowInstagram].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });
                    break;
                }
            case TaskType.FollowCMC:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;

                    if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowCMC].count == 1)
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowCMC].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        btnState = ButtonClaimSocialState.Waiting;
                    }

                    m_TaskItems[(int)TaskType.FollowCMC].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });
                    break;
                }
            case TaskType.FollowChannel:
                {
                    ButtonClaimSocialState btnState = ButtonClaimSocialState.Waiting;
                    if (PlayerData.Instance.data.starterTask.Contains(GameUtils.SOCIAL_TELEGRAM_ID))
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowChannel].isClaimed)
                        {
                            btnState = ButtonClaimSocialState.Completed;
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.CanClaim;
                        }
                    }
                    else
                    {
                        if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowChannel].count == 1)
                        {
                            if (PlayerData.Instance.data.socialTasks[(int)TaskType.FollowChannel].isClaimed)
                            {
                                btnState = ButtonClaimSocialState.Completed;
                            }
                            else
                            {
                                btnState = ButtonClaimSocialState.CanClaim;
                            }
                        }
                        else
                        {
                            btnState = ButtonClaimSocialState.Waiting;
                        }
                    }

                    m_TaskItems[(int)TaskType.FollowChannel].InitTaskItem(string.Empty, btnState, (type, btnState) =>
                    {
                        OnTaskClick(type, btnState);
                    });
                    break;
                }
            case TaskType.BoostChannel:
                {

                    break;
                }
        }
    }

    private void OnTaskClick(TaskType type, ButtonClaimSocialState btnState, int reward = 0)
    {
        if (btnState == ButtonClaimSocialState.CanClaim)
        {
            string content = "";
            switch (type)
            {
                case TaskType.Video:
                    {
                        AdsManager.instance.ShowAds(reward);
                        content = $"{reward} GP";
                        break;
                    }
                case TaskType.InviteFriend_1:
                    {
                        //PlayerData.Instance.OnAddItem(GameUtils.EVOLVE_POTION, 1);
                        content = $"1 Evolve Potion";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.InviteFriend_2:
                    {
                        PlayerData.Instance.data.gotchipoint += 300;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "300 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.FollowX:
                    {
                        PlayerData.Instance.data.gotchipoint += 300;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "300 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.FollowCMC:
                    {
                        PlayerData.Instance.data.gotchipoint += 300;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "300 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.FollowYoutube:
                    {
                        PlayerData.Instance.data.gotchipoint += 50;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "50 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.FollowInstagram:
                    {
                        PlayerData.Instance.data.gotchipoint += 50;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "50 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.FollowChannel:
                    {
                        PlayerData.Instance.data.gotchipoint += 100;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "100 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
                case TaskType.BoostChannel:
                    {
                        PlayerData.Instance.data.gotchipoint += 100;
                        UIManager.Instance.SetTotalExpPointsText();
                        content = "100 GP";
                        PlayerData.Instance.data.socialTasks[(int)type].isClaimed = true;
                        break;
                    }
            }

            if (type != TaskType.Video)
            {
                //UIManager.instance.SpawnNotifyPopup("CONGRATULATION", $"YOU received {content}");
                ShowUIView<PopupNotify>().Init("CONGRATULATION", $"YOU received {content}", 0, false, false);
            }
        }
        else if (btnState == ButtonClaimSocialState.Waiting)
        {
            switch (type)
            {
                case TaskType.FollowX:
                    {
                        Application.OpenURL(GameUtils.X_LINK);
                        PlayerData.Instance.data.socialTasks[(int)type].count = 1;
                        break;
                    }
                case TaskType.FollowYoutube:
                    {
                        Application.OpenURL(GameUtils.YOUTUBE_LINK);
                        PlayerData.Instance.data.socialTasks[(int)type].count = 1;
                        break;
                    }
                case TaskType.FollowCMC:
                    {
                        Application.OpenURL(GameUtils.CMC_LINK);
                        PlayerData.Instance.data.socialTasks[(int)type].count = 1;
                        break;
                    }
                case TaskType.FollowChannel:
                    {
                        OpenLink(GameUtils.TELEGRAM_LINK);
                        PlayerData.Instance.data.socialTasks[(int)type].count = 1;
                        break;
                    }
                case TaskType.FollowInstagram:
                    {
                        Application.OpenURL(GameUtils.INSTAGRAM_LINK);
                        PlayerData.Instance.data.socialTasks[(int)type].count = 1;
                        break;
                    }
                case TaskType.InviteFriend_1:
                    {
                        OnInviteClick();
                        break;
                    }
                case TaskType.InviteFriend_2:
                    {
                        OnInviteClick();
                        break;
                    }
            }
        }
        WebSocketRequestHelper.RequestClaimTask((int)type, () => { });
        InitTask();
        //PlayerData.Instance.SaveData();
    }

#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void ShareInviteLink(string link);
#endif
    private void OnInviteClick()
    {
#if !UNITY_EDITOR && !UNITY_ANDROID
        //Application.OpenURL(PlayerData.Instance.InviteLink);
        ShareInviteLink(PlayerData.Instance.userInfo.link);
#endif
    }
#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void OpenTelegramLink(string link);
#endif
    private void OpenLink(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR && !UNITY_ANDROID
        // Call the JavaScript function to open the Telegram link
        OpenTelegramLink(url);
#else
        Debug.Log("Open link: " + url);
#endif
    }
}