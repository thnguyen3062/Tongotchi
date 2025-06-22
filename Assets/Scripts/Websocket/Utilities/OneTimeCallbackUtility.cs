using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Websocket.Utils
{
    public static class OneTimeCallbackUtility
    {
        private static readonly Dictionary<string, Delegate> _callbackMap = new();

        public static void Register(string requestId, Action callback)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                Debug.LogError("RequestId is null or empty when registering callback.");
                return;
            }

            _callbackMap[requestId] = callback;
        }

        public static void Register<T>(string requestId, Action<T> callback)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                Debug.LogError("RequestId is null or empty when registering callback.");
                return;
            }

            _callbackMap[requestId] = callback;
        }

        public static void Register<T1, T2>(string requestId, Action<T1, T2> callback)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                Debug.LogError("RequestId is null or empty when registering callback.");
                return;
            }

            _callbackMap[requestId] = callback;
        }

        public static async void RegisterWithTimeout(string requestId, Action callback, float timeoutSeconds)
        {
            Register(requestId, callback);
            await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            Clear(requestId);
        }

        public static async void RegisterWithTimeout<T>(string requestId, Action<T> callback, float timeoutSeconds)
        {
            Register(requestId, callback);
            await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            Clear(requestId);
        }

        public static async void RegisterWithTimeout<T1, T2>(string requestId, Action<T1, T2> callback, float timeoutSeconds)
        {
            Register(requestId, callback);
            await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            Clear(requestId);
        }

        public static void Invoke(string requestId)
        {
            if (_callbackMap.TryGetValue(requestId, out var callback))
            {
                if (callback == null) return;
                if (callback is Action action)
                {
                    action?.Invoke();
                    _callbackMap.Remove(requestId);
                }
                else
                {
                    Debug.LogError($"Callback type mismatch for requestId: {requestId}");
                }
            }
        }

        public static void Invoke<T>(string requestId, T value)
        {
            if (_callbackMap.TryGetValue(requestId, out var callback))
            {
                if (callback != null)
                {
                    if (callback is Action<T> action)
                    {
                        action?.Invoke(value);
                        _callbackMap.Remove(requestId);
                    }
                    else
                    {
                        Debug.LogError($"Callback type mismatch for requestId: {requestId}");
                    }
                }
            }
        }

        public static void Invoke<T1, T2>(string requestId, T1 value1, T2 value2)
        {
            if (_callbackMap.TryGetValue(requestId, out var callback))
            {
                if (callback is Action<T1, T2> action)
                {
                    action?.Invoke(value1, value2);
                    _callbackMap.Remove(requestId);
                }
                else
                {
                    Debug.LogError($"Callback type mismatch for requestId: {requestId}");
                }
            }
        }

        public static void Clear(string requestId)
        {
            if (_callbackMap.ContainsKey(requestId))
            {
                _callbackMap.Remove(requestId);
            }
        }

        public static void ClearAll()
        {
            _callbackMap.Clear();
        }
    }
}