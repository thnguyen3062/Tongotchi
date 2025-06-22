using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChangeShopPvp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PvpShopTab tab;
    [SerializeField] private Image m_ButtonImage;
    [SerializeField] private Sprite m_SelectedSprite;
    [SerializeField] private Sprite m_Deselectedprite;

    private ICallback.CallFunc2<PvpShopTab> onButtonSelected;
    public ButtonChangeShopPvp SetOnButtonSelected(ICallback.CallFunc2<PvpShopTab> func) { onButtonSelected = func; return this; }

    private Button tabButton;

    private void OnEnable()
    {
        if (!tabButton) tabButton = GetComponent<Button>();
        m_ButtonImage.sprite = m_Deselectedprite;

        if (tabButton) tabButton.onClick.AddListener(OnButtonSelected);
    }

    private void OnDisable()
    {
        if (tabButton) tabButton.onClick.RemoveListener(OnButtonSelected);
    }

    private void OnButtonSelected()
    {
        onButtonSelected?.Invoke(tab);
    }

    public void OnChangeButtonSprite(PvpShopTab tab)
    {
        m_ButtonImage.sprite = this.tab == tab ? m_SelectedSprite : m_Deselectedprite;
    }
}
