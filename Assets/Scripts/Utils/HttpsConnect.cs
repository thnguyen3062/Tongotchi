using Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpsConnect : MonoBehaviour
{
    public static HttpsConnect instance;
    public Transform m_ToastParents;

    private string userData;
    private enum SendType
    {
        POST,
        GET
    }
    private string GetUserData
    {
        get
        {
            if (string.IsNullOrEmpty(userData))
                return userData = JsonConvert.SerializeObject(new UserData(PlayerData.Instance.userInfo.telegramCode));
            return userData;
        }
    }

    private string ErrorLog(string apiName, string error) => $"Call API {apiName} failed with error: {error}";

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

    #region fetching data
    private IEnumerator FetchingPostDataRoutine(string json, string url, bool showLoading, ICallback.CallFunc2<string> onCompleted, ICallback.CallFunc2<string> onFailed, int count = 0)
    {
        if (showLoading)
        {
            //UIManager.instance.LoadingScreen.SetActive(true);
            UIManager.Instance.ShowView<UILoadingView>();
        }

        Debug.Log($"Fetching Data From: \n{url}\nWith Data: \n{json}");
        using UnityWebRequest www = UnityWebRequest.Post(url, json, "application/json-patch+json");

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (count < 3)
            {
                Debug.Log(count);
                Debug.LogError("Fetching Error, re-trying...");
                StartCoroutine(FetchingPostDataRoutine(json, url, showLoading, onCompleted, onFailed, ++count));
            }
            else
            {
                onFailed?.Invoke(www.error);
                if (showLoading)
                {
                    //UIManager.instance.LoadingScreen.gameObject.SetActive(false);
                    UIManager.Instance.HideView<UILoadingView>();
                }
            }
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log($"Response: {jsonResponse}");

            onCompleted?.Invoke(jsonResponse);
        }

        if (showLoading)
        {
            //UIManager.instance.LoadingScreen.SetActive(false);
            UIManager.Instance.HideView<UILoadingView>();
        }
    }

    private IEnumerator FetchingGetDataRoutine(string url, bool showLoading, ICallback.CallFunc2<string> onCompleted, ICallback.CallFunc2<string> onFailed, int count = 0)
    {
        if (showLoading)
        {
            //UIManager.instance.LoadingScreen.gameObject.SetActive(true);
            UIManager.Instance.ShowView<UILoadingView>();
        }

        Debug.Log($"Fetching Data From: \n{url}");
        using UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (count < 3)
            {
                Debug.LogError("Fetching Error, re-trying...");
                StartCoroutine(FetchingGetDataRoutine(url, showLoading, onCompleted, onFailed, count++));
            }
            else
            {
                onFailed?.Invoke(www.error);
            }
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log($"Response: {jsonResponse}");

            onCompleted?.Invoke(jsonResponse);
        }

        if (showLoading)
        {
            //UIManager.instance.LoadingScreen.gameObject.SetActive(false);
            UIManager.Instance.HideView<UILoadingView>();
        }

    }
    #endregion

    #region api calls
    public void GetUser(string query, string command, ICallback.CallFunc onCompleted)
    {
        string json = JsonConvert.SerializeObject(new GetUserData(query, command));
        StartCoroutine(FetchingPostDataRoutine(json, GameUtils.GET_USER_URL, false, (response) =>
        {
            PlayerData.Instance.userInfo = JsonConvert.DeserializeObject<UserInfo>(response);
            onCompleted?.Invoke();
        }, (error) =>
        {
            Debug.LogError(ErrorLog("GetUser", error));
            Toast.Show("Cannot get user info");
        }));
    }

    //public void GetFriends(bool showLoading, ICallback.CallFunc2<FriendData[]> onCompleted)
    //{
    //    StartCoroutine(FetchingPostDataRoutine(GetUserData, GameUtils.GET_FRIEND_URL, showLoading, (response) =>
    //    {
    //        FriendData[] friends = JsonConvert.DeserializeObject<FriendData[]>(response);
    //        onCompleted?.Invoke(friends);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("GetFriend", error));
    //        Toast.Show("Cannot get friends");
    //    }));
    //}

    //public void SetScore(float exp)
    //{
    //    string json = JsonConvert.SerializeObject(new ScoreData(PlayerData.Instance.userInfo.telegramCode, exp));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.SET_SCORE_URL, true, (response) =>
    //    {
    //        Debug.Log($"Set Score successed with score: {exp}");
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("SetScore", error));
    //        Toast.Show("Cannot upload score");
    //    }));
    //}

    //public void ClaimScore(ICallback.CallFunc2<float> onCompletd)
    //{
    //    string json = GetUserData;
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.CLAIM_URL, true, (response) =>
    //    {
    //        ClaimData data = JsonConvert.DeserializeObject<ClaimData>(response);
    //        onCompletd?.Invoke((data != null) ? data.score : -1);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("ClaimScore", error));
    //        Toast.Show("Cannot claim reward");
    //        onCompletd?.Invoke(-1);
    //    }));
    //}

    //public void SaveToCloud(string json, ICallback.CallFunc onCompleted = null)
    //{
    //    string url = $"{GameUtils.SAVE_GAME_URL}/{PlayerData.Instance.userInfo.telegramCode}";
    //    StartCoroutine(FetchingPostDataRoutine(json, url, false, (response) =>
    //    {
    //        onCompleted?.Invoke();
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("SaveToCloud", error));
    //        Toast.Show("Cannot save to cloud");
    //    }));
    //}

    //public void LoadFromCloud(ICallback.CallFunc2<string> onLoadCompleted)
    //{
    //    string url = $"{GameUtils.LOAD_GAME_URL}/{PlayerData.Instance.userInfo.telegramCode}";
    //    StartCoroutine(FetchingPostDataRoutine("", url, false, (response) =>
    //    {
    //        onLoadCompleted?.Invoke(response);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("LoadFromCloud", error));
    //    }));
    //}

    //public void SendReminder(string message, int scheduleTime)
    //{
    //    string json = JsonConvert.SerializeObject(new SendReminderData(PlayerData.Instance.userInfo.telegramCode, message, scheduleTime));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.REMINDER_URL, false, (response) =>
    //    {
    //        ReminderReceivedData data = JsonConvert.DeserializeObject<ReminderReceivedData>(response);
    //        PlayerData.Instance.data.reminderCode = data.code;
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("SendReminder", error));
    //    }));
    //}

    //public void CancelReminder()
    //{
    //    string json = JsonConvert.SerializeObject(new CancelReminderData(PlayerData.Instance.userInfo.telegramCode, PlayerData.Instance.data.reminderCode));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.REMINDER_CANCEL_URL, false, (response) =>
    //    {
    //        Debug.Log("Cancel Successful");
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("CancelReminder", error));
    //    }));
    //}

    //public void CreateInvoiceLink(int amount, ICallback.CallFunc3<string, string> onCompleted)
    //{
    //    string json = JsonConvert.SerializeObject(new InvoiceData(amount));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.CREATE_INVOICE_LINK_URL, true, (response) =>
    //    {
    //        InvoiceResponseData data = JsonConvert.DeserializeObject<InvoiceResponseData>(response);
    //        onCompleted?.Invoke(data.transactionCode, data.link);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("CreateInvoiceLink", error));
    //        Toast.Show("Something went wrong, please try again!");
    //    }));
    //}

    //public void CheckInvoice(string telegramCode, string transactionID, ICallback.CallFunc2<bool> onCompleted)
    //{
    //    string json = JsonConvert.SerializeObject(new TransactionReceived(telegramCode, transactionID));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.CHECK_INVOICE_URL, true, (response) =>
    //    {

    //        CheckInvoiceResponse data = JsonConvert.DeserializeObject<CheckInvoiceResponse>(response);
    //        if (data.status.Equals("PAID"))
    //            onCompleted?.Invoke(true);
    //        else
    //            onCompleted?.Invoke(false);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog("CheckInvoice", error));
    //        onCompleted?.Invoke(false);
    //    }));
    //}

    //public void GetTime(ICallback.CallFunc2<string> onCompleted, bool showLoading = false)
    //{
    //    StartCoroutine(FetchingGetDataRoutine(GameUtils.TIME_URL, showLoading, (response) =>
    //    {
    //        TimeResponse timeResponse = JsonConvert.DeserializeObject<TimeResponse>(response);
    //        if (!string.IsNullOrEmpty(timeResponse.currrent))
    //            onCompleted?.Invoke(timeResponse.currrent);
    //    }, (error) =>
    //    {
    //        Debug.Log(ErrorLog("GetTime", error));
    //    }));
    //}
    //public void GetPet(int id, bool showLoading, ICallback.CallFunc2<GamePetData> onCompleted)
    //{
    //    string json = JsonConvert.SerializeObject(new SendPetData(PlayerData.Instance.userInfo.telegramCode, id));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.LOAD_PET_DATA, showLoading, (response) =>
    //    {
    //        GamePetData data = JsonConvert.DeserializeObject<GamePetData>(response);
    //        onCompleted?.Invoke(data);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog($"GetPet {id} ", error));
    //        Toast.Show("Cannot load pet, please try again!");
    //    }));
    //}

    //public void SavePet(string json, bool showLoading, ICallback.CallFunc2<bool> onCompleted)
    //{
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.SAVE_PET_DATA, showLoading, (response) =>
    //    {
    //        onCompleted?.Invoke(true);
    //    }, (error) =>
    //    {
    //        Debug.LogError(ErrorLog($"Save pet", error));
    //        onCompleted?.Invoke(false);
    //    }));
    //}

    //public void SubmitScoreLeaderboard(int score, ICallback.CallFunc2<LeaderboardResponse> onCompleted)
    //{
    //    string json = JsonConvert.SerializeObject(new SubmitLeaderboardData(PlayerData.Instance.userInfo.telegramCode, score));
    //    StartCoroutine(FetchingPostDataRoutine(json, GameUtils.SUBMIT_LEADERBOARD_URL, false, (ResponseData) =>
    //    {
    //        StartCoroutine(ShowLeaderboard(1.5f, onCompleted));
    //    },
    //    (error) =>
    //    {
    //        Debug.LogError(ErrorLog($"Submit Leaderboard", error));
    //        onCompleted?.Invoke(null);
    //    }));
    //}

    //public IEnumerator ShowLeaderboard(float time, ICallback.CallFunc2<LeaderboardResponse> onCompleted)
    //{
    //    yield return new WaitForSeconds(time);
    //    StartCoroutine(FetchingPostDataRoutine(GetUserData, GameUtils.GET_LEADERBOARD_URL, false, (response) =>
    //    {
    //        LeaderboardResponse leaderboard = JsonConvert.DeserializeObject<LeaderboardResponse>(response);
    //        onCompleted?.Invoke(leaderboard);
    //    },
    //    (error) =>
    //    {
    //        Debug.LogError(ErrorLog($"Get Leaderboard", error));
    //        onCompleted?.Invoke(null);
    //        Toast.Show("Cannot Fetching Leaderboard Data");
    //    }));
    //}
    #endregion
}

