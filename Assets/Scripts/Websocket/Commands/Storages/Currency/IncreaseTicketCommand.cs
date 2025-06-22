using Game.Websocket.Interface;
using Game.Websocket.Model;
using Newtonsoft.Json;

public class CurrencyAmountPacket : UserData
{
    public int amount;
    public CurrencyAmountPacket(string telegramCode, int amount) : base(telegramCode)
    {
        this.telegramCode = telegramCode;
        this.amount = amount;
    }
}

public class IncreaseTicketCommand : IWebSocketCommand
{
    private readonly string _actionType;

    private CurrencyAmountPacket _data;

    public IncreaseTicketCommand(string actionType, string telegramCode, int amount)
    {
        _actionType = actionType;
        _data = new CurrencyAmountPacket(telegramCode, amount);
    }

    public string ToJson()
    {
        var command = new SendCommand<CurrencyAmountPacket>("STORAGE", "INCREASE_TICKET", _data, _actionType);
        return JsonConvert.SerializeObject(command);
    }
}
