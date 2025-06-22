using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PopupEvolveFailed : PopupEvolve
    {
        [SerializeField] TextMeshProUGUI m_OwnedItemText;
        [SerializeField] TextMeshProUGUI m_MissingItemText;
        [SerializeField] private Button m_GoToShopBtn;

        protected override void OnViewShown()
        {
            base.OnViewShown();
            m_GoToShopBtn.onClick.AddListener(OnGoToShop);
        }

        protected override void OnViewHidden()
        {
            base.OnViewHidden();
            m_GoToShopBtn.onClick.RemoveListener(OnGoToShop);
        }

        public void OnInitDataEvolveFailed()
        {
            m_ItemImage.sprite = PlayerData.Instance.GameItemSpriteDict["Item_28"];
            m_OwnedItemText.text = "Own " + PlayerData.Instance.GetOwnedItemEvolve();
            int itemMissing = 0;
            int petLevel = PlayerData.Instance.PetData.petLevel + 1;
            if (petLevel == GameUtils.FIRST_EVOLVE_LEVEL)
                itemMissing = GameUtils.FIRST_EVOLVE_ITEM - PlayerData.Instance.GetOwnedItemEvolve();
            else if (petLevel == GameUtils.SECOND_EVOLVE_LEVEL)
                itemMissing = GameUtils.SECOND_EVOLVE_ITEM - PlayerData.Instance.GetOwnedItemEvolve();
            else if (petLevel == GameUtils.THIRD_EVOLVE_LEVEL)
                itemMissing = GameUtils.THIRD_EVOLVE_ITEM - PlayerData.Instance.GetOwnedItemEvolve();
            m_MissingItemText.text = "You need " + itemMissing + " more.";
        }

        public void OnGoToShop()
        {
            ShowUIView<ShopManager>().ShowTab(ShopButtonTab.Subscribe);
            Hide();
            //PoolManager.Pools["Popup"].Despawn(this.transform);
        }
    }
}