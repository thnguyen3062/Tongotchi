using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class LoadPetCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly int _id;

        public LoadPetCommand(int id, string actionType)
        {
            _id = id;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<LoadPetData>("PET", "LOAD", new LoadPetData(_id), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}