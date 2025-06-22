using TMPro;
using UnityEngine;

public class UITextField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpReference;

    public void SetString(string text)
    {
        tmpReference.text = text;
    }

    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }
}
