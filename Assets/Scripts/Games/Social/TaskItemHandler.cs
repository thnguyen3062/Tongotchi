using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonClaimSocialState
{
    Completed,
    CanClaim,
    Waiting
}

public class TaskItemHandler : MonoBehaviour
{
    [SerializeField] private TaskType taskType;
    public TaskType TaskType => taskType;
    [SerializeField] private TextMeshProUGUI m_Content;
    [SerializeField] private Button m_ClaimBtn;
    [SerializeField] private TextMeshProUGUI m_ClaimText;

    public void InitTaskItem(string content, ButtonClaimSocialState claimState, ICallback.CallFunc3<TaskType, ButtonClaimSocialState> onClaim)
    {
        if (!string.IsNullOrEmpty(content))
            m_Content.text = content;

        if (claimState == ButtonClaimSocialState.Completed)
            m_ClaimBtn.gameObject.SetActive(false);
        else if (claimState == ButtonClaimSocialState.CanClaim)
            m_ClaimText.text = "Claim";
        else
            m_ClaimText.text = "Go";
        //else if (claimState == ButtonClaimSocialState.CanClaim)
        //    m_ClaimBtn.interactable = true;
        //else
        //    m_ClaimBtn.interactable = false;

        m_ClaimBtn.onClick.RemoveAllListeners();
        m_ClaimBtn.onClick.AddListener(delegate
        {
            Debug.Log("Task Clicked");
            onClaim(taskType, claimState);
        });
    }
}
