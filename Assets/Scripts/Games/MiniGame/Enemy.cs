using Core.Utils;
using PathologicalGames;
using System.Collections;
using UnityEngine;

namespace Minigame.Scrambler
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float fallForce = -5f; // Force applied when the enemy falls off
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D[] m_Colliders;
        [SerializeField] private Transform m_Left;
        [SerializeField] private Transform m_Right;

        [SerializeField] private LayerMask platformLayer;
        private float moveSpeed;
        private bool movingRight = true;

        private ICallback.CallFunc onEnemyDie;
        public Enemy SetOnEnemyDie(ICallback.CallFunc func) { onEnemyDie = func; return this; }

        private void Awake()
        {
            gameObject.transform.position = new Vector2(0, transform.position.y);
            this.moveSpeed = MiniGameDataSO.Instance.baseMiniGameData.enemyMoveSpeed;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(m_Left.position, new Vector3(m_Left.position.x, transform.position.y - 0.11f));
            Gizmos.DrawLine(m_Right.position, new Vector3(m_Right.position.x, transform.position.y - 0.11f));
        }

        private void Update()
        {
            rb.linearVelocity = moveSpeed * Time.deltaTime * (movingRight ? Vector2.right : Vector2.left);

            RaycastHit2D hitLeft = Physics2D.Raycast(m_Left.position, Vector2.down, 0.11f, platformLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(m_Right.position, Vector2.down, 0.11f, platformLayer);

            if ((hitLeft.collider == null && !movingRight) || (hitRight.collider == null && movingRight))
            {
                movingRight = !movingRight;
            }
        }

        public void OnHeadHit()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fallForce); // Apply a downward force
            rb.gravityScale = 1; // Ensure the enemy falls due to gravity
            transform.parent = null;
            for (int i = 0; i < m_Colliders.Length; i++)
            {
                m_Colliders[i].enabled = false;
            }
            StartCoroutine(DespawnRoutine());
        }

        private IEnumerator DespawnRoutine()
        {
            yield return new WaitForSeconds(2);
            onEnemyDie?.Invoke();
        }

        private void OnDespawned()
        {
            for (int i = 0; i < m_Colliders.Length; i++)
            {
                m_Colliders[i].enabled = true;
            }
            transform.parent = null;
        }
    }

}

