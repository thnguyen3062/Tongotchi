using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Invoice
{
    public class CreateInvoiceLinkCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        public readonly int _amount;

        public CreateInvoiceLinkCommand(int amount, string actionType)
        {
            _amount = amount;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<InvoiceData>("INVOICE", "LINK", new InvoiceData(_amount), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}