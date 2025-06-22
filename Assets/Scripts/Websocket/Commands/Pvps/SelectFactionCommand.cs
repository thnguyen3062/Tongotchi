using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Pvp
{
    public class SelectFactionCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly string _factionName;

        public SelectFactionCommand(string factionName, string actionType)
        {
            _factionName = factionName;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<SendFactionData>("PVP", "FACTION", new SendFactionData(_factionName), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}