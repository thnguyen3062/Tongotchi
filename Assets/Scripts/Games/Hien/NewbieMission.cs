using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class NewbieMission : MonoBehaviour
{
    [SerializeField] private Transform[] Steps;
    [SerializeField] private Button m_XSocialBtn;
    [SerializeField] private Button m_YoutubeSocialBtn;
    [SerializeField] private Button m_TelegramSocialBtn;
    [SerializeField] private Button m_XFounderBtn;
    [SerializeField] private Button m_PinTheAppBtn;
    private int socialCount;

#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void OpenTelegramLink(string link);
#endif

    public void InitNewBie()
    {
        if (PlayerData.Instance.data.starterTask.Count >= 3)
            goto step2;

        Steps[0].gameObject.SetActive(true);
        for (int i = 0; i < PlayerData.Instance.data.starterTask.Count; i++)
        {
            Steps[0].GetChild(0).GetChild(PlayerData.Instance.data.starterTask[i]).GetComponent<Button>().interactable = false;
            socialCount++;
        }
        return;

    step2:
        if (PlayerData.Instance.data.starterTask.Count >= 4)
            goto step3;

        Steps[1].gameObject.SetActive(true);
        return;

     step3:
        if (PlayerData.Instance.data.starterTask.Count >= 5)
            goto step4;

        Steps[2].gameObject.SetActive(true);
        return;

    step4:
        CompleteNewbieQuest();
    }

    private void OnDisable()
    {
        foreach (var step in Steps)
        {
            step.gameObject.SetActive(false);
        }
    }

    public void OnClickX()
    {
        PlayerData.Instance.data.starterTask.Add(GameUtils.SOCIAL_X_ID);
        Application.OpenURL(GameUtils.X_LINK);
        m_XSocialBtn.interactable = false;
        CheckSocialCount();
    }
    public void OnClickYoutube()
    {
        PlayerData.Instance.data.starterTask.Add(GameUtils.SOCIAL_YOUTUBE);
        Application.OpenURL(GameUtils.YOUTUBE_LINK);
        m_YoutubeSocialBtn.interactable = false;
        CheckSocialCount();
    }
    public void OnClickTelegram()
    {
        PlayerData.Instance.data.starterTask.Add(GameUtils.SOCIAL_TELEGRAM_ID);
        OpenLink(GameUtils.TELEGRAM_LINK);
        m_TelegramSocialBtn.interactable = false;
        CheckSocialCount();
    }

    private void OpenLink(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR && !UNITY_ANDROID
        // Call the JavaScript function to open the Telegram link
        OpenTelegramLink(url);
#else
        Debug.Log("Open link: " + url);
#endif
    }

    private void CheckSocialCount()
    {
        socialCount++;

        if (socialCount == 3)
        {
            Steps[0].gameObject.SetActive(false);
            InitNewBie();
            GameManager.Instance.ReduceTimeWhenCompleteTask();
        }
    }

    public void OnClickFounderX()
    {
        PlayerData.Instance.data.starterTask.Add(GameUtils.SOCIAL_FOUNDER_X);
        Application.OpenURL(GameUtils.FOUNDER_X_LINK);
        Steps[1].gameObject.SetActive(false);
        m_XFounderBtn.interactable = false;
        InitNewBie();
        GameManager.Instance.ReduceTimeWhenCompleteTask();
    }

    public void OnPinTheApp()
    {
        PlayerData.Instance.data.starterTask.Add(GameUtils.PIN_APP_TELEGRAM);
        Steps[2].gameObject.SetActive(false);
        m_PinTheAppBtn.interactable = false;
        GameManager.Instance.ReduceTimeWhenCompleteTask();
        CompleteNewbieQuest();
    }

    private void CompleteNewbieQuest()
    {
        OnHatch();

        void OnHatch()
        {
            PlayerData.Instance.data.socialQuestCompleted = true;
            gameObject.SetActive(false);
        }
    }
}
