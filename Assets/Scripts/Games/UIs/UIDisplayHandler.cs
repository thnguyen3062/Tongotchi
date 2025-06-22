using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Display
{
    public class UIDisplayHandler : MonoBehaviour
    {
        [Header("Currency")]
        [SerializeField] private TextMeshProUGUI m_DiamondText;
        [SerializeField] private TextMeshProUGUI m_TicketText;

        [Header("Status")]
        [SerializeField] private Image m_HappinessIcon;
        [SerializeField] private Sprite[] m_HappninessSprite;
        [SerializeField] private Image m_HappynessBar;
        [SerializeField] private Image m_HyegineBar;
        [SerializeField] private Image m_HungerBar;
        [SerializeField] private Image m_HealthBar;
        [SerializeField] private Image m_TimerBar;
        [SerializeField] private Image m_ExpBar;
        [SerializeField] private TextMeshProUGUI m_TimerText;
        [SerializeField] private TextMeshProUGUI m_LevelText;
        [SerializeField] private TextMeshProUGUI m_MinigameTicketText;
        [SerializeField] private TextMeshProUGUI m_TotalExpPointsText;

        public void InitUI()
        {
            PlayerData.Instance.OnCurrencyChange += OnUpdateCurrency;
            GameManager.instance.PetManager.StatusManager.onUpdateStatus += OnUpdateStatusUI;
            PlayerData.Instance.onLevelUp += OnPetLevelup;
            PlayerData.Instance.onUpdateExp += OnUpdateExp;
        }

        private void OnDestroy()
        {
            PlayerData.Instance.OnCurrencyChange -= OnUpdateCurrency;
            GameManager.instance.PetManager.StatusManager.onUpdateStatus -= OnUpdateStatusUI;
            PlayerData.Instance.onLevelUp -= OnPetLevelup;
            PlayerData.Instance.onUpdateExp -= OnUpdateExp;
        }

        private void OnUpdateCurrency(CurrencyType type, int value)
        {
            switch (type)
            {
                case CurrencyType.Diamond:
                    m_DiamondText.text = GameUtils.FormatCurrency(value);
                    break;
                case CurrencyType.Ticket:
                    m_TicketText.text = GameUtils.FormatCurrency(value);
                    break;
            }
        }

        public void UpdateTimerText(string text, float fillAmout)
        {
            m_TimerBar.fillAmount = fillAmout;
            m_TimerText.text = text;
        }

        public void UpdateLevelText(string text)
        {
            m_LevelText.text = text;
        }

        public void UpdateMinigameTicketText(string text)
        {
            m_MinigameTicketText.text = text;
        }

        public void OnUpdateStatusUI(StatusType type, float value)
        {
            float currentValue;
            switch (type)
            {
                case StatusType.Happyness:
                    currentValue = (float)value / GameUtils.MAX_HAPPYNESS_VALUE;
                    m_HappynessBar.fillAmount = currentValue;

                    if (currentValue >= 0.7f)
                    {
                        m_HappynessBar.color = GameUtils.HexToColor("#42bd41");
                        m_HappinessIcon.sprite = m_HappninessSprite[0];
                    }
                    else if (currentValue >= 0.3f)
                    {
                        m_HappynessBar.color = GameUtils.HexToColor("#ffc107");
                        m_HappinessIcon.sprite = m_HappninessSprite[1];
                    }
                    else
                    {
                        m_HappynessBar.color = GameUtils.HexToColor("#e51c23");
                        m_HappinessIcon.sprite = m_HappninessSprite[2];
                    }
                    break;

                case StatusType.Hunger:
                    currentValue = (float)value / GameUtils.MAX_HUNGER_VALUE;
                    m_HungerBar.fillAmount = currentValue;

                    if (currentValue >= 0.7f)
                        m_HungerBar.color = GameUtils.HexToColor("#42bd41");
                    else if (currentValue >= 0.3f)
                        m_HungerBar.color = GameUtils.HexToColor("#ffc107");
                    else
                        m_HungerBar.color = GameUtils.HexToColor("#e51c23");
                    break;

                case StatusType.Hygiene:
                    currentValue = (float)value / GameUtils.MAX_HYGIENEV_VALUE;
                    m_HyegineBar.fillAmount = currentValue;

                    if (currentValue >= 0.7f)
                        m_HyegineBar.color = GameUtils.HexToColor("#42bd41");
                    else if (currentValue >= 0.3f)
                        m_HyegineBar.color = GameUtils.HexToColor("#ffc107");
                    else
                        m_HyegineBar.color = GameUtils.HexToColor("#e51c23");
                    break;

                case StatusType.Health:
                    currentValue = (float)value / GameUtils.MAX_HEALTH_VALUE;
                    m_HealthBar.fillAmount = currentValue;
                    break;

            }
        }

        private void OnUpdateExp(float value, float maxValue)
        {
            m_ExpBar.fillAmount = value / maxValue;
        }

        private void OnPetLevelup(bool isEvolve, bool canUpdateUI)
        {
            if (isEvolve && canUpdateUI)
            {
                m_LevelText.text = "Evolve!!";
            }
            else
            {
                m_LevelText.text = "LEVEL " + (PlayerData.Instance.PetData.petLevel);

            }
        }

        public void SetTotalExpPointsText()
        {
            m_TotalExpPointsText.text = PlayerData.Instance.data.gotchipoint.ToString();          
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_TotalExpPointsText.transform.parent.GetComponent<RectTransform>());
        }
    }
}