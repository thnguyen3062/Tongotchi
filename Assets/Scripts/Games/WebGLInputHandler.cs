using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WebGLInputHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    // Called when the input field loses focus
    public void OnInputFieldFocusLost(string message)
    {
        inputField.DeactivateInputField();
    }

    // Called when the input field value changes
    public void OnInputFieldChanged(string value)
    {
        inputField.text = value;
    }

    public void ActivateInputField()
    {
        // Focus the input field
        inputField.ActivateInputField();
    }
}