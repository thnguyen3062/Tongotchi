using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemHandler : MonoBehaviour
{
    private int id;
    [SerializeField] private Image m_ButtonImage;
    [SerializeField] private Sprite m_PlusSprite;
    [SerializeField] private Sprite m_BorderSprite;
    [SerializeField] private Image m_ItemImage;
    [SerializeField] private TextMeshProUGUI m_ItemCount;
    private bool isPlus;
    private Button m_InventoryItemButton;
    private ICallback.CallFunc3<int, bool> onInventoryItemClick;

    public InventoryItemHandler SetOnInventoryItemClick(ICallback.CallFunc3<int, bool> func) { onInventoryItemClick = func; return this; }

    private void Start()
    {
        m_InventoryItemButton = GetComponent<Button>();
        m_InventoryItemButton.onClick.AddListener(OnClick);
    }

    public void InitItem(int id, int count, bool isPlus = false)
    {
        this.isPlus = isPlus;
        if (!isPlus)
        {
            this.id = id;
            m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id]; ;
            m_ItemCount.text = count.ToString();
            m_ButtonImage.sprite = m_BorderSprite;
            m_ItemImage.gameObject.SetActive(true);
            m_ItemCount.gameObject.SetActive(true);
        }
        else
        {
            m_ButtonImage.sprite = m_PlusSprite;
            m_ItemImage.gameObject.SetActive(false);
            m_ItemCount.gameObject.SetActive(false);
        }
    }

    public void InitSpecialItem(int id, int count, float duration)
    {
        isPlus = false;
        this.id = id;
        m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id];
        if (id == GameUtils.ROBOT_BOOST || id == GameUtils.TICKET_POTION_BOOST)
        {
            m_ItemCount.text = $"{PlayerData.Instance.Items[id].itemName}\nDuration: {GameUtils.ConvertSecondsToTimeString((int)duration)}";
        }
        else
        {
            m_ItemCount.text = $"{PlayerData.Instance.Items[id].itemName}\nOwned: {count}";
        }
    }

    private void OnClick()
    {
        onInventoryItemClick?.Invoke(id, isPlus);
    }
}
