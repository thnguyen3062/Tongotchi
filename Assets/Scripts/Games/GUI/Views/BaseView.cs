using UnityEngine;

namespace Game
{
    public enum ViewOrder
    {
        None,
        First,
        Last,
    }

    public class BaseView : MonoBehaviour
    {
        [SerializeField] private bool isPopup;
        [SerializeField] private bool isOverlayUI;
        private UIManager manager;

        public bool IsShown { get; private set; }
        public bool IsPopup => isPopup;
        public bool IsOverlayUI => isOverlayUI;
        protected UIManager GUIManager => manager;

        public void Setup(UIManager uiManager)
        {
            this.manager = uiManager;
            OnSetup();
        }

        public void Show(ViewOrder order = ViewOrder.None, bool force = false) 
        {
            if (!IsShown || force)
            {
                IsShown = true;
                gameObject.SetActive(true);
                if (order == ViewOrder.First) transform.SetAsFirstSibling();
                if (order == ViewOrder.Last) transform.SetAsLastSibling();
                OnViewShown();
            }
        }

        public void Hide()
        {
            if (IsShown || gameObject.activeSelf)
            {
                IsShown = false;
                gameObject.SetActive(false);
                OnViewHidden();
            }
        }

        /// <summary>
        /// Invoke when UIManager started
        /// </summary>
        protected virtual void OnSetup() { }
        protected virtual void OnViewShown() { }
        protected virtual void OnViewHidden() { }

        protected T ShowUIView<T>(ViewOrder order = ViewOrder.None) where T : BaseView
        {
            return manager.ShowView<T>(order);
        }

        protected void HideUIView<T>() where T : BaseView
        {
            manager.HideView<T>();
        }

        protected T GetUIView<T>() where T : BaseView
        {
            return manager.GetView<T>();
        }
    }
}