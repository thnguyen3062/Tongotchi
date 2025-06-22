using System.Runtime.InteropServices;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private int gpReward;

    public static AdsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void ShowAds();
#endif
    public void ShowAds(int reward)
    {
        Debug.Log("Show Ads with reward " + reward);
        gpReward = reward;
#if !UNITY_ANDROID
        ShowAds();
#endif
    }

    public void OnShowAdsSuccess()
    {
        PlayerData.Instance.data.gotchipoint += gpReward;
        PlayerData.Instance.data.socialTasks[(int)TaskType.Video].count++;
        UIManager.Instance.SetTotalExpPointsText();
        UIManager.Instance.ShowView<PopupNotify>().Init("CONGRATULATION", $"YOU received {gpReward} GP", 0, false, false);
    }

    public void OnShowAdsFailed()
    {
        Toast.Show("Failed");
    }
}
