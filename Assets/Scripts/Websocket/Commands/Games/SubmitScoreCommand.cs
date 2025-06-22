using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Game
{
    public class SubmitScoreCommand : IWebSocketCommand
    {
        private readonly float _exp;

        public SubmitScoreCommand(float exp)
        {
            _exp = exp;
        }

        public string ToJson()
        {
            var command = new SendCommand<ScoreData>("GAME", "SUBMIT", new ScoreData(null, _exp));
            return JsonConvert.SerializeObject(command);
        }
    }
}