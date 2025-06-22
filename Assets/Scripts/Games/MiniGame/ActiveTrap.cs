using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTrap : MonoBehaviour
{
    [SerializeField] private GameObject childTrap;
    [SerializeField] private float timeActive;
    private MinigameData minigameData;


    private void Awake()
    {
        minigameData = MiniGameDataSO.Instance.baseMiniGameData;
        this.timeActive = minigameData.trapActiveTime;
    }
    // Start is called before the first frame update
    private void Start()
    {
        childTrap.SetActive(false);
    }

    //private void Update()
    //{
    //    if (gameObject.active == false)
    //    {
    //        Debug.Log(gameObject.active == false);
    //        childTrap.SetActive(false);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ActivateTrapAfterDelay(1f));
        }
    }

    private IEnumerator ActivateTrapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        childTrap.SetActive(true);
    }

    private void OnDisable()
    {
        childTrap.SetActive(false);
    }
}
