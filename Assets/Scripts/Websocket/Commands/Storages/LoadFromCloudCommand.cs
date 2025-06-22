using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Storage
{
    public class LoadFromCloudCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public LoadFromCloudCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "LOAD", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}