using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Storage
{
    public class SaveToCloudCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly PlayerSaveData _data;

        public SaveToCloudCommand(PlayerSaveData data, string actionType)
        {
            _data = data;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<PlayerSaveData>("STORAGE", "SAVE", _data, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}