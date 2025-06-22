using Core.Utils;
using Game.Websocket.Interface;
using Game.Websocket.Model;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Websocket
{
    public class WebSocketConnector : MonoBehaviour
    {
        public static WebSocketConnector Instance;
        private WebSocket _websocket;
        private WebSocketMessageDispatcher _dispatcher;
        private bool showLoading;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _dispatcher = new WebSocketMessageDispatcher();
        }

        public async void ConnectSocket(string accessToken, ICallback.CallFunc2<bool> onLoadCompleted)
        {
            _websocket = new WebSocket("wss://9jw2jtdfbk.execute-api.ap-southeast-1.amazonaws.com/production", accessToken);

            _websocket.OnOpen += () =>
            {
                OnWebsocketOpened(onLoadCompleted);
            };

            _websocket.OnError += (e) =>
            {
                OnWebsocketError(e, onLoadCompleted);
            };

            _websocket.OnClose += (e) =>
            {
                OnWebSocketClosed(e);
            };

            _websocket.OnMessage += (bytes) =>
            {
                OnWebSocketReceivedMessage(bytes);
            };

            // Keep sending messages at every 0.3s
            InvokeRepeating(nameof(Ping), 0.0f, 30);

            // waiting for messages
            await _websocket.Connect();
        }

        private void OnWebsocketOpened(ICallback.CallFunc2<bool> onLoadCompleted)
        {
            Debug.Log("Connection open!");
            onLoadCompleted?.Invoke(true);
        }

        private void OnWebsocketError(string error, ICallback.CallFunc2<bool> onLoadCompleted)
        {
            Debug.LogError("Error! " + error);
            onLoadCompleted?.Invoke(false);
        }

        private void OnWebSocketClosed(WebSocketCloseCode e)
        {
            Debug.Log("Connection closed! " + e.ToString());
        }

        private void OnWebSocketReceivedMessage(byte[] bytes)
        {
            // getting the message as a string
            var message = Encoding.UTF8.GetString(bytes);
            var response = JsonConvert.DeserializeObject<SocketResponse>(message);
            Debug.Log($"<color=green>----------response----------</color>\n" + message);
            _dispatcher.DispatchMessage(response);

            if (showLoading)
            {
                //UIManager.instance.LoadingScreen.SetActive(false);
                UIManager.Instance.HideView<UILoadingView>();
            }
        }

        #region ping
        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (_websocket == null)
                return;

            _websocket.DispatchMessageQueue();
#endif
        }

        private async void Ping()
        {
            if (_websocket.State == WebSocketState.Open)
            {
                // Sending plain text
                await _websocket.SendText("{}");
            }
        }
        #endregion

        public async Task SendCommand(IWebSocketCommand command)
        {
            if (_websocket == null || _websocket.State != WebSocketState.Open)
                return;

            string json = command.ToJson();
            Debug.Log($"Send: {json}");
            await _websocket.SendText(json);
        }

        private async void OnApplicationQuit()
        {
            await _websocket.Close();
        }

        #region DEO DUNG NUA VA SE XOA SAU KHI TEST XONG
        /*
        /// <summary>
        /// ---------------------------------------------------------------------------
        /// </summary>
        /// <param name="message"></param>
        private void ReceivedData(string message)
        {
            SocketResponse response = JsonConvert.DeserializeObject<SocketResponse>(message);

            switch (response.group)
            {
                case "GAME":
                    ParseGameCommand(response.command, response.content, response.actionType);
                    break;
                case "STORAGE":
                    ParseStorageCommand(response.command, response.content);
                    break;
                case "PET":
                    ParsePetCommand(response.command, response.content);
                    break;
                case "MINIGAME":
                    ParseMinigameCommand(response.command, response.content);
                    break;
                case "INVOICE":
                    ParseInvoiceCommand(response.command, response.content);
                    break;
                case "REMINDER":
                    ParseReminderCommand(response.command, response.content);
                    break;
                case "SHOP":
                    ParseShopCommand(response.command, response.content);
                    break;
                case "ITEM":
                    ParseInventoryCommand(response.command, response.content);
                    break;
                case "PVP":
                    ParsePvpCommand(response.command, response.content);
                    break;
            }
        }
        
        #region received response

        #region storage
        private ICallback.CallFunc2<string> onLoadGameCompleted;
        private ICallback.CallFunc onSaveGameCompleted;
        private void ParseStorageCommand(string command, string content)
        {
            switch (command)
            {
                case "LOAD":
                    onLoadGameCompleted?.Invoke(content);
                    break;
                case "SAVE":
                    onSaveGameCompleted?.Invoke();
                    break;
            }
        }
        #endregion

        #region pet
        private ICallback.CallFunc2<GamePetData> onLoadPetCompleted;
        private ICallback.CallFunc2<bool> onSavePetCompleted;
        private ICallback.CallFunc2<GamePetData[]> onLoadAllPetCompleted;
        private void ParsePetCommand(string command, string content)
        {
            switch (command)
            {
                case "LOAD":
                    GamePetData data = JsonConvert.DeserializeObject<GamePetData>(content);
                    onLoadPetCompleted?.Invoke(data);
                    break;
                case "SAVE":
                    onSavePetCompleted?.Invoke(true);
                    break;
                case "DEFENSE":

                    break;
                case "EQUIPE_ITEM":

                    break;
                case "QUERY":
                    GamePetData[] datas = JsonConvert.DeserializeObject<GamePetData[]>(content);
                    onLoadAllPetCompleted?.Invoke(datas);
                    break;
            }
        }
        #endregion

        #region game
        ICallback.CallFunc2<string> onLoadTimeCompleted;

        private Dictionary<string, ICallback.CallFunc2<string>> concurrentDictionary = new Dictionary<string, ICallback.CallFunc2<string>>();

        //Dictionary<string, ICallback.CallFunc2<string>> onLoadTimeCallbacks = new Dictionary<string, ICallback.CallFunc2<string>>();
        ICallback.CallFunc2<float> onClaimScoreCompleted;
        private ICallback.CallFunc2<FriendData[]> onGetFriendCompleted;
        private void ParseGameCommand(string command, string content, string actionType = "")
        {
            switch (command)
            {
                case "TIME":
                    TimeResponse response = JsonConvert.DeserializeObject<TimeResponse>(content);
                    if (!string.IsNullOrEmpty(response.currrent) && !string.IsNullOrEmpty(actionType))
                    {
                        ICallback.CallFunc2<string> action = concurrentDictionary.FirstOrDefault(data => data.Key.Equals(actionType)).Value;
                        if (action != null)
                        {
                            action?.Invoke(response.currrent);
                        }
                    }
                    break;
                case "SUBMIT":

                    break;
                case "CLAIM":
                    ClaimData data = JsonConvert.DeserializeObject<ClaimData>(content);
                    onClaimScoreCompleted?.Invoke((data != null) ? data.score : -1);
                    break;
                case "FRIENDS":
                    FriendData[] friends = JsonConvert.DeserializeObject<FriendData[]>(content);
                    onGetFriendCompleted?.Invoke(friends);
                    break;
            }
        }
        #endregion

        #region minigame
        private ICallback.CallFunc2<LeaderboardResponse> onSubmitLeaderboardResponse;
        private ICallback.CallFunc2<LeaderboardResponse> onGetLeaderboardResponse;
        private void ParseMinigameCommand(string command, string content)
        {
            switch (command)
            {
                case "SUBMIT":
                    //StartCoroutine(ShowLeaderboardRoutine(1.5f, onSubmitLeaderboardResponse));
                    break;
                case "LEADERBOARD":
                    LeaderboardResponse response = JsonConvert.DeserializeObject<LeaderboardResponse>(content);
                    onGetLeaderboardResponse?.Invoke(response);
                    break;
            }
        }

        //private IEnumerator ShowLeaderboardRoutine(float time, ICallback.CallFunc2<LeaderboardResponse> onCompleted)
        //{
        //    yield return new WaitForSeconds(1.5f);
        //    ShowLeaderboard(time, onCompleted);
        //}
        #endregion

        #region invoice
        private ICallback.CallFunc3<string, string> onCreateInvoiceCompleted;
        private ICallback.CallFunc2<bool> onCheckStatusCompleted;
        private void ParseInvoiceCommand(string command, string content)
        {
            switch (command)
            {
                case "LINK":
                    {
                        InvoiceResponseData data = JsonConvert.DeserializeObject<InvoiceResponseData>(content);
                        onCreateInvoiceCompleted?.Invoke(data.transactionCode, data.link);
                        break;
                    }
                case "STATUS":
                    {
                        CheckInvoiceResponse data = JsonConvert.DeserializeObject<CheckInvoiceResponse>(content);
                        if (data.status.Equals("PAID"))
                            onCheckStatusCompleted?.Invoke(true);
                        else
                            onCheckStatusCompleted?.Invoke(false);
                        break;
                    }
            }
        }
        #endregion

        #region reminder
        private void ParseReminderCommand(string command, string content)
        {
            switch (command)
            {
                case "SCHEDULE":
                    ReminderReceivedData data = JsonConvert.DeserializeObject<ReminderReceivedData>(content);
                    PlayerData.Instance.data.reminderCode = data.code;
                    break;
                case "CANCEL":

                    break;
            }
        }
        #endregion

        

        #region pvp shop
        private ICallback.CallFunc2<PvpShopQueryResponse> onPvpShopQueryCallback;
        private ICallback.CallFunc2<PvpShopQueryResponse> onPvpShopRefreshCallback;
        private ICallback.CallFunc2<PvpShopQueryResponse> onPvpShopBuyCallback;
        private void ParseShopCommand(string command, string content)
        {
            switch (command)
            {
                case "QUERY":
                    PvpShopQueryResponse data = JsonConvert.DeserializeObject<PvpShopQueryResponse>(content);
                    onPvpShopQueryCallback?.Invoke(data);
                    break;
                case "BUY":
                    PvpShopQueryResponse data2 = JsonConvert.DeserializeObject<PvpShopQueryResponse>(content);
                    onPvpShopBuyCallback?.Invoke(data2);
                    break;
                case "REFRESH":
                    PvpShopQueryResponse data1 = JsonConvert.DeserializeObject<PvpShopQueryResponse>(content);
                    onPvpShopRefreshCallback?.Invoke(data1);
                    break;
            }
        }
        #endregion

        #region pvp inventory
        private ICallback.CallFunc3<InventoryPvpItemData[], UpgradePvpItemReseponse> onPvpInventoryQueryCallback;
        private ICallback.CallFunc3<InventoryPvpItemData[], UpgradePvpItemReseponse> onPvpInventoryUpgradeCallback;
        private UpgradePvpItemReseponse upgradedPvpItem;
        private void ParseInventoryCommand(string command, string content)
        {
            switch (command)
            {
                case "QUERY":
                    InventoryPvpItemData[] data = JsonConvert.DeserializeObject<InventoryPvpItemData[]>(content);
                    if (upgradedPvpItem == null)
                        onPvpInventoryQueryCallback?.Invoke(data, new UpgradePvpItemReseponse());
                    else
                        onPvpInventoryQueryCallback?.Invoke(data, upgradedPvpItem);
                    break;
                case "UPGRADE":
                    UpgradePvpItemReseponse response = JsonConvert.DeserializeObject<UpgradePvpItemReseponse>(content);
                    upgradedPvpItem = response;
                    //QueryPvpInventory(onPvpInventoryUpgradeCallback);
                    break;
            }
        }
        #endregion

        #region pvp
        private ICallback.CallFunc2<PVPProfileResponse> onGetProfileCompleted;
        private ICallback.CallFunc2<PVPProfileResponse> onSelectFactionCompleted;
        private ICallback.CallFunc2<PvpLeaderboardResponse> onGetLeaderboardCompleted;
        private ICallback.CallFunc2<PvpLeaderboardResponse> onGetTeamLeaderboardCompleted;
        private ICallback.CallFunc2<PvpCombat> onCombatCompleted;
        private ICallback.CallFunc2<bool> onResetAttackCountCompleted;

        public void ParsePvpCommand(string command, string content)
        {
            switch (command)
            {
                case "PROFILE":
                    {
                        PVPProfileResponse response = JsonConvert.DeserializeObject<PVPProfileResponse>(content);

                        //// chưa vào faction nào, bật popup chọn faction
                        //// chỗ này test thôi, không cần quan tâm, sẽ xoá sau
                        //if (!string.IsNullOrEmpty(response.error_code))
                        //{
                        //    if (response.error_code.Equals("912"))
                        //    {
                        //        Debug.Log("Not joined any faction. Select a faction");

                        //        // just for testing purpose
                        //        SelectFaction("Tongo", (s) =>
                        //        {

                        //        });
                        //    }
                        //}
                        //// hết chỗ test

                        // chỗ này Hiền check 
                        // if (!string.IsNullOrEmpty(response.error_code))
                        // rồi check
                        // if (response.error_code.Equals("912"))
                        // nếu equals thì bật popup select Faction
                        // nếu không thì chạy cái khác
                        // chỗ này chỉ để check đã join faction nào hay chưa, nếu chưa thì phải chọn, nếu rồi thì parse dữ liệu của faction thôi
                        onGetProfileCompleted?.Invoke(response);
                        break;
                    }
                case "FACTION":
                    {
                        PVPProfileResponse response = JsonConvert.DeserializeObject<PVPProfileResponse>(content);
                        onSelectFactionCompleted?.Invoke(response);
                        break;
                    }
                case "LEADERBOARD":
                    {
                        PvpLeaderboardResponse response = JsonConvert.DeserializeObject<PvpLeaderboardResponse>(content);
                        onGetLeaderboardCompleted?.Invoke(response);
                        break;
                    }
                case "TEAM_LEADERBOARD":
                    {
                        PvpLeaderboardResponse reponse = JsonConvert.DeserializeObject<PvpLeaderboardResponse>(content);
                        onGetTeamLeaderboardCompleted?.Invoke(reponse);
                        break;
                    }
                case "COMBAT":
                    {
                        PvpCombat data = JsonConvert.DeserializeObject<PvpCombat>(content);
                        onCombatCompleted?.Invoke(data);
                        break;
                    }
                case "RESET":
                    {
                        PVPProfileResponse response = JsonConvert.DeserializeObject<PVPProfileResponse>(content);
                        PlayerData.Instance.SetPvpProfile(response);
                        onResetAttackCountCompleted?.Invoke(true);
                        break;
                    }
            }
        }
        #endregion
        #endregion
        */

        /*
        private async void SendWebSocket(string json)
        {
            //if (showLoading)
            //    UIManager.instance.LoadingScreen.SetActive(true);
            if (_websocket.State == WebSocketState.Open)
            {
                Debug.Log("------------------------Request---------------------------- ");
                Debug.Log(json);
                await _websocket.SendText(json);
            }
        }

        public void CollectCompensation()
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("COMPENSATION", "RECEIVE", new UserData(null)));
            SendWebSocket(json);
        }
        */
        #region send request
        /*
        #region Game
        public void GetTime(ICallback.CallFunc2<string> onCompleted, string actionType = "", bool showLoading = false)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("GAME", "TIME", new UserData(null), actionType));

            this.showLoading = showLoading;
            onLoadTimeCompleted = onCompleted;
            concurrentDictionary.Add(actionType, onCompleted);
            SendWebSocket(json);
        }

        public void SubmitScore(float exp)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<ScoreData>("GAME", "SUBMIT", new ScoreData(null, exp)));
            SendWebSocket(json);
        }

        public void ClaimScore(ICallback.CallFunc2<float> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("GAME", "CLAIM", new UserData(null)));
            onClaimScoreCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void GetFriend(bool showLoading, ICallback.CallFunc2<FriendData[]> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("GAME", "FRIENDS", new UserData(null)));
            onGetFriendCompleted = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */
        /*
        #region storage
        public void SaveToCloud(PlayerSaveData data, ICallback.CallFunc onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<PlayerSaveData>("STORAGE", "SAVE", data));
            onSaveGameCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void LoadFromCloud(ICallback.CallFunc2<string> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("STORAGE", "LOAD", new UserData(null)));
            onLoadGameCompleted = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */

        /*
        #region pet
        public void SavePet(GamePetData data, bool showLoading, ICallback.CallFunc2<bool> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<GamePetData>("PET", "SAVE", data));
            onSavePetCompleted = onCompleted;
            this.showLoading = showLoading;
            Debug.Log(json);
            SendWebSocket(json);
        }

        public void LoadPet(int id, bool showLoading, ICallback.CallFunc2<GamePetData> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<LoadPetData>("PET", "LOAD", new LoadPetData(id)));
            onLoadPetCompleted = onCompleted;
            this.showLoading = showLoading;
            SendWebSocket(json);
        }

        public void LoadAllPet(ICallback.CallFunc2<GamePetData[]> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("PET", "QUERY", new UserData(null)));
            onLoadAllPetCompleted = onCompleted;
            SendWebSocket(json);
        }

        // 2 hàm này không có response nên không cần callback nhé, cứ call 1 phát rồi set luôn, không cần đợi response ở đây
        public void SetDefensePet(int petId, bool showLoading)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<LoadPetData>("PET", "DEFENSE", new LoadPetData(petId)));
            this.showLoading = showLoading;
            SendWebSocket(json);
        }

        public void PetEquipeItem(int petId, string _id, bool showLoading)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<PetEquipItem>("PET", "EQUIPE_ITEM", new PetEquipItem(petId, _id)));
            this.showLoading = showLoading;
            SendWebSocket(json);
        }
        #endregion
        */

        /*
        #region minigame
        public void SubmitScoreLeaderboard(int score, ICallback.CallFunc2<LeaderboardResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<SubmitLeaderboardData>("MINIGAME", "SUBMIT", new SubmitLeaderboardData(null, score)));
            onSubmitLeaderboardResponse = onCompleted;
            SendWebSocket(json);
        }

        public void ShowLeaderboard(float time, ICallback.CallFunc2<LeaderboardResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("MINIGAME", "LEADERBOARD", new UserData(null)));
            onGetLeaderboardResponse = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */

        /*
        #region reminder
        public void SendReminder(string message, int scheduleTime)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<SendReminderData>("REMINDER", "SCHEDULE", new SendReminderData(null, message, scheduleTime)));
            SendWebSocket(json);
        }

        public void CancelReminder()
        {
            string json = JsonConvert.SerializeObject(new SendCommand<CancelReminderData>("REMINDER", "CANCEL", new CancelReminderData(null, PlayerData.Instance.data.reminderCode)));
            SendWebSocket(json);
        }
        #endregion
        */

        /*
        #region invoice
        public void CreateInvoiceLink(int amount, ICallback.CallFunc3<string, string> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<InvoiceData>("INVOICE", "LINK", new InvoiceData(amount)));
            onCreateInvoiceCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void CheckInvoice(string transactionId, ICallback.CallFunc2<bool> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<TransactionReceived>("INVOICE", "STATUS", new TransactionReceived(null, transactionId)));
            onCheckStatusCompleted = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */

        /*
        #region shop pvp
        public void QueryPvpShop(ICallback.CallFunc2<PvpShopQueryResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("SHOP", "QUERY", new UserData(null)));
            onPvpShopQueryCallback = onCompleted;
            SendWebSocket(json);
        }

        public void RefreshPvpShop(ICallback.CallFunc2<PvpShopQueryResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("SHOP", "REFRESH", new UserData(null)));
            onPvpShopRefreshCallback = onCompleted;
            SendWebSocket(json);
        }

        public void BuyPvpShopItem(int itemId, ICallback.CallFunc2<PvpShopQueryResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<BuyPvpShopItem>("SHOP", "BUY", new BuyPvpShopItem(itemId)));
            onPvpShopBuyCallback = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */

        /*
        #region inventory pvp
        public void QueryPvpInventory(ICallback.CallFunc3<InventoryPvpItemData[], UpgradePvpItemReseponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("ITEM", "QUERY", new UserData(null)));
            onPvpInventoryQueryCallback = onCompleted;
            SendWebSocket(json);
        }

        public void UpgradePvpItem(string _id, ICallback.CallFunc3<InventoryPvpItemData[], UpgradePvpItemReseponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UpgradePvpIventoryItem>("ITEM", "UPGRADE", new UpgradePvpIventoryItem(_id)));
            onPvpInventoryUpgradeCallback = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */
        /*
        #region pvp
        public void GetProfile(bool showLoading, ICallback.CallFunc2<PVPProfileResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("PVP", "PROFILE", new UserData(null)));
            onGetProfileCompleted = onCompleted;
            this.showLoading = showLoading;
            SendWebSocket(json);
        }

        // chỗ này sau khi get profile xong mà chưa có faction thì gọi là select faction
        // chỗ này gửi lên tên của faction nhé (Tongo) hoặc (Ochi)
        // check response của GetProfile để hiểu hơn
        public void SelectFaction(string factionName, ICallback.CallFunc2<PVPProfileResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<SendFactionData>("PVP", "FACTION", new SendFactionData(factionName)));
            onSelectFactionCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void GetPvpLeaderboard(ICallback.CallFunc2<PvpLeaderboardResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("PVP", "LEADERBOARD", new UserData(null)));
            onGetLeaderboardCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void GetPvpTeamLeaderboard(ICallback.CallFunc2<PvpLeaderboardResponse> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("PVP", "TEAM_LEADERBOARD", new UserData(null)));
            onGetTeamLeaderboardCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void GoCombat(int petId, ICallback.CallFunc2<PvpCombat> onCompleted)
        {
            string json = JsonConvert.SerializeObject(new SendCommand<LoadPetData>("PVP", "COMBAT", new LoadPetData(petId)));
            onCombatCompleted = onCompleted;
            SendWebSocket(json);
        }

        public void ResetAttackCount(ICallback.CallFunc2<bool> onCompleted)
        {
            // "PVP" "RESET"
            string json = JsonConvert.SerializeObject(new SendCommand<UserData>("PVP", "RESET", new UserData(null)));
            onResetAttackCountCompleted = onCompleted;
            SendWebSocket(json);
        }
        #endregion
        */
        #endregion

        #endregion
    }
}