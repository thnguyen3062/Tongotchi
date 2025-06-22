using Game;
using UnityEngine;

public class PopupLevel15Up : PopupLevelupReward
{
    public override void ClaimLevelReward()
    {
        base.ClaimLevelReward();
        GameManager.Instance.PetController.InitPet(true);
    }
}
