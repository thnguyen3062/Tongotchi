using Game.Websocket.Interface;
using UnityEngine;
using Newtonsoft.Json;
using Game.Websocket.Model;
namespace Game.Websocket.Commands.Storage
{
    public class TestStorageCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData _userData;

        public TestStorageCommand(string requestId, string telegramCode)
        {
            _actionType = requestId;
            _userData = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "TEST", _userData, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}