using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

namespace Game.Websocket.Commands.Compensation
{
    public class CollectCompensationCommand : IWebSocketCommand
    {
        public string ToJson()
        {
            var command = new SendCommand<UserData>("COMPENSATION", "RECEIVE", new UserData(null));
            return JsonConvert.SerializeObject(command);
        }
    }
}