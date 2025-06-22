using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using UnityEngine;
namespace Game.Websocket.Commands.Pet
{
    public class PetStateRequestBody
    {
        public int petId;
        public string action;

        public PetStateRequestBody(int petId, string action)
        {
            this.petId = petId;
            this.action = action;
        }
    }

    public class RequestPetStateCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private PetStateRequestBody _body;

        public RequestPetStateCommand(string requestId, int petId, string action)
        {
            _actionType = requestId;
            _body = new PetStateRequestBody(petId, action);
        }

        public string ToJson()
        {
            var command = new SendCommand<PetStateRequestBody>("PET", "STATE", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}