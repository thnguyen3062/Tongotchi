using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Storage
{
    public class ChangePetCommand : IWebSocketCommand
    {
        public class ChangePetRequestBody
        {
            public int petId;
        }

        private readonly string _actionType;
        private ChangePetRequestBody body;

        public ChangePetCommand(string requestId, int petId)
        {
            _actionType = requestId;
            body = new ChangePetRequestBody();
            body.petId = petId;
        }

        public string ToJson()
        {
            var command = new SendCommand<ChangePetRequestBody>("PET", "CHANGE", body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}