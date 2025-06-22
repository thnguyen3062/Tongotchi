using Game.Websocket.Interface;
using Newtonsoft.Json;
namespace Game.Websocket.Commands.Tickets
{
    using global::Game.Websocket.Model;

    public class DecreaseTicketCommand : IWebSocketCommand
    {
        private string _actionType;
        private CurrencyAmountPacket _data;

        public DecreaseTicketCommand(string actionType, string telegramCode, int decreaseAmount)
        {
            _actionType = actionType;
            _data = new CurrencyAmountPacket(telegramCode, decreaseAmount);
        }

        public string ToJson()
        {
            var command = new SendCommand<CurrencyAmountPacket>("STORAGE", "DECREASE_TICKET", _data, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}