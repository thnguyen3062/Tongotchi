using System.Collections;
using UnityEngine;

namespace Game.Websocket.Model
{
    public class PvpLeaderboardResponse
    {
        public PvpLeaderboard current;
        public PvpLeaderboard[] leaderboard;
    }
}