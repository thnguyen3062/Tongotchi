namespace Game.Websocket.Model
{
    public struct SendFactionData
    {
        public string faction;

        public SendFactionData(string faction)
        {
            this.faction = faction;
        }
    }
}