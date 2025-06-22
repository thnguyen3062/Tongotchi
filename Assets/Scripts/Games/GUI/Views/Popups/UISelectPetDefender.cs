using Game.Websocket;
using UnityEngine;

public class SelectPetDefender : SelectPetPVP
{
    protected override void OnConfirm()
    {
        WebSocketRequestHelper.SetDefensePet(currentPetId);
        Toast.Show("Set Defense Pet Successfully");
        base.OnConfirm();
    }
}
