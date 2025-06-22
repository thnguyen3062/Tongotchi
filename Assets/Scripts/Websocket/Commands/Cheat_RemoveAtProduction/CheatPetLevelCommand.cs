using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Cheat
{
    public class CheatPetLevelCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private class RequestBody
        {
            public string telegramCode;
            public int petId;
            public int level;
        }

        private readonly RequestBody _body;

        public CheatPetLevelCommand(string actionType, string telegramCode, int petId, int level)
        {
            _actionType = actionType;
            _body = new RequestBody();
            _body.telegramCode = telegramCode;
            _body.petId = petId;
            _body.level = level;
        }

        public string ToJson()
        {
            var command = new SendCommand<RequestBody>("ITEM", "BUY", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}