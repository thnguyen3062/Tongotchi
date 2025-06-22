using Core.Utils;
using Game;
using PathologicalGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevelupReward : BaseView
{
    [SerializeField] protected TextMeshProUGUI m_DetailTmp;
    [SerializeField] protected TextMeshProUGUI m_LevelText;
    [SerializeField] protected TextMeshProUGUI m_RewardText;
    [SerializeField] private Button m_ClaimBtn;

    private LevelRewardInfo rewardInfo;

    protected override void OnViewShown()
    {
        if (m_ClaimBtn) m_ClaimBtn.onClick.AddListener(ClaimLevelReward);
    }

    protected override void OnViewHidden()
    {
        if (m_ClaimBtn) m_ClaimBtn.onClick.RemoveListener(ClaimLevelReward);
    }

    public virtual void InitDataLevelUpReward()
    {
        SoundManager.Instance.PlayVFX("14. Level Up");
        int petLevel = PlayerData.Instance.PetData.petLevel;
        m_LevelText.text = (petLevel - 1).ToString() + "->" + petLevel.ToString();
        rewardInfo = PlayerData.Instance.GetLevelRewardInfo();
        string txt = "";
        if (rewardInfo.TicketsReward != 0)
            txt += "<sprite=0>" + rewardInfo.TicketsReward + " Tickets\n";
        if (rewardInfo.DiamondsReward != 0)
            txt += "<sprite=2>" + rewardInfo.DiamondsReward + " Diamonds\n";
        if (rewardInfo.EggReward != 0)
            txt += rewardInfo.EggReward + " Eggs";
        m_RewardText.text = txt;
    }

    public virtual void ClaimLevelReward()
    {
        SoundManager.Instance.PlayVFX("05. Claim Gift");
        PlayerData.Instance.AddCurrency(CurrencyType.Ticket, rewardInfo.TicketsReward);
        PlayerData.Instance.AddCurrency(CurrencyType.Diamond, rewardInfo.DiamondsReward);

        for (int i = 0; i < rewardInfo.EggReward; i++)
        {
            if (rewardInfo.EggReward != 0 && !PlayerData.Instance.MaxPet)
            {
                PlayerData.Instance.AddNewPetSlot((success, newPet) =>
                {
                    if (!success)
                    {
                        return;
                    }
                });
            }
            else
            {
                PlayerData.Instance.AddCurrency(CurrencyType.Diamond, 2);
            }
        }
        Hide();
        //PoolManager.Pools["Popup"].Despawn(this.transform);
    }
}
