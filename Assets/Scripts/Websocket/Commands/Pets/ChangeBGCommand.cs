using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class ChangeBGCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly Body _body;

        private class Body
        {
            public int petId;
            public int backgroundId;
        }

        public ChangeBGCommand(string actionType, int petId, int backgroundId)
        {
            _actionType = actionType;
            _body = new Body { petId = petId, backgroundId = backgroundId };
        }

        public string ToJson()
        {
            var command = new SendCommand<Body>("PET", "BACKGROUND", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}