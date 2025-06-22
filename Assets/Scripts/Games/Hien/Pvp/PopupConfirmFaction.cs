using Game;
using Game.Websocket;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirmFaction : BaseView
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Desc;
    [SerializeField] private Button m_ConfirmBtn;
    [SerializeField] private Button m_CancelBtn;
    private int teamId;

    protected override void OnViewShown()
    {
        m_ConfirmBtn.onClick.AddListener(OnClickConfirm);
        m_CancelBtn.onClick.AddListener(Hide);
    }

    protected override void OnViewHidden()
    {
        m_ConfirmBtn.onClick.RemoveListener(OnClickConfirm);
        m_CancelBtn.onClick.RemoveListener(Hide);
    }

    public void InitData(int index)
    {
        teamId = index;
        m_Title.text = "Choose " + ((index == 0) ? "Tongo" : "Ochi");
        m_Desc.text = "Are you sure you want to choose for team " + ((index == 0) ? "Tongo" : "Ochi") + "?";
    }

    public void OnClickConfirm()
    {
        WebSocketRequestHelper.SelectFactionOnce((teamId == 0) ? "tongo" : "ochi", (profile) =>
        {
            PlayerData.Instance.SetPvpProfile(profile);
            GetUIView<UIHeaderFooterOnly>().OnOpenPvP();
            Hide();
            //this.gameObject.SetActive(false);
        });
    }
}
