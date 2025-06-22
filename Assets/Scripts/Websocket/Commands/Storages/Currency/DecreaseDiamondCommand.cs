using UnityEngine;
using Newtonsoft.Json;
using Game.Websocket.Interface;
using Game.Websocket.Model;
namespace Game.Websocket.Commands
{
    public class DecreaseDiamondCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private CurrencyAmountPacket _data;

        public DecreaseDiamondCommand(string actionType, string telegramCode, int decreaseAmount)
        {
            _actionType = actionType;
            _data = new CurrencyAmountPacket(telegramCode, decreaseAmount);
        }


        public string ToJson()
        {
            var command = new SendCommand<CurrencyAmountPacket>("STORAGE", "DECREASE_DIAMOND", _data, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}