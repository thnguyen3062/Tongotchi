using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
namespace Game.Websocket.Commands.Pet
{
    public class InteractRequest
    {
        public int petId;
        public int itemId;
        public string actionType;

        public InteractRequest(int petId, int itemId, string actionType)
        {
            this.petId = petId;
            this.itemId = itemId;
            this.actionType = actionType;
        }
    }

    public class CheckPetStatsCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private InteractRequest _request;

        public CheckPetStatsCommand(string actionType, int petId)
        {
            _request = new InteractRequest(petId, -1, string.Empty);
            _actionType = actionType;
        }

        public virtual string ToJson()
        {
            var command = new SendCommand<InteractRequest>("PET", "STATUS", _request, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }

    public class CarePetCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private InteractRequest _request;

        public CarePetCommand(int petId, int itemId, string interactAction, string actionType)
        {
            _request = new InteractRequest(petId, itemId, interactAction);
            _actionType = actionType;
        }

        public virtual string ToJson()
        {
            var command = new SendCommand<InteractRequest>("PET", "CARE", _request, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

public class PetLevelExpResponse
{
    public int petLevel;
    public float petExp;
    public bool isLevelUp;
    public bool needEvolved;
}

public class PetStatusesResponse
{
    public StatusData status;
    public int quantity;
    public bool isSick;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
