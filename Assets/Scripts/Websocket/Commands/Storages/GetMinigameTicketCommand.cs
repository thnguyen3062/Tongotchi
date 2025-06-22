using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Storage
{
    public class GetMinigameTicketResponse
    {
        public bool success;
        public string message;
        public int remainingPlays;
    }

    public class GetMinigameTicketCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData _body;

        public GetMinigameTicketCommand(string actionType, string telegramCode)
        {
            _actionType = actionType;
            _body = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "GET_MINI_GAME_TICKET", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}