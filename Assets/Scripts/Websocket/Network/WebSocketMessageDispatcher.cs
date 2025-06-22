using Game.Websocket.Commands.Game;
using Game.Websocket.Commands.Pvp;
using Game.Websocket.Commands.Storage;
using Game.Websocket.Model;
using Game.Websocket.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Websocket
{
    public class WebSocketMessageDispatcher
    {
        public void DispatchMessage(SocketResponse response)
        {
            switch (response.group)
            {
                case "GAME":
                    DispatchGameResponse(response);
                    break;
                case "PET":
                    DispatchPetResponse(response);
                    break;
                case "STORAGE":
                    DispatchStorageResponse(response);
                    break;
                case "MINIGAME":
                    DispatchMinigameResponse(response);
                    break;
                case "REMINDER":
                    DispatchReminderResponse(response);
                    break;
                case "INVOICE":
                    DispatchInvoiceResponse(response);
                    break;
                case "SHOP":
                    DispatchShopResponse(response);
                    break;
                case "ITEM":
                    DispatchItemResponse(response);
                    break;
                case "PVP":
                    DispatchPvpResponse(response);
                    break;
                default:
                    Debug.LogWarning($"Unhandled group: {response.group}");
                    break;
            }
        }

        #region GAME

        private void DispatchGameResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "FRIENDS":
                    HandleGameFriendsResponse(response);
                    break;
                case "TIME":
                    HandleGameTimeResponse(response);
                    break;
                case "CLAIM":
                    HandleGameClaimResponse(response);
                    break;
                default:
                    Debug.Log($"Unhandled GAME command: {response.command}");
                    break;
            }
        }

        private void HandleGameFriendsResponse(SocketResponse response)
        {
            var friends = JsonConvert.DeserializeObject<FriendData[]>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, friends);
        }

        private void HandleGameTimeResponse(SocketResponse response)
        {
            TimeResponse data = JsonConvert.DeserializeObject<TimeResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, data.currrent);
        }

        private void HandleGameClaimResponse(SocketResponse response)
        {
            ClaimScoreResponse responseBody = JsonConvert.DeserializeObject<ClaimScoreResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, responseBody);
        }
        #endregion

        #region PET

        private void DispatchPetResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "LOAD":
                    HandlePetLoadResponse(response);
                    break;
                case "QUERY":
                    HandlePetQueryResponse(response);
                    break;
                case "SAVE":
                    HandlePetSaveResponse(response);
                    break;
                case "CARE_CARE":
                    HandleCarePetResponse(response, "CARE");
                    break;
                case "CARE_EXP":
                    HandleCarePetResponse(response, "EXP");
                    break;
                case "STATUS_STATUS":
                    HandleCheckPetStatusResponse(response, "STATUS");
                    break;
                case "STATUS_EXP":
                    HandleCheckPetStatusResponse(response, "EXP");
                    break;
                case "STATE":
                    HandlePetStateResponse(response);
                    break;
                case "CHANGE":
                    HandleChangePetResponse(response);
                    break;
                case "EVOLVE_RESULT":
                    HandlePetEvolveResponse(response, "RESULT");
                    break;
                case "POOP":
                    HandlePoopResponse(response);
                    break;
                case "REVIVE":
                    HandleReviveResponse(response);
                    break;
                case "FUSION_START_RESULT":
                    HandleStartFusionResponse(response);
                    break;
                case "FUSION_CLAIM_RESULT":
                    HandleFusionPetResponse(response, "RESULT");
                    break;
                case "FUSION_CLAIM_ENTITY":
                    HandleFusionPetResponse(response, "ENTITY");
                    break;
                case "BACKGROUND":
                    HandleChangeBGResponse(response);
                    break; 
                default:
                    Debug.Log($"Unhandled PET command: {response.command}");
                    break;
            }
        }
        private void HandleReviveResponse(SocketResponse response)
        {
            RevivePetResponse body = JsonConvert.DeserializeObject<RevivePetResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, body);
        }
        private void HandlePoopResponse(SocketResponse response)
        {
            PetPoopResponse body = JsonConvert.DeserializeObject<PetPoopResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, body);
        }

        private void HandlePetLoadResponse(SocketResponse response)
        {
            var pet = JsonConvert.DeserializeObject<GamePetData>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, pet);
        }

        private void HandlePetQueryResponse(SocketResponse response)
        {
            var pets = JsonConvert.DeserializeObject<GamePetData[]>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, pets);
        }

        private void HandlePetSaveResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke<bool>(response.actionType, true);
        }

        private void HandleCarePetResponse(SocketResponse response, string subCommand)
        {
            HandlePetCareStatusExpResponse(response, subCommand);
        }

        private void HandleCheckPetStatusResponse(SocketResponse response, string subCommand)
        {
            HandlePetCareStatusExpResponse(response, subCommand);
        }

        private int petCareResponseMax = 2;
        private int petCareResponseCount = 0;
        PetLevelExpResponse levelExpData = null;
        PetStatusesResponse statusData = null;

        private class ExpResponse
        {
            public PetLevelExpResponse expRes;
        }

        private void HandlePetCareStatusExpResponse(SocketResponse response, string subCommand)
        {
            switch (subCommand)
            {
                case "CARE":
                case "STATUS":
                    petCareResponseCount++;
                    statusData = JsonConvert.DeserializeObject<PetStatusesResponse>(response.content);
                    break;
                case "EXP":
                    petCareResponseCount++;
                    ExpResponse res = JsonConvert.DeserializeObject<ExpResponse>(response.content);
                    levelExpData = res.expRes;
                    break;
                default:
                    Debug.LogError("Unknow Sub-Command for PET_CARE response");
                    break;
            }

            if (petCareResponseCount == petCareResponseMax)
            {
                OneTimeCallbackUtility.Invoke(response.actionType, statusData, levelExpData);
                petCareResponseCount = 0;
            }
        }

        private int petEvolveResponseMax = 1;
        private int petEvolveResponseCount = 0;
        GamePetData petEntity = null;
        EvolveResultBody evolveResult = null;
        private void HandlePetEvolveResponse(SocketResponse response, string subCommand)
        {
            switch (subCommand)
            {
                //case "ENTITY":
                //    petEvolveResponseCount++;
                //    LoggerUtil.Logging("HandlePetEvolveResponse", "ENTITY");
                //    petEntity = JsonConvert.DeserializeObject<GamePetData>(response.content);
                //    break;
                case "RESULT":
                    petEvolveResponseCount++;
                    LoggerUtil.Logging("HandlePetEvolveResponse.RESULT", $"{response.content}");
                    evolveResult = JsonConvert.DeserializeObject<EvolveResultBody>(response.content);
                    break;
            }
            if (petEvolveResponseCount == petEvolveResponseMax)
            {
                OneTimeCallbackUtility.Invoke(response.actionType, evolveResult);
                petEvolveResponseCount = 0;
                petEntity = null;
                evolveResult = null;
            }
        }

        private void HandlePetStateResponse(SocketResponse response)
        {
            PetStateResponse stateData = JsonConvert.DeserializeObject<PetStateResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, stateData);
        }

        private void HandleChangePetResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType);
        }
        
        private void HandleStartFusionResponse(SocketResponse response)
        {
            FusionStartResult result = JsonConvert.DeserializeObject<FusionStartResult>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, result);
        }

        private const int MAX_FUSION_RESPONSE_PACKET = 2;
        private GamePetData entity = null;
        private FusionClaimResult result = null;
        private int fusionResponseCount = 0;

        private void HandleFusionPetResponse(SocketResponse response, string subCommand)
        {
            switch(subCommand)
            {
                case "ENTITY":
                    ++fusionResponseCount;
                    entity = JsonConvert.DeserializeObject<GamePetData>(response.content);
                    break;
                case "RESULT":
                    ++fusionResponseCount;
                    result = JsonConvert.DeserializeObject<FusionClaimResult>(response.content);
                    break;
            }
            if (fusionResponseCount == MAX_FUSION_RESPONSE_PACKET)
            {
                OneTimeCallbackUtility.Invoke(response.actionType, result, entity);
            }
        }

        private void HandleChangeBGResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType);
        }
        #endregion

        #region STORAGE
        private void DispatchStorageResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "LOAD":
                    HandleStorageLoadResponse(response);
                    break;
                case "SAVE":
                    HandleStorageSaveResponse(response);
                    break;
                case "DAILY_REWARD":
                    HandleReceiveDailyReward(response);
                    break;
                case "AFK_FARM":
                    HandleAFKFarmResponse(response);
                    break;
                case "INCREASE_TICKET":
                    HandleIncreaseTicketsResponse(response);
                    break;
                case "DECREASE_TICKET":
                    HandleDecreaseTicketsResponse(response);
                    break;
                case "GET_TICKET":
                    HandleGetTicketsResponse(response);
                    break;
                case "INCREASE_DIAMOND":
                    HandleIncreaseDiamond(response);
                    break;
                case "DECREASE_DIAMOND":
                    HandleDecreaseDiamond(response);
                    break;
                case "GET_DIAMOND":
                    HandleGetDiamond(response);
                    break;
                case "LOAD_ITEM":
                    HandleLoadInventoryResponse(response);
                    break;
                case "BUY_ITEM":
                    HandleBuyItemResponse(response);
                    break;
                case "PLAY_MINI_GAME":
                    HandlePlayMinigameResponse(response);
                    break;
                case "SOCIAL_TASK":
                    HandleClaimTaskResponse(response);
                    break;
                case "MINI_GAME_BUY_TIMES":
                    HandleResetMinigameResponse(response);
                    break;
                case "GET_MINI_GAME_TICKET":
                    HandleGetMinigameTicketResponse(response);
                    break;
                case "GET_BOOSTS":
                    HandleQueryBoosts(response);
                    break;
                case "SETTINGS":
                    HandleSetSoundResponse(response);
                    break;
            }
        }

        private void HandleStorageLoadResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType, response.content);
        }

        private void HandleStorageSaveResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType);
        }

        private void HandleReceiveDailyReward(SocketResponse response)
        {
            DailyRewardResponse _response = JsonConvert.DeserializeObject<DailyRewardResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleIncreaseTicketsResponse(SocketResponse response)
        {
            TicketChangeResponse _response = JsonConvert.DeserializeObject<TicketChangeResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleDecreaseTicketsResponse(SocketResponse response)
        {
            TicketChangeResponse _response = JsonConvert.DeserializeObject<TicketChangeResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleGetTicketsResponse(SocketResponse response)
        {
            GetCurrencyResponse _response = JsonConvert.DeserializeObject<GetCurrencyResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleIncreaseDiamond(SocketResponse response)
        {
            TicketChangeResponse _response = JsonConvert.DeserializeObject<TicketChangeResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleDecreaseDiamond(SocketResponse response)
        {
            TicketChangeResponse _response = JsonConvert.DeserializeObject<TicketChangeResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleGetDiamond(SocketResponse response)
        {
            GetCurrencyResponse _response = JsonConvert.DeserializeObject<GetCurrencyResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _response);
        }

        private void HandleAFKFarmResponse(SocketResponse response)
        {
            AFKRewardResponse _responseContent = JsonConvert.DeserializeObject<AFKRewardResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _responseContent);
        }

        private void HandleLoadInventoryResponse(SocketResponse response)
        {
            Dictionary<int, InventoryItem> data = JsonConvert.DeserializeObject<Dictionary<int, InventoryItem>>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, data);
        }

        private void HandleBuyItemResponse(SocketResponse response)
        {
            BuyItemResponse content = JsonConvert.DeserializeObject<BuyItemResponse>(response.content) as BuyItemResponse;
            OneTimeCallbackUtility.Invoke(response.actionType, content);
        }

        private void HandlePlayMinigameResponse(SocketResponse response)
        {
            PlayMinigameResponse _canPlay = JsonConvert.DeserializeObject<PlayMinigameResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, _canPlay);
        }

        private void HandleClaimTaskResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType);
        }

        private void HandleResetMinigameResponse(SocketResponse response)
        {
            PurchaseResponse body = JsonConvert.DeserializeObject<PurchaseResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, body);
        }

        private void HandleGetMinigameTicketResponse(SocketResponse response)
        {
            GetMinigameTicketResponse body = JsonConvert.DeserializeObject<GetMinigameTicketResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, body);
        }

        private void HandleQueryBoosts(SocketResponse response)
        {
            GetBoostsResponse body = JsonConvert.DeserializeObject<GetBoostsResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, body);
        }
        
        private void HandleSetSoundResponse(SocketResponse response)
        {
            ToggleSoundBody body = JsonConvert.DeserializeObject<ToggleSoundBody>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, body);
        }
        #endregion

        #region MINIGAME

        private void DispatchMinigameResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "SUBMIT":
                    HandleSubmitScore(response);
                    break;
                case "LEADERBOARD":
                    HandleMinigameLeaderboardResponse(response);
                    break;
                default:
                    Debug.Log($"Unhandled MINIGAME command: {response.command}");
                    break;
            }
        }

        public class SubmitScoreResponse
        {
            public string status;
        }

        private void HandleSubmitScore(SocketResponse response)
        {
            var status = JsonConvert.DeserializeObject<SubmitScoreResponse>(response.content);
            bool success = status.status.Equals("SUCCESS");
            OneTimeCallbackUtility.Invoke(response.actionType, success);
        }

        private void HandleMinigameLeaderboardResponse(SocketResponse response)
        {
            var leaderboard = JsonConvert.DeserializeObject<LeaderboardResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, leaderboard);
        }

        #endregion

        #region REMINDER

        private void DispatchReminderResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType, response.content);
        }

        #endregion

        #region INVOICE

        private void DispatchInvoiceResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "LINK":
                    HandleInvoiceLinkResponse(response);
                    break;
                case "STATUS":
                    HandleInvoiceStatusResponse(response);
                    break;
                default:
                    Debug.Log($"Unhandled INVOICE command: {response.command}");
                    break;
            }
        }

        private void HandleInvoiceLinkResponse(SocketResponse response)
        {
            var result = JsonConvert.DeserializeObject<InvoiceResponseData>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, result.transactionCode, result.link);
        }

        private void HandleInvoiceStatusResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType, response.content);
        }

        #endregion

        #region SHOP

        private void DispatchShopResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "QUERY":
                case "REFRESH":
                case "BUY":
                    HandleShopQueryResponse(response);
                    break;
                case "BUYBOOST":
                    HandleBuyBoostResponse(response);
                    break;
                default:
                    Debug.Log($"Unhandled SHOP command: {response.command}");
                    break;
            }
        }

        private void HandleShopQueryResponse(SocketResponse response)
        {
            var shop = JsonConvert.DeserializeObject<PvpShopQueryResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, shop);
        }

        private void HandleBuyBoostResponse(SocketResponse response)
        {
            var data = JsonConvert.DeserializeObject<BuyBoostResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, data);
        }
        #endregion

        #region ITEM

        private void DispatchItemResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "QUERY":
                    HandleItemQueryResponse(response);
                    break;
                case "UPGRADE":
                    HandleItemUpgradeResponse(response);
                    break;
                case "BUY":
                    HandleCheatPetResponse(response);
                    break;
                default:
                    Debug.Log($"Unhandled ITEM command: {response.command}");
                    break;
            }
        }

        private void HandleItemQueryResponse(SocketResponse response)
        {
            var items = JsonConvert.DeserializeObject<InventoryPvpItemData[]>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, items);
        }

        private void HandleItemUpgradeResponse(SocketResponse response)
        {
            var item = JsonConvert.DeserializeObject<InventoryPvpItemData>(response.content);
            var responseItem = new UpgradePvpItemReseponse();
            responseItem._id = item._id;
            OneTimeCallbackUtility.Invoke(response.actionType, responseItem);
        }

        private void HandleCheatPetResponse(SocketResponse response)
        {
            OneTimeCallbackUtility.Invoke(response.actionType);
        }
        #endregion

        #region PVP

        private void DispatchPvpResponse(SocketResponse response)
        {
            switch (response.command)
            {
                case "PROFILE":
                case "FACTION":
                    HandlePvpProfileResponse(response);
                    break;
                case "LEADERBOARD":
                case "TEAM_LEADERBOARD":
                    HandlePvpLeaderboardResponse(response);
                    break;
                case "COMBAT":
                    HandlePvpCombatResponse(response);
                    break;
                case "RESET":
                    HandlePvpResetResponse(response);
                    break;
                default:
                    Debug.Log($"Unhandled PVP command: {response.command}");
                    break;
            }
        }

        private void HandlePvpProfileResponse(SocketResponse response)
        {
            var profile = JsonConvert.DeserializeObject<PVPProfileResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, profile);
        }

        private void HandlePvpLeaderboardResponse(SocketResponse response)
        {
            var leaderboard = JsonConvert.DeserializeObject<PvpLeaderboardResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, leaderboard);
        }

        private void HandlePvpCombatResponse(SocketResponse response)
        {
            var combat = JsonConvert.DeserializeObject<PvpCombat>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, combat);
        }

        private void HandlePvpResetResponse(SocketResponse response)
        {
            PVPProfileResponse callback = JsonConvert.DeserializeObject<PVPProfileResponse>(response.content);
            OneTimeCallbackUtility.Invoke(response.actionType, callback);
        }

        #endregion
    }
}