using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Websocket.Commands.Storage
{
    public class ToggleSoundCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly ToggleSoundBody _body;

        public ToggleSoundCommand(string actionType, bool isOn)
        {
            _actionType = actionType;
            _body = new ToggleSoundBody(isOn);
        }

        public string ToJson()
        {
            var command = new SendCommand<ToggleSoundBody>("STORAGE", "SETTINGS", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }

    public class ToggleSoundBody
    {
        public bool isOn;

        public ToggleSoundBody(bool isOn)
        {
            this.isOn = isOn;
        }
    }
}