using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands
{
    public class AFKRewardCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData _userData;

        public AFKRewardCommand(string requestId, string telegramCode)
        {
            _actionType = requestId;
            _userData = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "AFK_FARM", _userData, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}