using Core.Utils;
using Game;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupEvolve : BaseView
{
    [SerializeField] TextMeshProUGUI m_LevelText;
    [SerializeField] TextMeshProUGUI m_RewardText;
    [SerializeField] TextMeshProUGUI m_ItemSpentText;
    [SerializeField] TextMeshProUGUI m_ChanceText;
    [SerializeField] protected Image m_ItemImage;
    [SerializeField] private Button m_ConfirmBtn;
    [SerializeField] private Button m_CancelBtn;

    private ICallback.CallFunc2<EvolveResultBody> onConfirm;
    private ICallback.CallFunc onGoToShop;

    public PopupEvolve SetOnConfirmEvolveCallback(ICallback.CallFunc2<EvolveResultBody> func) { onConfirm = func; return this; }
    public PopupEvolve SetOnGoToShopCallback(ICallback.CallFunc func) { onGoToShop = func; return this; }

    private bool canEvolve = false;
    private bool isSuccess = false;
    private int itemsRequire = 0;
    private LevelRewardInfo rewardInfo;

    protected override void OnViewShown()
    {
        base.OnViewShown();
        if (m_ConfirmBtn) m_ConfirmBtn.onClick.AddListener(OnConfirm);
        if (m_CancelBtn) m_CancelBtn.onClick.AddListener(OnCancel);
    }

    protected override void OnViewHidden()
    {
        base.OnViewHidden();
        if (m_ConfirmBtn) m_ConfirmBtn.onClick.RemoveListener(OnConfirm);
        if (m_CancelBtn) m_CancelBtn.onClick.RemoveListener(OnCancel);
    }

    public void InitDataEvolve()
    {
        m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_28"];
        SoundManager.Instance.PlayVFX("15. Evovle");
        int petLevel = PlayerData.Instance.PetData.petLevel;

        if (petLevel >= 37) petLevel = GameUtils.THIRD_EVOLVE_LEVEL;

        else if (petLevel >= 25) petLevel = GameUtils.SECOND_EVOLVE_LEVEL;

        else petLevel = GameUtils.FIRST_EVOLVE_LEVEL;

        m_LevelText.text = (petLevel).ToString() + "->" + (petLevel + 1).ToString();
        Debug.Log(petLevel);
        rewardInfo = PlayerData.Instance.GetLevelRewardEvolveInfo();
        string info = "";
        if (rewardInfo.TicketsReward != 0)
            info += rewardInfo.TicketsReward + " Tickets\n";
        if (rewardInfo.DiamondsReward != 0)
            info += rewardInfo.DiamondsReward + " Diamonds\n";
        if (rewardInfo.EggReward != 0)
            info += rewardInfo.EggReward + " Eggs";
        m_RewardText.text = info;
        ;
        int chance = 100;
        if (petLevel == GameUtils.FIRST_EVOLVE_LEVEL)
        {
            m_ItemSpentText.text = "x" + GameUtils.FIRST_EVOLVE_ITEM;
            m_ChanceText.text = GameUtils.FIRST_EVOLVE_CHANCE + "%";
            chance = GameUtils.FIRST_EVOLVE_CHANCE;
            itemsRequire = GameUtils.FIRST_EVOLVE_ITEM;
        }
        else if (petLevel == GameUtils.SECOND_EVOLVE_LEVEL)
        {
            m_ItemSpentText.text = "x" + GameUtils.SECOND_EVOLVE_ITEM;
            m_ChanceText.text = GameUtils.SECOND_EVOLVE_CHANCE + "%";
            chance = GameUtils.FIRST_EVOLVE_CHANCE;
            itemsRequire = GameUtils.SECOND_EVOLVE_ITEM;
        }
        else if (petLevel == GameUtils.THIRD_EVOLVE_LEVEL)
        {
            m_ItemSpentText.text = "x" + GameUtils.THIRD_EVOLVE_ITEM;
            m_ChanceText.text = GameUtils.THIRD_EVOLVE_CHANCE + "%";
            chance = GameUtils.FIRST_EVOLVE_CHANCE;
            itemsRequire = GameUtils.THIRD_EVOLVE_ITEM;
        }
        canEvolve = (PlayerData.Instance.GetOwnedItemEvolve() >= itemsRequire) ? true : false;
        isSuccess = RandomChance(chance);
        //|| petLevel == GameUtils.SECOND_EVOLVE_LEVEL || petLevel == GameUtils.THIRD_EVOLVE_LEVEL
    }

    public void OnCancel()
    {
        SoundManager.Instance.PlayVFX("20. Cancel");
        Hide();
        //PoolManager.Pools["Popup"].Despawn(this.transform);
    }

    public void OnConfirm()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        GameManager.Instance.EvolvePet((EvolveResultBody result) => {
            if (result != null)
            {
                isSuccess = result.evolve;
                //canEvolve = result.exists;
                onConfirm?.Invoke(result);
                Hide();
            }
        });
        /*
        if (canEvolve)
        {
            GameManager.instance.EvolvePet((EvolveResultBody result, GamePetData petEntity) => {
                onConfirm?.Invoke(canEvolve, isSuccess);
            });
            //PlayerData.Instance.OnEvolve(itemsRequire, isSuccess);

            // Response:
            // FirebaseAnalytics.instance.LogCustomEvent($"pet_evolve_from{petEvolveLevel - 1} to {petEvolveLevel}");
        }
        */
        //PoolManager.Pools["Popup"].Despawn(this.transform);
    }

    private bool RandomChance(int chance)
    {
        int rnd = Random.Range(1, 101);
        if (rnd <= chance)
            return true;
        else
            return false;
    }
}
