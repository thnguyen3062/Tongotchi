using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Game
{
    public class ClaimScoreCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public ClaimScoreCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("GAME", "CLAIM", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }

    public class ClaimScoreResponse
    {
        public float referenceExp;
        public int referenceLv;
    }
}