#region send/response data
[Serializable]
public class FriendData
{
    public string _id;
    public string code;
    public string firstName;
    public string lastName;
    public string telegramCode;
    public string referrer;
    public float summaryRewardExps;
    public float summaryPendingExps;
    public float summaryHarvestExps;
}

[Serializable]
public class ClaimData
{
    public float score;
}

public class SubmitLeaderboardData
{
    public string telegramCode;
    public int score;
    public string minigameId;

    public SubmitLeaderboardData(string telegramCode, int score, string minigameID)
    {
        this.telegramCode = telegramCode;
        this.score = score;
        this.minigameId = minigameID;
    }
}

public class LeaderboardResponse
{
    public CurrentUserPosition current;
    public List<CurrentLeaderboard> leaderboard;
}

public class CurrentUserPosition
{
    public string telegramCode;
    public string displayName;
    public int score;
    public int position;

}

public class CurrentLeaderboard
{
    public string telegramCode;
    public string displayName;
    public int score;
    public int position;
}

public class InvoiceResponseData
{
    public string transactionCode;
    public string link;
}

public class InvoiceData
{
    public int amount;

    public InvoiceData(int amount)
    {
        this.amount = amount;
    }
}

[Serializable]
public class UserInfo
{
    public string telegramCode;
    public string code;
    public string createdDate;
    public string firstName;
    public string lastModified;
    public string lastName;
    public string link;
    public string referrer;
    public string username;
    public bool isFirstTime;
    public bool maintenance;
    public List<int> pets;
    public string currentTime;
    public string accessToken;
    public bool isCompensation;
}

