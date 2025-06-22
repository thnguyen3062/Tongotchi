using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Game
{
    public class GetFriendCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public GetFriendCommand(string actionType)
        {
            _actionType = actionType;
        }
        public string ToJson()
        {
            var command = new SendCommand<UserData>("GAME", "FRIENDS", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}