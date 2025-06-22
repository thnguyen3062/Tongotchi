using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Game.Websocket.Commands.Storage
{
    public class GetBoostsCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private LoadPetData _body;

        public GetBoostsCommand(string requestId, int petId)
        {
            _actionType = requestId;
            _body = new LoadPetData(petId);
        }

        public string ToJson()
        {
            var command = new SendCommand<LoadPetData>("STORAGE", "GET_BOOSTS", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

public class GetBoostsResponse
{
    public bool success;
    public List<BoostItem> pet_boosts;
    public List<BoostItem> player_boosts;
}