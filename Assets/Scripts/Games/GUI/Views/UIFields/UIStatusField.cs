using System;
using UnityEngine;
using UnityEngine.UI;
namespace Game
{
    public class UIStatusField : MonoBehaviour
    {
        private Action<bool> onClickAction;

        [SerializeField] private Button m_Btn;
        [SerializeField] private Image m_StatFiller;
        [SerializeField] private Image m_Emotion;
        [SerializeField] private Sprite[] m_Emotions;
        [SerializeField] private GameObject m_Arrow;

        public bool IsSelected { get; private set; }

        private void Awake()
        {
            m_Btn.onClick.AddListener(Click);
        }

        public void AddListener(Action<bool> action)
        {
            onClickAction += action;
        }

        public void RemoveListener(Action<bool> action)
        {
            onClickAction -= action;
        }

        public void SetStatValue(float value, float max)
        {
            if (m_StatFiller != null)
            {
                m_StatFiller.fillAmount = value / max;
                if (value >= 0.7f)
                {
                    m_StatFiller.color = GameUtils.HexToColor("#42bd41");
                    if (m_Emotions.Length > 0) m_Emotion.sprite = m_Emotions[0];
                }
                else if (value >= 0.3f)
                {
                    m_StatFiller.color = GameUtils.HexToColor("#ffc107");
                    if (m_Emotions.Length > 0) m_Emotion.sprite = m_Emotions[1];
                }
                else
                {
                    m_StatFiller.color = GameUtils.HexToColor("#e51c23");
                    if (m_Emotions.Length > 0) m_Emotion.sprite = m_Emotions[2];
                }
            }
        }

        private void Click()
        {
            IsSelected = !IsSelected;
            onClickAction?.Invoke(IsSelected);
        }

        public void Select()
        {
            if (IsSelected)
            {
                if (m_Arrow) m_Arrow.SetActive(true);
            }
        }

        public void Deselect()
        {
            if (m_Arrow) m_Arrow.SetActive(false);
            IsSelected = false;
        }
    }
}