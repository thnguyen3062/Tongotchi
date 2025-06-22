using PathologicalGames;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum BallState
{
    Waiting,
    Dropping,
    Dropped
}

public class BallController : MonoBehaviour
{
    public int id;
    public int index;
    public BallState state = BallState.Waiting;
    public Rigidbody2D rb;
    Vector2 originalPosition;
    Vector2 curPosition;
    float offsetX;
    private List<BallController> checkObjects;
    Vector2 screenBounce;
    float posX;
    float objWidth;
    Vector2 hitTarget = Vector2.up * 1.5f;
    bool isCheckWarning;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Init(int ballID)
    {
        state = BallState.Waiting;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.localScale = Vector3.one;
        index = transform.GetSiblingIndex();
        transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        checkObjects = new List<BallController>();
        id = ballID;
        transform.GetComponent<SpriteRenderer>().sprite = GameController.Instance.ballSprite[id];
        Vector2 size = GetComponent<SpriteRenderer>().sprite.bounds.size;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + size.y / 2);
        GetComponent<CircleCollider2D>().radius = size.x / 2;
        screenBounce = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        objWidth = size.x / 2;
    }

    private void Update()
    {
        if (state == BallState.Waiting && !EventSystem.current.IsPointerOverGameObject() && GameController.Instance.gameState != SuikaGameState.GameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                originalPosition = transform.position;
                curPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                offsetX = curPosition.x - originalPosition.x;
            }
            if (Input.GetMouseButton(0))
            {
                curPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                posX = Mathf.Clamp(curPosition.x - offsetX, -screenBounce.x + objWidth / 3, screenBounce.x - objWidth / 3);
                transform.position = new Vector2(posX, transform.position.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                state = BallState.Dropping;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DetermineCollision(collision);
    }

    private void DetermineCollision(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Ball") && state != BallState.Waiting)
        {
            state = BallState.Dropped;
        }
        if (collision.gameObject.tag == "Ball" && collision.gameObject.GetComponent<BallController>().id == id && collision.gameObject.tag != "Ground" && state != BallState.Waiting)
        {
            BallController ballCollision = collision.gameObject.GetComponent<BallController>();
            if (index < ballCollision.index)
            {
                return;
            }
            checkObjects.Add(ballCollision);
            if (checkObjects.Count > 1)
            {
                return;
            }
            if (GameController.Instance.gameMode == GameMode.Normal)
            {
                if (ballCollision.id == 10)
                {
                    return;
                }
            }
            /*
            else if(GameController.Instance.gameMode == GameMode.Sesame)
            {
                if(ballCollision.id == 0)
                {
                    return;
                }
            }
            */
            transform.DOMove(ballCollision.transform.position, 0.05f).onComplete += delegate
            {
                transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            };

            transform.DOMove(ballCollision.transform.position, 0.1f);
            GameController.Instance.GetScore(ballCollision.id + 1);
            ballCollision.id = GameController.Instance.gameMode == GameMode.Normal ? ballCollision.id + 1 : ballCollision.id - 1;
            //ballCollision.id++;
            ballCollision.transform.SetParent(GameController.Instance.ballContainer.GetChild(ballCollision.id));
            ballCollision.transform.DOScale(new Vector3(0.175f, 0.175f), 0.1f).onComplete += delegate
            {
                ballCollision.GetComponent<SpriteRenderer>().sprite = GameController.Instance.ballSprite[ballCollision.id];
                Vector2 size = ballCollision.GetComponent<SpriteRenderer>().sprite.bounds.size;
                ballCollision.GetComponent<CircleCollider2D>().radius = size.x / 2;
                ballCollision.transform.DOScale(GameController.BALL_BASE_SCALE, 0.1f);
                PoolManager.Pools["Ball"].Despawn(transform, GameController.Instance.despawnContainer);
            };

            GameController.Instance.UpdateIndex();
            //return;
        }
    }

    void UpdateIndex()
    {
        if(state == BallState.Waiting)
        {
            transform.SetAsLastSibling();
        }
        index = transform.GetSiblingIndex();
    }

    void OnSpawned()
    {
        GameController.onUpdateIndex += UpdateIndex;
    }

    void OnDespawned()
    {
        GameController.onUpdateIndex -= UpdateIndex;
    }
}
