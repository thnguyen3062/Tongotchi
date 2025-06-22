using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWarning : MonoBehaviour
{
    private float warningDistance = 1.2f * 0.35f;
    private void Update()
    {
        FindClosestBall();
    }

    void FindClosestBall()
    {
        float distanceToClosestBall = Mathf.Infinity;
        BallController closestBall = null;
        BallController[] allBalls = FindObjectsOfType<BallController>();

        foreach(BallController currentBall in allBalls)
        {
            float distanceToBall = (currentBall.transform.position - transform.position).sqrMagnitude;
            if (currentBall.state == BallState.Dropped)
            {
                if (distanceToBall < distanceToClosestBall)
                {
                    distanceToClosestBall = distanceToBall;
                    closestBall = currentBall;
                }
            }
        }
        if (closestBall != null)
        {
            Vector2 size = closestBall.GetComponent<SpriteRenderer>().sprite.bounds.size;
            float distance = Vector2.Distance(transform.position, new Vector3(closestBall.transform.position.x, closestBall.transform.position.y + size.y / 2));
            if(distance <= warningDistance && distance > 0)
            {
                GameController.Instance.warningLineAnim.GetComponent<Animator>().SetBool("isWarning", true);
            }
            else if (distance > warningDistance)
            {
                GameController.Instance.warningLineAnim.GetComponent<Animator>().SetBool("isWarning", false);
            }
            Debug.DrawLine(transform.position, new Vector3(closestBall.transform.position.x, closestBall.transform.position.y + size.y / 2));
        }
    }
}
