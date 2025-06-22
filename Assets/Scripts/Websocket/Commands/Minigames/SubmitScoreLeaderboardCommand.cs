using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Minigame
{
    public class SubmitScoreLeaderboardCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly int _score;
        private readonly string _minigameId;

        public SubmitScoreLeaderboardCommand(int score, string minigameId, string actionType)
        {
            _score = score;
            _actionType = actionType;
            _minigameId = minigameId;
        }

        public string ToJson()
        {
            var command = new SendCommand<SubmitLeaderboardData>("MINIGAME", "SUBMIT", new SubmitLeaderboardData(null, _score, _minigameId), _actionType);
                return JsonConvert.SerializeObject(command);
        }
    }
}