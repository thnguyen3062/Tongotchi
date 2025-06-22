using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_NoText;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_ScoreText;

    public void InitItem(int no, string userName, int score)
    {
        m_NoText.text = $"{NumericUtils.FormatNumber(no)}. ";//.ToString() + ". ";
        m_NameText.text = userName;
        m_ScoreText.text = score.ToString();
    }
}
