using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.InventoryPvp
{
    public class QueryPvpInventoryCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public QueryPvpInventoryCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("ITEM", "QUERY", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}