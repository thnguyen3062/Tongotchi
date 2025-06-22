using System;

namespace Game.Websocket.Model
{
    public struct PVPProfileResponse
    {
        public string error_code;
        public string error_message;

        public string faction;
        public string first_name;
        public int match_count;
        public int ranking_point;
        public DateTime start_date;
        public DateTime end_date;
        public int today_attack_count;
        public bool today_reset_attack;
    }
}