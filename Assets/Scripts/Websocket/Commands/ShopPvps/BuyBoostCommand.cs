using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Websocket.Commands
{
    public class BuyBoostCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private BuyBoostRequest _body;
        public class BuyBoostRequest
        {
            public int itemId;
            public int petId;
        }

        public BuyBoostCommand(string requestId, int itemId, int petId)
        {
            _actionType = requestId;
            _body = new BuyBoostRequest { 
                itemId = itemId,
                petId = petId
            };
        }

        public string ToJson()
        {
            var command = new SendCommand<BuyBoostRequest>("SHOP", "BUYBOOST", _body, _actionType);
            return JsonConvert.SerializeObject(command);

        }
    }
}

public class BuyBoostResponse
{
    public bool success;
    public string message;
    public List<BoostItem> boost;
}