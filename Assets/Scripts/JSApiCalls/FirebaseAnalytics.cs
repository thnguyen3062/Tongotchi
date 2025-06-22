using System.Runtime.InteropServices;
using UnityEngine;

public class FirebaseAnalytics : MonoBehaviour
{
    public static FirebaseAnalytics instance;

#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void LogEvent(string eventName, string eventParams);
#endif
    private void Awake()
    {
        instance = this;
    }

    public void LogCustomEvent(string eventName, string eventParams = "")
    {
#if !UNITY_EDITOR && !UNITY_ANDROID
        LogEvent(eventName, eventParams);
#endif
    }
}

public class CustomEventWithVariable
{
    public string value;

    public CustomEventWithVariable(string value)
    {
        this.value = value;
    }
}