using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class EvolvePetCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private int petId;

        public EvolvePetCommand(string requestId, int petId)
        {
            _actionType = requestId;
            this.petId = petId;
        }

        public string ToJson()
        {
            var command = new SendCommand<LoadPetData>("PET", "EVOLVE", new LoadPetData(petId), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

public class EvolveResultBody
{
    public bool evolve;
    public string msg;
    public int petId;
    public int petEvolveLevel;
    public int petLevel;
}