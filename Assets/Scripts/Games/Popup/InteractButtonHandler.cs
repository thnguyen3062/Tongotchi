using Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractButtonHandler : MonoBehaviour
{
    [SerializeField] private ActionType m_ActionType;
    [SerializeField] private GameObject m_Arrow;

    private Button interactButton;
    private bool isSelected;

    private ICallback.CallFunc2<ActionType> onInteractButtonClicked;
    public InteractButtonHandler SetOnInteractButtonClicked(ICallback.CallFunc2<ActionType> func) { onInteractButtonClicked = func; return this; }

    public Button InteractButton
    {
        get
        {
            if (interactButton == null)
            {
                interactButton = GetComponent<Button>();
            }
            return interactButton;
        }
    }

    private void Start()
    {
        InteractButton.onClick.AddListener(OnClick);
        UIManager.Instance.onButtonSelected += OnInteractWithPet;
    }

    private void OnClick()
    {
        onInteractButtonClicked?.Invoke(m_ActionType);
    }

    private void OnInteractWithPet(ActionType type, bool isClose)
    {
        m_Arrow.SetActive(m_ActionType == type && !isClose);
    }

    public void Interactable(bool value)
    {
        InteractButton.interactable = value;
    }
}
