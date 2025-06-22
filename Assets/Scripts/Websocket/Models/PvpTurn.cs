namespace Game.Websocket.Model
{
    public struct PvpTurn
    {
        public string from;
        public string to;
        public float speed;
        public float damage;
        public bool critical;
        public float attacker_hp;
        public float defender_hp;
    }
}