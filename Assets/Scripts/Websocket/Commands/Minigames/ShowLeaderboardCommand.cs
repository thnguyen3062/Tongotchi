using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Minigame
{
    public class ShowLeaderboardCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private Body _body;
        public ShowLeaderboardCommand(string actionType, string minigameId)
        {
            _actionType = actionType;
            _body = new Body();
            _body.minigameId = minigameId;
        }

        public class Body
        {
            public string minigameId;
        }

        public string ToJson()
        {
            var command = new SendCommand<Body>("MINIGAME", "LEADERBOARD", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}