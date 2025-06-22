using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pvp
{
    public class GetProfileCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public GetProfileCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("PVP", "PROFILE", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}