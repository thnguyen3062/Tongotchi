using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.InventoryPvp
{
    public class UpgradePvpItemCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly string _id;

        public UpgradePvpItemCommand(string id, string actionType)
        {
            _id = id;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UpgradePvpIventoryItem>("ITEM", "UPGRADE", new UpgradePvpIventoryItem(_id), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}