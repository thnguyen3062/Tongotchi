using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Websocket.Commands.Storage
{
    public class AddItemRequestBody
    {
        public int itemId;
        public int quantity;

        public AddItemRequestBody(int itemId, int amount)
        {
            this.itemId = itemId;
            this.quantity = amount;
        }
    }

    public class AddItemCommand : IWebSocketCommand
    {
        private string _actionType;
        private AddItemRequestBody _body;

        public AddItemCommand(string requestId, int itemId, int amount)
        {
            _actionType = requestId;
            _body = new AddItemRequestBody(itemId, amount);
        }

        public string ToJson()
        {
            var command = new SendCommand<AddItemRequestBody>("STORAGE", "BUY_ITEM", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

public class BuyItemResponse
{
    public bool success;
    public int itemId;
    public int newQuantity;
    public bool wasUpdated;
}