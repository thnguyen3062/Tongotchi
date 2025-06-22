using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Storage
{
    public class LoadInventoryCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData _userData;

        public LoadInventoryCommand(string requestId, string telegramCode = null)
        {
            _actionType = requestId;
            _userData = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "LOAD_ITEM", _userData, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}