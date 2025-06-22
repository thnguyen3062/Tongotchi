using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class StartFusionCommand : IWebSocketCommand
    {
        private class RequestBody
        {
            public int petId1;
            public int petId2;
            public int additionalPotions;
        }

        private readonly string _actionType;
        private readonly RequestBody _body;

        public StartFusionCommand(string actionType, int petId1, int petId2, int potionCount)
        {
            _actionType = actionType;
            _body = new RequestBody();
            _body.petId1 = petId1;
            _body.petId2 = petId2;
            _body.additionalPotions = potionCount;
        }

        public string ToJson()
        {
            var command = new SendCommand<RequestBody>("PET", "FUSION_START", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}