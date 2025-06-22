using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Websocket.Commands.Storage
{
    public class PoopRequestBody
    {
        public int petId;
        public string action;

        public PoopRequestBody(int petId, string action)
        {
            this.petId = petId;
            this.action = action;
        }
    }

    public class RequestPoopCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private PoopRequestBody _body;

        public RequestPoopCommand(string actionType, int petId, string action)
        {
            _actionType = actionType;
            _body = new PoopRequestBody(petId, action);
        }

        public string ToJson()
        {
            var command = new SendCommand<PoopRequestBody>("PET", "POOP", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

/// <summary>
/// This model is used to handle both pooping and cleaning poop(s).
/// </summary>
public class PetPoopResponse
{
    public bool success;

    public int poopsSpawned;

    public string message;

    public int timeRemaining;

    public int poopsCleaned;
}