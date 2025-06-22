using System;
using UnityEngine;

namespace Game.UI.InteractButton
{
    public class UIInteractButtonHandler : MonoBehaviour
    {
        [SerializeField] private InteractButtonHandler m_FeedBtn;
        [SerializeField] private InteractButtonHandler m_ShowerBtn;
        [SerializeField] private InteractButtonHandler m_CureBtn;
        [SerializeField] private InteractButtonHandler m_ToyBtn;
        [SerializeField] private InteractButtonHandler m_SleepBtn;
        [SerializeField] private InteractButtonHandler m_CleanBtn;

        public static Action<ActionType> OnButtonInteracted;

        private void Start()
        {
            AssignCallback();
            SetButtonCallback();
        }

        private void OnDestroy()
        {
            UnassignCallback();
        }

        private void AssignCallback()
        {
            PetManager.OnSleep += LockAllInteract;
            PetManager.OnPetAwake += UnlockAllInteract;
        }

        private void UnassignCallback()
        {
            PetManager.OnSleep -= LockAllInteract;
            PetManager.OnPetAwake -= UnlockAllInteract;
        }

        private void SetButtonCallback()
        {
            m_FeedBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
            m_ShowerBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
            m_CureBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
            m_ToyBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
            m_SleepBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
            m_CleanBtn.SetOnInteractButtonClicked(OnActionButtonClicked);
        }

        private void OnActionButtonClicked(ActionType callback)
        {
            OnButtonInteracted?.Invoke(callback);
        }

        private void LockAllInteract()
        {
            if (GameManager.instance.PetManager.LockActionsWhenSleep)
            {
                m_CleanBtn.Interactable(false);
                m_FeedBtn.Interactable(false);
                m_ShowerBtn.Interactable(false);
            }
        }

        private void UnlockAllInteract()
        {
            m_CleanBtn.Interactable(true);
            m_FeedBtn.Interactable(true);
            m_ShowerBtn.Interactable(true);
        }
    }
}