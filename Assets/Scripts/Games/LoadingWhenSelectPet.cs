using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingWhenSelectPet : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadSceneAsync(0);
    }
}
