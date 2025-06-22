using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChange : MonoBehaviour
{
    [SerializeField] private ShopButtonTab tab;
    [SerializeField] private Image m_ButtonImage;
    [SerializeField] private Sprite m_SelectedSprite;
    [SerializeField] private Sprite m_Deselectedprite;

    private ICallback.CallFunc2<ShopButtonTab> onButtonSelected;
    public ButtonSpriteChange SetOnButtonSelected(ICallback.CallFunc2<ShopButtonTab> func) { onButtonSelected = func; return this; }

    private Button tabButton;
    private void Awake()
    {
        tabButton = GetComponent<Button>();
        m_ButtonImage.sprite = m_Deselectedprite;
        tabButton.onClick.AddListener(OnButtonSelected);
    }

    private void OnEnable()
    {
        //ShopManager.instance.onTabSelected += OnChangeButtonSprite;
    }

    private void OnDisable()
    {
        //ShopManager.instance.onTabSelected -= OnChangeButtonSprite;
    }

    private void OnButtonSelected()
    {
        onButtonSelected?.Invoke(tab);
    }

    public void OnChangeButtonSprite(ShopButtonTab tab)
    {
        m_ButtonImage.sprite = this.tab == tab ? m_SelectedSprite : m_Deselectedprite;
    }
}
