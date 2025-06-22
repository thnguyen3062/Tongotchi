using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameButtonClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private StartGame m_StartGame;

    public void OnPointerClick(PointerEventData eventData)
    {
        m_StartGame.OnClickStart();
    }
}
