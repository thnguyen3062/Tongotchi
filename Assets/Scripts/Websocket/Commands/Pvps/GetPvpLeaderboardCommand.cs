using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pvp
{
    public class GetPvpLeaderboardCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public GetPvpLeaderboardCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("PVP", "LEADERBOARD", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}