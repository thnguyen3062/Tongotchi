using Game;
using UnityEngine;
using UnityEngine.UI;

public class UIChooseFactionPanel : BaseView
{
    [SerializeField] private Button m_OchiBtn;
    [SerializeField] private Button m_TongoBtn;

    protected override void OnViewShown()
    {
        m_OchiBtn.onClick.AddListener(Ochi);
        m_TongoBtn.onClick.AddListener(Tongo);
    }

    protected override void OnViewHidden()
    {
        m_OchiBtn.onClick.RemoveListener(Ochi);
        m_TongoBtn.onClick.RemoveListener(Tongo);
    }

    public void Tongo()
    {
        OnClickChooseTeam(0);
    }

    public void Ochi()
    {
        OnClickChooseTeam(1);
    }

    public void OnClickChooseTeam(int ind)
    {
        ShowUIView<PopupConfirmFaction>().InitData(ind);
    }
}
