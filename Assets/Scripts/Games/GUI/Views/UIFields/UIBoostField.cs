using TMPro;
using UnityEngine;

public class UIBoostField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_BoostTimeTmp;

    public void SetTime(float remainTime)
    {
        int totalSeconds = Mathf.FloorToInt(remainTime);

        int days = totalSeconds / 86400;               // 60 * 60 * 24
        int hours = (totalSeconds % 86400) / 3600;      // 60 * 60
        int minutes = (totalSeconds % 3600) / 60;       // 60
        int seconds = totalSeconds % 60;

        m_BoostTimeTmp.text = $"{days:D2}:{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}