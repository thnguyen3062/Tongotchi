using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevelUpFailed : PopupLevelupReward
{
    private ICallback.CallFunc OnTryAgain;

    [SerializeField] protected TextMeshProUGUI m_ItemLostText;
    [SerializeField] private Button m_ConfirmBtn;
    [SerializeField] private Button m_TryAgainBtn;

    public PopupLevelUpFailed SetTryAgainCallback(ICallback.CallFunc func) { OnTryAgain = func; return this; }

    protected override void OnViewShown()
    {
        base.OnViewShown();
        m_ConfirmBtn.onClick.AddListener(OnConfirmRewardFailed);
        m_TryAgainBtn.onClick.AddListener(OnClickTryAgain);
    }

    protected override void OnViewHidden()
    {
        base.OnViewHidden();
        m_ConfirmBtn.onClick.RemoveListener(OnConfirmRewardFailed);
        m_TryAgainBtn.onClick.RemoveListener(OnClickTryAgain);
    }

    public void InitFusionFailed(string errorMessage, int itemSpent)
    {
        m_DetailTmp.text = "Penalty";
        m_LevelText.text = $"{errorMessage}";
        m_TryAgainBtn.gameObject.SetActive(false);
    }

    public void InitDataEvolveFailed()
    {
        int petLevel = PlayerData.Instance.PetData.petLevel + 1;
        m_LevelText.text = (petLevel).ToString() + "->" + (petLevel + 1).ToString();
        int itemLost = 0;
        if (petLevel == GameUtils.FIRST_EVOLVE_LEVEL)
            itemLost = GameUtils.FIRST_EVOLVE_ITEM;
        else if (petLevel == GameUtils.SECOND_EVOLVE_LEVEL)
            itemLost = GameUtils.SECOND_EVOLVE_ITEM;
        else if (petLevel == GameUtils.THIRD_EVOLVE_LEVEL)
            itemLost = GameUtils.THIRD_EVOLVE_ITEM;
        m_ItemLostText.text = "x" + itemLost.ToString();
        m_TryAgainBtn.gameObject.SetActive(true);
    }

    public void OnClickTryAgain()
    {
        SoundManager.Instance.PlayVFX("2. Screen Touch");
        OnTryAgain?.Invoke();
        Hide();
    }

    public void OnConfirmRewardFailed()
    {
        Hide();
    }
}
