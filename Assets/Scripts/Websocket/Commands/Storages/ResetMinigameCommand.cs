using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Websocket.Commands.Storage
{
    public class PurchaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int RemainingPlays { get; set; }
        public int PurchasedToday { get; set; }
        public int MaxPurchasesPerDay { get; set; }
        public int Diamonds { get; set; }
    }

    public class ResetMinigameCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData _body;

        public ResetMinigameCommand(string requestId, string telegramCode)
        {
            _actionType = requestId;
            _body = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "MINI_GAME_BUY_TIMES", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}