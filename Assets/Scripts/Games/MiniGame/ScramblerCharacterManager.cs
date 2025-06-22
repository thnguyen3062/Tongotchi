using Core.Utils;
using Spine.Unity;
using UnityEngine;

namespace Minigame.Scrambler
{
    public class ScramblerCharacterManager : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private SkeletonMecanim m_Skel;
        [SerializeField] private Rigidbody2D m_Rigidbody;
        [SerializeField] private float m_MoveSpeed;
        [SerializeField] private float m_BounceForce;
        private float buffer = 0;

        private ICallback.CallFunc onGameOver;
        public ScramblerCharacterManager SetOnGameOver(ICallback.CallFunc func) { onGameOver = func; return this; }
        public void InitCharacter()
        {
            m_Skel.Skeleton.SetSkin($"Character {PlayerData.Instance.PetData.petId + 1}/Stage {PlayerData.Instance.PetData.petEvolveLevel}/Normal");
        }

        public void OnMoveCharacter(int sideIndex)
        {
            m_Rigidbody.linearVelocity = new Vector3(m_MoveSpeed * sideIndex, m_Rigidbody.linearVelocity.y);
            Flip();
        }

        private void Flip()
        {
            if (m_Rigidbody.linearVelocity.x > 0)
                m_Animator.transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
            else if (m_Rigidbody.linearVelocity.x < 0)
                m_Animator.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (ScramblerManager.Instance.GameStarted)
            {
                if (collision.gameObject.CompareTag("DeadZone") || collision.gameObject.CompareTag("TrapActive"))
                {
                    onGameOver?.Invoke();
                }

                if (collision.gameObject.CompareTag("Enemy"))
                {
                    // Check if the player is above the enemy
                    if (collision.contacts[0].point.y > collision.transform.position.y)
                    {
                        m_Rigidbody.linearVelocity = new Vector2(m_Rigidbody.linearVelocity.x, m_BounceForce); // Bounce the player
                        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.OnHeadHit(); // Notify the enemy to fall off
                        }
                    }
                    else
                    {
                        onGameOver?.Invoke();
                    }
                }
            }
        }

        private void Update()
        {
            // Get the viewport position of the character
            Vector3 characterViewportPosition = Camera.main.WorldToViewportPoint(transform.position);

            // Check if the character is out of the right boundary
            if (characterViewportPosition.x > 1 + buffer)
            {
                // Reposition the character to the left side
                Vector3 newPosition = new Vector3(0 - buffer, characterViewportPosition.y, characterViewportPosition.z);
                transform.position = Camera.main.ViewportToWorldPoint(newPosition);
            }
            // Check if the character is out of the left boundary
            else if (characterViewportPosition.x < 0 - buffer)
            {
                // Reposition the character to the right side
                Vector3 newPosition = new Vector3(1 + buffer, characterViewportPosition.y, characterViewportPosition.z);
                transform.position = Camera.main.ViewportToWorldPoint(newPosition);
            }
        }
    }
}