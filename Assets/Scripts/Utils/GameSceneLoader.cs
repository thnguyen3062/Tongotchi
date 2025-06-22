using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utils
{
    public class GameSceneLoader : GameSingleton<GameSceneLoader>
    {
        public static void LoadScene(int sceneIndex, Action onCompleted)
        {
            Instance.StartCoroutine(Instance.LoadSceneCoroutine(sceneIndex, onCompleted));
        }

        private System.Collections.IEnumerator LoadSceneCoroutine(int sceneIndex, Action onCompleted)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

            // Wait until the scene is fully loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Invoke the callback after the scene is loaded
            onCompleted?.Invoke();
        }
    }
}