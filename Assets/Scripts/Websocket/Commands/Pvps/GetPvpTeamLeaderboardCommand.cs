using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pvp
{
    public class GetPvpTeamLeaderboardCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public GetPvpTeamLeaderboardCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("PVP", "TEAM_LEADERBOARD", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}