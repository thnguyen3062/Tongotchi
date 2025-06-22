using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using System;

namespace Game.Websocket.Commands.Pvp
{
    public class ResetAttackCountCommand : IWebSocketCommand
    {
        private readonly string _actionType;

        public ResetAttackCountCommand(string actionType)
        {
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<UserData>("PVP", "RESET", new UserData(null), _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}