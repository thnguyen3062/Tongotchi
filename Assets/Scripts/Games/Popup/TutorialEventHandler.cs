using UnityEngine;
using UnityEngine.Events;

public class TutorialEventHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent onTutorialStarted;
    [SerializeField] private UnityEvent onTutorialFinished;

    private void Awake()
    {
        TutorialHandler.OnTutorialStarted += HandlerStartEvent;
        TutorialHandler.OnTutorialFinished += HandlerFinishedEvent;
    }

    private void HandlerStartEvent()
    {
        onTutorialStarted?.Invoke();
    }

    private void HandlerFinishedEvent()
    {
        onTutorialFinished?.Invoke();
    }
}
