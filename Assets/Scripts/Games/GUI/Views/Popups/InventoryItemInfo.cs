using Core.Utils;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class InventoryItemInfo : ItemInfoPopup
    {
        private Action onUseItemSuccess;

        [SerializeField] private Button m_UseBtn;

        private ItemCategory _itemType;
        private string _itemName;

        public InventoryItemInfo SetUseItemSuccessCallback(Action action)
        {
            onUseItemSuccess = action;
            return this;
        }

        protected override void OnViewShown()
        {
            base.OnViewShown();
            m_UseBtn.onClick.AddListener(OnConfirm);
        }

        protected override void OnViewHidden()
        {
            base.OnViewHidden();
            m_UseBtn.onClick.RemoveListener(OnConfirm);
            onUseItemSuccess = null;
        }

        public override void SetInfo(int id, string itemName, ItemCategory type, int ownedCount, string itemInfo, float price, float value, ICallback.CallFunc onFailToUse = null)
        {
            base.SetInfo(id, itemName, type, ownedCount, itemInfo, price, value, onFailToUse);
            _itemType = type;
            _itemName = itemName;
            if (m_UseBtn != null)
            {
                bool canInteract = GameManager.Instance.PetController.CarePetCondition(PetController.ToActionType(type), type == ItemCategory.Medicine, out string errorMessage);
                m_UseBtn.interactable = canInteract;
            }
        }

        private void OnConfirm()
        {
            SoundManager.Instance.PlayVFX("11. Buy Item");
            if (ownedCount >= 1)
            {
                /*
                 * Flow:
                 * - If item is food, send feed request
                 * - If item is medicine, send cure request
                 * - If item is toy, send play toy requset
                 * - When client receive the callback, invoke an action.
                 */
                //UIManager.instance.OnUseItem(id);
                GameManager.Instance.UseItemForPet(id, _itemType, false, () => {
                    FirebaseAnalytics.instance.LogCustomEvent("user_use_item", JsonConvert.SerializeObject(new CustomEventWithVariable(_itemName)));
                    onUseItemSuccess?.Invoke();
                    Hide();
                });
            }
            else
            {
                onFailedToUseCallback?.Invoke();
            }
        }
    }
}