using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class SetDefensePetCommand : IWebSocketCommand
    {
        private readonly int _petId;

        public SetDefensePetCommand(int petId)
        {
            _petId = petId;
        }

        public string ToJson()
        {
            var command = new SendCommand<LoadPetData>("PET", "DEFENSE", new LoadPetData(_petId));
            return JsonConvert.SerializeObject(command);
        }
    }
}