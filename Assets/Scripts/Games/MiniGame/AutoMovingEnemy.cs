using UnityEngine;

namespace Minigame.Scrambler
{
    public class AutoMovingEnemy : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private LayerMask platformLayer;
        private bool movingRight = true;
        private MinigameData minigameData;

        private void Awake()
        {
            gameObject.transform.position = new Vector2(0, transform.position.y);
            minigameData = MiniGameDataSO.Instance.baseMiniGameData;
            this.moveSpeed = minigameData.enemyMoveSpeed;
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 0.11f));
        }

        private void Update()
        {
            if (movingRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.11f, platformLayer);
            if (hit.collider == null)
            {
                movingRight = !movingRight;
            }
        }
    }
}
