using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using System;

namespace Game.Websocket.Commands.Storage
{
    public class PlayMinigameCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData _data;

        public PlayMinigameCommand(string requestId, string telegramCode)
        {
            _actionType = requestId;
            _data = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "PLAY_MINI_GAME", _data, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }

    public class PlayMinigameResponse
    {
        public bool success;
        public string message;
        public int remainingPlays;
        public int purchasedToday;
        public int maxPurchasesPerDay;
    }
}