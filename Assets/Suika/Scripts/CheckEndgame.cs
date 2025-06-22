using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEndgame : MonoBehaviour
{
    bool isCheckGameOver;
    float timeWaitToMerge = 0;

    public void OnEndGame()
    {
        GameController.Instance.GameOver();
        GameController.Instance.gameState = SuikaGameState.GameOver;
    }

    private void Update()
    {
        if (isCheckGameOver)
        {
            timeWaitToMerge += Time.deltaTime;
            if(timeWaitToMerge >= 1.5f) 
            {
                OnEndGame();
                timeWaitToMerge = 0;
                isCheckGameOver = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && collision.GetComponent<BallController>().state == BallState.Dropped && !isCheckGameOver)
        {
            GameController.Instance.warningLineAnim.GetComponent<Animator>().SetBool("isEndgame", true);
            isCheckGameOver = true;
            print("a");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && collision.GetComponent<BallController>().state == BallState.Dropped && isCheckGameOver)
        {
            isCheckGameOver = false;
            timeWaitToMerge = 0;
            GameController.Instance.warningLineAnim.GetComponent<Animator>().SetBool("isEndgame", false);
            print("b");
        }
    }
}
