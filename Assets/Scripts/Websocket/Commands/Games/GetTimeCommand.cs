using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Game
{
    public class GetTimeCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public GetTimeCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("GAME", "TIME", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}