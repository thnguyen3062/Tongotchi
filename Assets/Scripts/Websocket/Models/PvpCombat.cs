namespace Game.Websocket.Model
{
    public struct PvpCombat
    {
        public GamePetData attacker;
        public GamePetData defender;
        public PvpTurn[] turns;
        public int attacker_inc_ranking_point;
    }
}