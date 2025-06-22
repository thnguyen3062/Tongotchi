using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Game
{
    public class UIFusionSuccessPopup : BasePetPopup
    {
        [SerializeField] private TextMeshProUGUI m_PetNameTmp;
        [SerializeField] private TextMeshProUGUI m_PetLevelTmp;
        [SerializeField] private Button m_ConfirmBtn;

        private SkeletonGraphic m_PetPreviewSkeleton;
        private GamePetData pet;

        protected override void OnViewShown()
        {
            m_ConfirmBtn.onClick.AddListener(ConfirmAndClose);
        }

        protected override void OnViewHidden()
        {
            m_ConfirmBtn.onClick.RemoveListener(ConfirmAndClose);
        }

        public void LoadFusionedPet(GamePetData petData)
        {
            this.pet = petData;
            m_PetPreviewSkeleton = InstantiatePetPreview(pet);
            LoadPetPvPStats();
        }

        private void ConfirmAndClose()
        {
            Hide();
        }
    }
}