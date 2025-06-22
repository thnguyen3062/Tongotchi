using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.ShopPvp
{
    public class BuyPvpShopItemCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly int _itemId;

        public BuyPvpShopItemCommand(int itemId, string actionType)
        {
            _itemId = itemId;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<BuyPvpShopItem>("SHOP", "BUY", new BuyPvpShopItem(_itemId), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}