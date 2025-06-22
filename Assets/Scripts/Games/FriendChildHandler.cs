using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendChildHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_NameField;
    [SerializeField] private TextMeshProUGUI m_Exp;

    public void InitFriendChild(string userName, float exp)
    {
        m_NameField.text = userName;
        m_Exp.text = exp.ToString();
    }
}
