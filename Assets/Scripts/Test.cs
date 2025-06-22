//using Newtonsoft.Json;
//using PlayFab.PfEditor.EditorModels;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace Test
//{
//    public class Test : MonoBehaviour
//    {
//        [SerializeField] private TextAsset m_UserDataText;

//        private List<Wrapper> wrappers = new List<Wrapper>();

//        private const string TITLE_ID = "93AE5";
//        private const string SECRET_KEY = "BEIW6XZE7RWY1WRSSSFJSSGPSBYRARMQPR7N7UGRCP5XHBC9BF";

//        public void Start()
//        {
//            wrappers = JsonConvert.DeserializeObject<List<Wrapper>>(m_UserDataText.text);
//            Debug.Log(wrappers.Count);
//            StartCoroutine(QueryDataRoutine(0, 100));
//        }

//        private IEnumerator QueryDataRoutine(int start, int max)
//        {
//            if (wrappers.Count < max)
//                max = wrappers.Count;

//            for (int i = start; i < max; i++)
//            {
//                string playfabID = wrappers[i].PlayFabId;

//                // Create the URL for the API call
//                string url = $"https://{TITLE_ID}.playfabapi.com/Server/GetUserAccountInfo";
//                Debug.Log(url);
//                // Create the JSON body for the POST request
//                string jsonBody = $"{{\"PlayFabId\": \"{playfabID}\"}}";
//                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

//                // Setup the UnityWebRequest
//                UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
//                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
//                webRequest.downloadHandler = new DownloadHandlerBuffer();
//                webRequest.SetRequestHeader("Content-Type", "application/json");
//                webRequest.SetRequestHeader("X-SecretKey", SECRET_KEY); // Use the server-side secret key

//                // Send the request and wait for a response
//                yield return webRequest.SendWebRequest();

//                // Check for errors
//                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
//                {
//                    Debug.LogError($"Error fetching account info: {webRequest.error}");
//                }
//                else
//                {
//                    // Successfully received response
//                    //Debug.Log($"Response: {webRequest.downloadHandler.text}");
//                    WrapperResponse inf = JsonConvert.DeserializeObject<WrapperResponse>(webRequest.downloadHandler.text);
//                    Debug.Log(inf.data.UserInfo.CustomIdInfo.CustomId);
//                    wrappers[i].telegramCode = inf.data.UserInfo.CustomIdInfo.CustomId;

//                    Debug.Log(wrappers[i].telegramCode);
//                    // Parse response if needed, e.g., using JSON deserialization
//                    // You can use JsonUtility or a third-party library like Newtonsoft.Json
//                }
//            }

//            yield return new WaitForSeconds(1);
//            if (max >= wrappers.Count)
//                SaveJson();
//            else
//                StartCoroutine(QueryDataRoutine(start + 100, max + 100));
//        }

//        private void SaveJson()
//        {
//            string json = JsonConvert.SerializeObject(wrappers, Formatting.None);
//            string path = Path.Combine(Application.dataPath, "UserData.json");
//            File.WriteAllText(path, json);
//            Debug.Log($"Saved data to {path}");
//        }
//    }

//    [Serializable]
//    public class Wrapper
//    {
//        public string telegramCode;
//        public int position;
//        public string DisplayName;
//        public int StatValue;
//        public string PlayFabId;
//    }

//    public class WrapperResponse
//    {
//        public int code;
//        public string status;
//        public WrapperData data;
//    }

//    public class WrapperData
//    {
//        public UserInfo UserInfo;
//    }

//    public class UserInfo
//    {
//        public CustomIdInfo CustomIdInfo;
//    }

//    public class CustomIdInfo
//    {
//        public string CustomId;
//    }
//}