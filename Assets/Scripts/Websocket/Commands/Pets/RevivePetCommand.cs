using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class RevivePetCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private Body _body;
        private class Body
        {
            public int petId;
            public int diamond;

            public Body(int petId, int diamond)
            {
                this.petId = petId;
                this.diamond = diamond;
            }
        }

        public RevivePetCommand(string actionType, int petId, int diamond)
        {
            _actionType = actionType;
            _body = new Body(petId, diamond);
        }

        public string ToJson()
        {
            var command = new SendCommand<Body>("PET", "REVIVE", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

[System.Serializable]
public class RevivePetResponse
{
    public bool isDead;
    public StatusData status;
}