using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIActionButton : MonoBehaviour
    {
        public Action<ActionType> OnClick;
        [SerializeField] private ActionType action;
        [SerializeField] private Button m_Btn;
        [SerializeField] private Image m_Img;

        public ActionType Action => action;

        private void Awake()
        {
            m_Btn = GetComponent<Button>();
            m_Btn.onClick.AddListener(Click);
        }

        private void Click()
        {
            OnClick?.Invoke(action);
        }

        public void SetColor(Color color)
        {
            m_Img.color = color;
        }
    }
}