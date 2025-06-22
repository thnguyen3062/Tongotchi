using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
namespace Game.Websocket.Commands
{
    public class GetTicketCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private int amount;

        private UserData packet;

        public GetTicketCommand(string actionType, string telegramCode)
        {
            _actionType = actionType;
            packet = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "GET_TICKET", packet, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}