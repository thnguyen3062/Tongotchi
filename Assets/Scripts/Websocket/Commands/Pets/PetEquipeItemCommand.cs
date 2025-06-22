using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class PetEquipeItemCommand : IWebSocketCommand
    {
        private readonly int _petId;
        private readonly string _id;

        public PetEquipeItemCommand(int petId, string id)
        {
            _petId = petId;
            _id = id;
        }

        public string ToJson()
        {
            var command = new SendCommand<PetEquipItem>("PET", "EQUIPE_ITEM", new PetEquipItem(_petId, _id));
            return JsonConvert.SerializeObject(command);
        }
    }
}