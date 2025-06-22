using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;


namespace Game.Websocket.Commands.Pet
{
    public class ClaimFusionCommand : IWebSocketCommand
    {
        private readonly string _actionType;
        private readonly RequestBody _body;
        private class RequestBody
        {
            public string telegram_code;
            public string fusionId;

            public RequestBody(string telegram_code, string fusion_id)
            {
                this.telegram_code = telegram_code;
                this.fusionId = fusion_id;
            }
        }

        public ClaimFusionCommand(string actionType, string telegramCode, string fusionId)
        {
            _actionType = actionType;
            _body = new RequestBody(telegramCode, fusionId);
        }

        public string ToJson()
        {
            var command = new SendCommand<RequestBody>("PET", "FUSION_CLAIM", _body, _actionType);
            return JsonConvert.SerializeObject(command);
        }
    }
}

public class FusionStartResult
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("fusion_id")]
    public string FusionId { get; set; }

    [JsonProperty("completion_time")]
    public DateTime CompletionTime { get; set; }

    [JsonProperty("wait_minutes")]
    public double WaitMinutes { get; set; }

    [JsonProperty("success_chance")]
    public double SuccessChance { get; set; }

    [JsonProperty("pets_used")]
    public List<int> PetsUsed { get; set; }

    [JsonProperty("potions_used")]
    public int PotionsUsed { get; set; }

    //----------ERROR_HANDLER----------//

    [JsonProperty("error_code")]
    public string ErrorCode { get; set; }
    [JsonProperty("error_message")]
    public string ErrorMessage { get; set; }
}

public class FusionClaimResult
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("fusion_success")]
    public bool FusionSuccess { get; set; }

    [JsonProperty("new_pet")]
    public Pet NewPet { get; set; }

    [JsonProperty("fusion_xp_bonus")]
    public int FusionXpBonus { get; set; }

    [JsonProperty("fusion_pets_count")]
    public int FusionPetsCount { get; set; }

    //----------ERROR_HANDLER----------//

    [JsonProperty("error_code")]
    public string ErrorCode { get; set; }
    [JsonProperty("error_message")]
    public string ErrorMessage { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class Pet
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }
}