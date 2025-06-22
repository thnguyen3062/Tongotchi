public class UISelectPetAttacker : SelectPetPVP
{
    protected override void OnConfirm()
    {
        if (PlayerData.Instance.PVPProfile.today_attack_count >= 5)
        {
            bool canBuyMore = !PlayerData.Instance.PVPProfile.today_reset_attack;
            //UIManager.instance.SpawnBuyAttackCountNotify(!PlayerData.Instance.PVPProfile.today_reset_attack);
            ShowUIView<PopupNotify>().Init("OUT OF ATTACK!",
            "You've out of attack count today!\n" + (canBuyMore ? "Do you want to use 1<sprite=2> to reset?" : ""),
            1, false,
            canBuyMore,
            2);
            return;
        }
        PlayerData.Instance.AddAttackCountPvpProfile(1);
        ShowUIView<UIFindMatchPanel>().StartFindMatch(currentPetId);
        base.OnConfirm();
    }
}
