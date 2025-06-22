using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.ShopPvp
{
    public class RefreshPvpShopCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public RefreshPvpShopCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("SHOP", "REFRESH", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}