using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

public class ClaimSocialTaskCommand : IWebSocketCommand
{
    private readonly string _actionType;
    private ClaimTaskRequestBody _body;
    class ClaimTaskRequestBody
    {
        public int taskIndex;
    }

    public ClaimSocialTaskCommand(string requestId, int taskIndex)
    {
        _actionType = requestId;
        _body = new ClaimTaskRequestBody();
        _body.taskIndex = taskIndex;
    }

    public string ToJson()
    {
        var command = new SendCommand<ClaimTaskRequestBody>("STORAGE", "SOCIAL_TASK", _body, _actionType);
        return JsonConvert.SerializeObject(command);
    }
}
