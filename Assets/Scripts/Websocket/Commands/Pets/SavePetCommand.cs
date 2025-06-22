using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Websocket.Commands.Pet
{
    public class SavePetCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly GamePetData _data;

        public SavePetCommand(GamePetData data, string actionType)
        {
            _data = data;
            _actionType = actionType;
        }

        public string ToJson()
        {
            var command = new SendCommand<GamePetData>("PET", "SAVE", _data, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}