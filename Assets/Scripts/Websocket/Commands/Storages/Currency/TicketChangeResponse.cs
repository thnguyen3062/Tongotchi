using Newtonsoft.Json;
using UnityEngine;

public class TicketChangeResponse 
{
    public bool success;
    public string message;
    public int tickets;
    public int diamonds;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
