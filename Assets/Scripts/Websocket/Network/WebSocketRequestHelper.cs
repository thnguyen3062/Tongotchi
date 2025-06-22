using Game.Websocket.Commands.Game;
using Game.Websocket.Commands.InventoryPvp;
using Game.Websocket.Commands.Invoice;
using Game.Websocket.Commands.Pet;
using Game.Websocket.Commands.Pvp;
using Game.Websocket.Commands.ShopPvp;
using Game.Websocket.Commands.Storage;
using Game.Websocket.Model;
using Game.Websocket.Utils;
using System;
using Game.Websocket.Commands.Minigame;
using System.Threading.Tasks;
using Game.Websocket.Commands.Reminder;
using Game.Websocket.Commands.Compensation;
using Game.Websocket.Commands;
using Game.Websocket.Commands.Tickets;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using Game.Websocket.Commands.Cheat;

namespace Game.Websocket
{
    public static class WebSocketRequestHelper
    {
        #region GAME

        public static void GetTimeOnce(Action<string> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetTimeCommand(requestId));
        }

        public static void ClaimScoreOnce(Action<ClaimScoreResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ClaimScoreCommand(requestId));
        }

        public static void GetFriendListOnce(Action<FriendData[]> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetFriendCommand(requestId));
        }

        public static void SubmitScore(float exp)
        {
            _ = WebSocketConnector.Instance.SendCommand(new SubmitScoreCommand(exp));
        }

        #endregion

        #region STORAGE
        public static void LoadFromCloudOnce(Action<string> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new LoadFromCloudCommand(requestId));
        }

        public static void SaveToCloud(PlayerSaveData data, Action onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new SaveToCloudCommand(data, requestId));
        }

        public static void RequestDailyReward(string telegramCode, Action<DailyRewardResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new DailyRewardCommand(requestId, telegramCode));
        }

        public static void RequestAFKFarm(string telegramCode, Action<AFKRewardResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new AFKRewardCommand(requestId, telegramCode));
        }

        public static void GetTickets(string telegramCode, Action<GetCurrencyResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetTicketCommand(requestId, telegramCode));
        }

        public static void IncreaseTicket(string telegramCode, int amount, Action<TicketChangeResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new IncreaseTicketCommand(requestId, telegramCode, amount));
        }

        public static void DecreaseTicket(string telegramCode, int amount, Action<TicketChangeResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new DecreaseTicketCommand(requestId, telegramCode, amount));
        }

        public static void GetDiamond(string telegramCode, Action<GetCurrencyResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetDiamondCommand(requestId, telegramCode));
        }

        public static void IncreaseDiamond(string telegramCode, int amount, Action<TicketChangeResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new IncreaseDiamondCommand(requestId, telegramCode, amount));
        }

        public static void DecreaseDiamond(string telegramCode, int amount, Action<TicketChangeResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new DecreaseDiamondCommand(requestId, telegramCode, amount));
        }

        public static void RequestLoadInventory(Action<Dictionary<int, InventoryItem>> onCompleted = null, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new LoadInventoryCommand(requestId));
        }

        public static void RequestBuyItem(int itemId, int quantity, Action<BuyItemResponse> onCompleted = null, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new AddItemCommand(requestId, itemId, quantity));
        }

        public static void RequestPlayMinigame(string telegramCode, Action<PlayMinigameResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new PlayMinigameCommand(requestId, telegramCode));
        }

        public static void RequestChangePetId(int petId, Action onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ChangePetCommand(requestId, petId));
        }

        public static void RequestClaimTask(int taskIndex, Action onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ClaimSocialTaskCommand(requestId, taskIndex));
        }

        public static void TestStorage(string telegramCode, Action onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new TestStorageCommand(requestId, telegramCode));
        }

        public static void RequestResetMinigameTicket(string telegramCode, Action<PurchaseResponse> onCompleted, float timeoutSeconds = 5)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ResetMinigameCommand(requestId, telegramCode));
        }

        public static void RequestGetMinigameTicket(string telegramCode, Action<GetMinigameTicketResponse> onCompleted, float timeoutSeconds = 5)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetMinigameTicketCommand(requestId, telegramCode));
        }

        public static void RequestQueryBoost(int petId, Action<GetBoostsResponse> onCompleted, float timeoutSeconds = 5)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetBoostsCommand(requestId, petId));
        }

        public static void RequestToggleSound(bool toggle, Action<ToggleSoundBody> onCompleted = null, float timeoutSeconds = 5)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ToggleSoundCommand(requestId, toggle));
        }
        #endregion

        #region PET
        public static void LoadPetOnce(int petId, Action<GamePetData> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new LoadPetCommand(petId, requestId));
        }

        public static void LoadAllPetOnce(Action<GamePetData[]> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new LoadAllPetCommand(requestId));
        }

        public static void SavePetOnce(GamePetData data, Action<bool> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            if (onCompleted != null)
                OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new SavePetCommand(data, requestId));
        }

        public static void JoyPetOnce(int petId, int itemId, Action<PetStatusesResponse, PetLevelExpResponse> onCompleted)
        {
            CarePetOnce(petId, itemId, "play", onCompleted);
        }

        public static void FeedPetOnce(int petId, int itemId, Action<PetStatusesResponse, PetLevelExpResponse> onCompleted)
        {
            CarePetOnce(petId, itemId, "eat", onCompleted);
        }

        public static void WashPetOnce(int petId,  Action<PetStatusesResponse, PetLevelExpResponse> onCompleted)
        {
            CarePetOnce(petId, -1, "wash", onCompleted);
        }

        public static void CurePetOnce(int petId, int medicineId, Action<PetStatusesResponse, PetLevelExpResponse> onCompleted)
        {
            CarePetOnce(petId, medicineId, "medicine", onCompleted);
        }

        public static void CarePetOnce(int petId, int itemId, string careAction, Action<PetStatusesResponse, PetLevelExpResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new CarePetCommand(petId, itemId, careAction, requestId));
        }

        public static void CheckPetStatsOnce(int petId, Action<PetStatusesResponse, PetLevelExpResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new CheckPetStatsCommand(requestId, petId));
        }

        public static void SetDefensePet(int petId)
        {
            _ = WebSocketConnector.Instance.SendCommand(new SetDefensePetCommand(petId));
        }

        public static void EquipPetItem(int petId, string itemId)
        {
            _ = WebSocketConnector.Instance.SendCommand(new PetEquipeItemCommand(petId, itemId));
        }

        public static void RequestPetState(int petId, string action, Action<PetStateResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new RequestPetStateCommand(requestId, petId, action));
        }

        public static void RequestPetSick(int petId, Action<PetStateResponse> onCompleted)
        {
            RequestPetState(petId, "sick", onCompleted);
        }

        public static void RequestPetDie(int petId, Action<PetStateResponse> onCompleted)
        {
            RequestPetState(petId, "die", onCompleted);
        }

        public static void RequestEvolvePet(int petId, Action<EvolveResultBody> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new EvolvePetCommand(requestId, petId));
        }

        public static void RequestPetPoopAction(int petId, string action, Action<PetPoopResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new RequestPoopCommand(requestId, petId, action));
        }

        public static void RequestCleanPoop(int petId, Action<PetPoopResponse> onCompleted, float timeoutSeconds = 5f)
        {
            RequestPetPoopAction(petId, "clean", onCompleted, timeoutSeconds);
        }

        public static void RequestPetPoop(int petId, Action<PetPoopResponse> onCompleted, float timeoutSeconds = 5f)
        {
            RequestPetPoopAction(petId, "poop", onCompleted, timeoutSeconds);
        }

        public static void RequestRevivePet(int petId, int diamond, Action<RevivePetResponse> callback = null, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, callback, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new RevivePetCommand(requestId, petId, diamond));
        }

        public static void RequestStartFusion(int petId1, int petId2, int potionCount, Action<FusionStartResult> callback = null, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, callback, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new StartFusionCommand(requestId, petId1, petId2, potionCount));
        }

        public static void RequestClaimFusion(string telegramCode, string fusionId, Action<FusionClaimResult, GamePetData> callback = null, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, callback, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ClaimFusionCommand(requestId, telegramCode, fusionId));
        }

        public static void RequestChangeBG(int petId, int backgroundId, Action callback = null, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, callback, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ChangeBGCommand(requestId, petId, backgroundId));
        }
        #endregion

        #region MINIGAME

        public static void SubmitScoreLeaderboardOnce(int score, string minigameId, Action<bool> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new SubmitScoreLeaderboardCommand(score, minigameId, requestId));
        }

        public static void GetLeaderboardOnce(string minigameId, Action<LeaderboardResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ShowLeaderboardCommand(requestId, minigameId));
        }

        public static void ShowLeaderboardOnce(string minigameId, float delayTime, Action<LeaderboardResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();

            OneTimeCallbackUtility.RegisterWithTimeout(requestId, async (LeaderboardResponse result) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delayTime));
                onCompleted?.Invoke(result);
            }, timeoutSeconds);

            _ = WebSocketConnector.Instance.SendCommand(new ShowLeaderboardCommand(requestId, minigameId));
        }

        #endregion

        #region INVOICE

        public static void CreateInvoiceLinkOnce(int amount, Action<string, string> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new CreateInvoiceLinkCommand(amount, requestId));
        }

        public static void CheckInvoiceStatusOnce(string transactionId, Action<bool> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new CheckInvoiceCommand(transactionId, requestId));
        }

        #endregion

        #region REMINDER

        public static void SendReminder(string message, int time)
        {
            _ = WebSocketConnector.Instance.SendCommand(new SendReminderCommand(message, time));
        }

        public static void CancelReminder()
        {
            _ = WebSocketConnector.Instance.SendCommand(new CancelReminderCommand());
        }

        #endregion

        #region SHOP PVP

        public static void QueryPvpShopOnce(Action<PvpShopQueryResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new QueryPvpShopCommand(requestId));
        }

        public static void RefreshPvpShopOnce(Action<PvpShopQueryResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new RefreshPvpShopCommand(requestId));
        }

        public static void BuyPvpShopItemOnce(int itemId, Action<PvpShopQueryResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new BuyPvpShopItemCommand(itemId, requestId));
        }

        public static void BuyBoost(int itemId, int petId, Action<BuyBoostResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new BuyBoostCommand(requestId, itemId, petId));
        }
        #endregion

        #region INVENTORY PVP

        public static void QueryPvpInventoryOnce(Action<InventoryPvpItemData[], UpgradePvpItemReseponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();

            OneTimeCallbackUtility.RegisterWithTimeout(requestId, (InventoryPvpItemData[] items) =>
            {
                onCompleted?.Invoke(items, new UpgradePvpItemReseponse());
            }, timeoutSeconds);

            _ = WebSocketConnector.Instance.SendCommand(new QueryPvpInventoryCommand(requestId));
        }

        public static void UpgradePvpItemOnce(string itemId, Action<UpgradePvpItemReseponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();

            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);

            _ = WebSocketConnector.Instance.SendCommand(new UpgradePvpItemCommand(itemId, requestId));
        }

        #endregion

        #region PVP

        public static void GetPvpProfileOnce(Action<PVPProfileResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetProfileCommand(requestId));
        }

        public static void SelectFactionOnce(string factionName, Action<PVPProfileResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new SelectFactionCommand(factionName, requestId));
        }

        public static void GetPvpLeaderboardOnce(Action<PvpLeaderboardResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetPvpLeaderboardCommand(requestId));
        }

        public static void GetPvpTeamLeaderboardOnce(Action<PvpLeaderboardResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GetPvpTeamLeaderboardCommand(requestId));
        }

        public static void GoCombatOnce(int petId, Action<PvpCombat> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new GoCombatCommand(petId, requestId));
        }

        public static void ResetAttackCountOnce(Action<PVPProfileResponse> onCompleted, float timeoutSeconds = 5f)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, onCompleted, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new ResetAttackCountCommand(requestId));
        }

        #endregion

        #region Cheat_RemoveAtProduction
        public static void RequestCheatLevel(string telegramCode, int petId, int level, Action callback = null, float timeoutSeconds = 5)
        {
            string requestId = Guid.NewGuid().ToString();
            OneTimeCallbackUtility.RegisterWithTimeout(requestId, callback, timeoutSeconds);
            _ = WebSocketConnector.Instance.SendCommand(new CheatPetLevelCommand(requestId, telegramCode, petId, level));
        }

        #endregion

        #region Compensation
        public static void CollectCompensationOnce()
        {
            _ = WebSocketConnector.Instance.SendCommand(new CollectCompensationCommand());
        }
        #endregion
    }
}