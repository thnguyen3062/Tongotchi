using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConsoleView : MonoBehaviour
{
    public Action<string> OnSendCommand;

    public bool IsOpened { get; private set; }

    [SerializeField] private TMP_InputField m_CommandInput;
    [SerializeField] private Button m_SendCommandBtn;

    public void ToggleView()
    {
        IsOpened = !IsOpened;
        gameObject.SetActive(IsOpened);
    }

    public void Show()
    {
        IsOpened = true;
        gameObject.SetActive(IsOpened);
    }

    public void Hide()
    {
        IsOpened = false;
        gameObject.SetActive(IsOpened);
    }

    protected virtual void OnEnable()
    {
        m_SendCommandBtn.onClick.AddListener(SendCommand);
    }

    protected virtual void OnDisable()
    {
        m_SendCommandBtn.onClick.RemoveListener(SendCommand);
    }

    private void SendCommand()
    {
        OnSendCommand?.Invoke(m_CommandInput.text.Trim());
        m_CommandInput.text = string.Empty;
    }
}
