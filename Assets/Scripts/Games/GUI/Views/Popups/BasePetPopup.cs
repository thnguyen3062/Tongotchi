using Game.Utils;
using Game.Websocket;
using Spine.Unity;
using System;
using UnityEngine;

namespace Game
{
    public abstract class BasePetPopup : BaseView
    {
        [SerializeField] protected Transform m_ContentField;
        [SerializeField] protected Transform m_PetPreview;
        [SerializeField] private PetPvPStatLoader m_StatLoader;

        protected void LoadPetPvPStats()
        {
            m_StatLoader.LoadStats();
        }

        protected SkeletonGraphic InstantiatePetPreview(GamePetData petData)
        {
            var previewTrans = Instantiate(m_PetPreview, m_ContentField);
            var skeleton = previewTrans.GetComponent<SkeletonGraphic>();
            InitSkeletonData(skeleton, petData);
            return skeleton;
        }

        protected void InitSkeletonData(SkeletonGraphic skel, GamePetData pet)
        {
            int id = PetAnimController.ClampId(pet.petId);

            skel.skeletonDataAsset = PetDataSO.Instance.basePetData.skeletonData[id];
            string skinName = string.Format(PetAnimController.BASE_PET_SPINE_SKIN, pet.petId + 1, pet.petEvolveLevel);
            skel.initialSkinName = skinName;
            skel.Initialize(true);
            skel.AnimationState.SetAnimation(0, "main_game/normal", true);
            skel.SetMaterialDirty();
        }

        public static void SelectPet(int petId, Action onReloadCompleted = null)
        {
            PlayerData.Instance.data.selectedPetID = petId;
            WebSocketRequestHelper.RequestChangePetId(PlayerData.Instance.data.selectedPetID, () =>
            {
                PlayerData.Instance.LoadSelectedPet(true, (success) =>
                {
                    if (success)
                    {
                        GameManager.Instance.PetController.AnimController.UnListenEvent();
                        GameManager.Instance.PetController.InitPet(petId);
                        onReloadCompleted?.Invoke();
                    }
                    else
                    {
                        Debug.LogError("Response failed. Check log for more detail!");
                    }
                });
            });
        }
    }
}