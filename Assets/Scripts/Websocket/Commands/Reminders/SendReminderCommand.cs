using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Reminder
{
    public class SendReminderCommand : IWebSocketCommand
    {
        private readonly string _message;
        private readonly int _scheduleTime;

        public SendReminderCommand(string message, int scheduleTime)
        {
            _message = message;
            _scheduleTime = scheduleTime;
        }

        public string ToJson()
        {
            var command = new SendCommand<SendReminderData>("REMINDER", "SCHEDULE", new SendReminderData(null, _message, _scheduleTime));
            return JsonConvert.SerializeObject(command);
        }
    }
}