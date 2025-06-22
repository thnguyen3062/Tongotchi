using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.ShopPvp
{
    public class QueryPvpShopCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public QueryPvpShopCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("SHOP", "QUERY", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}