using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pet
{
    public class LoadAllPetCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public LoadAllPetCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("PET", "QUERY", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}