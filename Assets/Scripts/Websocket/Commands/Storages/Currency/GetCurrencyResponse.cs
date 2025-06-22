using UnityEngine;

public class GetCurrencyResponse 
{
    public bool success;
    public string telegram_code;
    public int tickets;
    public int diamond;

    public int GetCurrencyValue(CurrencyType currencyType)
    {
        if (currencyType == CurrencyType.Ticket)
        {
            return tickets;
        }
        return diamond;
    }
}
