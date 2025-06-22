namespace Game.Websocket.Model
{
    public struct SendCommand<T>
    {
        public string group;
        public string command;
        public T request;
        public string actionType;

        public SendCommand(string group, string command, T request, string actionType = "")
        {
            this.group = group;
            this.command = command;
            this.request = request;
            this.actionType = actionType;
        }
    }
}