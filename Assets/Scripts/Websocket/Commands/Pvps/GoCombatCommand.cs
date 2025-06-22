using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pvp
{
    public class GoCombatCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly int _petId;

        public GoCombatCommand(int petId, string actionType)
        {
            _petId = petId;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<LoadPetData>("PVP", "COMBAT", new LoadPetData(_petId), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}