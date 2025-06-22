using Game.Websocket;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game 
{
    public class UIHeaderFooterOnly : BaseView
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI m_DiamondTmp;
        [SerializeField] private TextMeshProUGUI m_TicketTmp;
        [SerializeField] private Button m_SoundToggleBtn;
        [SerializeField] private TextMeshProUGUI m_PetLevelTmp;
        [SerializeField] private Image m_PetExpbar;
        [SerializeField] private Button m_SelectPetBtn;
        [SerializeField] private Sprite[] m_SoundSprites;
        [Header("Footer")]
        [SerializeField] private Sprite m_InactiveTab;
        [SerializeField] private Sprite m_ActiveTab;
        [SerializeField] private Button m_HomeBtn;
        [SerializeField] private Button m_SocialBtn;
        [SerializeField] private Button m_PvpBtn;
        [SerializeField] private Button m_MingameBtn;

        private UIMainHomePanel homePanel;

        public UIMainHomePanel HomePanel
        {
            get
            {
                if (homePanel == null)
                {
                    homePanel = GetUIView<UIMainHomePanel>();
                }
                return homePanel;
            }
        }

        protected override void OnSetup()
        {
            GameManager.Instance.PetController.OnExpChange += OnUpdateExp;
            PlayerData.Instance.OnCurrencyChange += OnUpdateCurrency;
            SoundManager.Instance.AddSoundChangeCallback(OnSoundChange);
        }

        protected override void OnViewShown()
        {
            //Header
            m_SoundToggleBtn.onClick.AddListener(ToggleSound);
            m_SelectPetBtn.onClick.AddListener(ShowSelectPetPopup);

            if (PlayerData.Instance.data.selectedPetID != -1)
            {
                SetUILevel(PlayerData.Instance.PetData.petLevel);
                PlayerData.Instance.GetCurrency(CurrencyType.Diamond);
                PlayerData.Instance.GetCurrency(CurrencyType.Ticket);
            }

            // Footer
            m_HomeBtn.onClick.AddListener(GoToHome);
            m_SocialBtn.onClick.AddListener(GoToSocial);
            m_PvpBtn.onClick.AddListener(GoToTvt);
            m_MingameBtn.onClick.AddListener(GoToMinigame);
        }

        protected override void OnViewHidden()
        {
            m_SelectPetBtn.onClick.RemoveListener(ShowSelectPetPopup);
            m_SoundToggleBtn.onClick.RemoveListener(ToggleSound);

            // Footer
            m_HomeBtn.onClick.RemoveListener(GoToHome);
            m_SocialBtn.onClick.RemoveListener(GoToSocial);
            m_PvpBtn.onClick.RemoveListener(GoToTvt);
            m_MingameBtn.onClick.RemoveListener(GoToMinigame);
        }

        #region Header
        private void OnUpdateExp(float value, float maxValue, bool needEvolved)
        {
            Debug.Log($"Exp: {value}\nMaxValue: {maxValue}");
            if (!needEvolved)
            {
                m_PetExpbar.fillAmount = value / maxValue;
            }
            else
            {
                m_PetExpbar.fillAmount = 1;
            }
        }

        private void OnUpdateCurrency(CurrencyType type, int value)
        {
            switch (type)
            {
                case CurrencyType.Diamond:
                    m_DiamondTmp.text = GameUtils.FormatCurrency(value);
                    break;
                case CurrencyType.Ticket:
                    m_TicketTmp.text = GameUtils.FormatCurrency(value);
                    break;
            }
        }

        public void SetUILevel(int level)
        {
            m_PetLevelTmp.text = level.ToString();
        }

        private void ShowSelectPetPopup()
        {
            if (HomePanel.gameObject.activeSelf)
            {
                ShowUIView<UISelectPetPopup>().LoadPetsPopup(UISelectPetPopup.UIPetType.All);
            }
        }

        private void ToggleSound()
        {
            SoundManager.Instance.ToggleSoundSetting();
        }

        private void OnSoundChange(bool isOn)
        {
            PlayerData.RequestSetSound(isOn);
            m_SoundToggleBtn.image.sprite = m_SoundSprites[isOn ? 1 : 0];
        }
        #endregion

        #region Footer
        private void ClickTab(BottomButtonType tab)
        {
            HideUIView<PVPController>();
            HideUIView<UINormalInventory>();
            HideUIView<ShopManager>();
            HideUIView<ShopManager>();
            HideUIView<UISetupFusionPopup>();
            HideUIView<UISetupFusionPopup>();

            switch (tab)
            {
                case BottomButtonType.Home:
                    ShowUIView<UIMainHomePanel>().ShowHomeTab();
                    break;
                case BottomButtonType.Social:
                    ShowUIView<UIMainHomePanel>().ShowSocialTab();
                    break;
                case BottomButtonType.PVP:
                    GoToTvt();
                    break;
                case BottomButtonType.Minigame:
                    ShowUIView<UIMainHomePanel>().ShowSocialTab(UISocialBoard.BoardContentType.Minigame);
                    break;
            }
            GameManager.Instance.PetController.gameObject.SetActive(true);
        }

        private void GoToHome()
        {
            ResetTabs();
            ClickTab(BottomButtonType.Home);
            m_HomeBtn.image.sprite = m_ActiveTab;
        }

        private void GoToSocial()
        {
            ResetTabs();
            ClickTab(BottomButtonType.Social);
            m_SocialBtn.image.sprite = m_ActiveTab;
        }

        private void GoToMinigame()
        {
            ResetTabs();
            ClickTab(BottomButtonType.Minigame);
            m_MingameBtn.image.sprite = m_ActiveTab;
        }

        private void GoToTvt()
        {
            ResetTabs();
            if (CheckPvPAvailable())
            {
                StartCoroutine(DelayOpenPvp());
            }
            else
            {
                Toast.Show("      Must have at least 1 pet level 15 min to play PvP     ");
            }
            m_PvpBtn.image.sprite = m_ActiveTab;
        }

        private bool CheckPvPAvailable()
        {
            //if (!requireLv15) return true;

            List<SavedPetData> ownedPetList = PlayerData.Instance.data.listOwnedPet;
            int petCount = PlayerData.Instance.userInfo?.pets?.Count ?? 0;

            if (ownedPetList == null || ownedPetList.Count == 0)
            {
                if (PlayerData.Instance.data.selectedPetID == -1)
                {
                    return false;
                }
                else if (PlayerData.Instance.PetData != null && PlayerData.Instance.PetData.petLevel >= 15)
                {
                    return true;
                }
            }

            bool hasAvailablePet = ownedPetList.Any(pet => pet.petLevel >= 15);

            return hasAvailablePet;
        }

        private IEnumerator DelayOpenPvp()
        {
            ShowUIView<UILoadingView>();

            yield return new WaitForSeconds(0.5f);

            OnOpenPvP();

            yield return new WaitForSeconds(0.5f);

            HideUIView<UILoadingView>();
        }

        public void OnOpenPvP()
        {
            WebSocketRequestHelper.GetPvpProfileOnce((profile) =>
            {
                if (!string.IsNullOrEmpty(profile.error_code))
                {
                    if (profile.error_code.Equals("912"))
                    {
                        HideUIView<PVPController>();
                        ShowUIView<UIChooseFactionPanel>();
                        //m_ChooseFactionPopup.gameObject.SetActive(true);
                    }
                }
                else
                {
                    HideUIView<UIMainHomePanel>();
                    ShowUIView<PVPController>().InitDataMainPvp();
                    //m_PvpMainUI.GetComponent<PVPController>().InitDataMainPvp();
                    HideUIView<UIChooseFactionPanel>();
                    PlayerData.Instance.SetPvpProfile(profile);
                }
            });
        }
        #endregion

        private void ResetTabs()
        {
            m_HomeBtn.image.sprite = m_InactiveTab;
            m_SocialBtn.image.sprite = m_InactiveTab;
            m_PvpBtn.image.sprite = m_InactiveTab;
            m_MingameBtn.image.sprite = m_InactiveTab;
        }
    }
}