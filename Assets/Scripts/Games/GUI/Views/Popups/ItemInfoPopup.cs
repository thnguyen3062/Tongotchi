using Core.Utils;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Game
{
    public class ItemInfoPopup : BaseView
    {
        [SerializeField] private Image m_ItemImage;
        [SerializeField] private TextMeshProUGUI m_ItemName;
        [SerializeField] private TextMeshProUGUI m_Type;
        [SerializeField] private TextMeshProUGUI m_OwnedCount;
        [SerializeField] private TextMeshProUGUI m_ItemInfo;
        [SerializeField] private TextMeshProUGUI m_Price;
        [SerializeField] private Button m_Close;

        protected int id;
        protected int ownedCount;
        protected ICallback.CallFunc onFailedToUseCallback;

        protected override void OnViewShown()
        {
            m_Close.onClick.AddListener(OnClose);
        }

        protected override void OnViewHidden()
        {
            m_Close.onClick.RemoveListener(OnClose);
        }

        private void OnClose()
        {
            Hide();
        }

        public virtual void SetInfo(int id, string itemName, ItemCategory type, int ownedCount, string itemInfo, float price, float value, ICallback.CallFunc onFailToUse = null)
        {
            this.id = id;
            this.ownedCount = ownedCount;
            m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_" + id];
            m_ItemName.text = itemName;

            string itemType = "";
            if (type == ItemCategory.Food)
                itemType = "Food";
            else if (type == ItemCategory.Medicine)
                itemType = "Medicine";
            else if (type == ItemCategory.Toy)
                itemType = "Toy";
            m_Type.text = "Category: " + itemType;

            m_OwnedCount.text = "Owned: " + ownedCount.ToString();
            m_ItemInfo.text = string.Format(itemInfo, value);

            onFailedToUseCallback = onFailToUse;

            m_Price.text = "Price: " + price;

            /*

            */
        }
    }
}