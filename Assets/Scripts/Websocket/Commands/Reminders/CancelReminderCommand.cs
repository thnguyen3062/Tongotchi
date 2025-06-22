using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Reminder
{
    public class CancelReminderCommand : IWebSocketCommand
    {
        public string ToJson()
        {
            var command = new SendCommand<CancelReminderData>("REMINDER", "CANCEL", new CancelReminderData(null, PlayerData.Instance.data.reminderCode));
            return JsonConvert.SerializeObject(command);
        }
    }
}