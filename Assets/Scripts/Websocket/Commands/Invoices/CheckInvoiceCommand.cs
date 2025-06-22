using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Invoice
{
    public class CheckInvoiceCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly string _transactionId;

        public CheckInvoiceCommand(string transactionId, string actionType)
        {
            _transactionId = transactionId;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<TransactionReceived>("INVOICE", "STATUS", new TransactionReceived(null, _transactionId), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}