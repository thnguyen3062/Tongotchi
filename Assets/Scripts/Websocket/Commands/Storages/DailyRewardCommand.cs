using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Storage
{
    public class DailyRewardCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private UserData packet;

        public DailyRewardCommand(string requestId, string telegramCode)
        {
            _actionType = requestId;
            packet = new UserData(telegramCode);
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("STORAGE", "DAILY_REWARD", packet, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

public class DailyRewardResponse
{
    public int reward;
    public int current_day;
    public string message;
    public bool success;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}