public class ResponseData
{
    public string code;
    public string message;
}

public class GetUserData
{
    public string query;
    public string command;

    public GetUserData(string query, string command)
    {
        this.query = query;
        this.command = command;
    }
}

public class ScoreData
{
    public string telegramCode;
    public float score;

    public ScoreData(string telegramCode, float score)
    {
        this.telegramCode = telegramCode;
        this.score = score;
    }
}

public class UserData
{
    public string telegramCode;

    public UserData(string telegramCode)
    {
        this.telegramCode = telegramCode;
    }
}

public class SendReminderData
{
    public string telegramCode;
    public string message;
    public int scheduleTime;

    public SendReminderData(string telegramCode, string message, int scheduleTime)
    {
        this.telegramCode = telegramCode;
        this.message = message;
        this.scheduleTime = scheduleTime;
    }
}

public class ReminderReceivedData
{
    public string code;
}

public class CancelReminderData
{
    public string telegramCode;
    public string code;

    public CancelReminderData(string telegramCode, string code)
    {
        this.telegramCode = telegramCode;
        this.code = code;
    }
}

public class TransactionReceived
{
    public string telegramCode;
    public string transactionCode;

    public TransactionReceived(string telegramCode, string transactionCode)
    {
        this.telegramCode = telegramCode;
        this.transactionCode = transactionCode;
    }
}

public class CheckInvoiceResponse
{
    public string status;
}

public class TimeResponse
{
    public string currrent;
}

public struct LoadPetData
{
    public int petId;

    public LoadPetData(int petId)
    {
        this.petId = petId;
    }
}
#endregion