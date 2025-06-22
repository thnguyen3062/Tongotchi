using PathologicalGames;
using UnityEngine;

namespace Minigame.Scrambler
{
    public class ScramblerPlatformManager : MonoBehaviour
    {
        [SerializeField] private float m_MoveSpeed;
        private MinigameData minigameData;
        private GameObject currentEnemy;
        public float time;

        private void Start()
        {
            minigameData = MiniGameDataSO.Instance.baseMiniGameData;
        }



        private void OnEnable()
        {
            if (currentEnemy != null)
            {
                currentEnemy.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (currentEnemy != null)
            {
                currentEnemy.SetActive(false);
            }
        }

        private void Update()
        {
            if (!ScramblerManager.Instance.GameStarted)
                return;

            this.m_MoveSpeed = minigameData.platformMoveSpeed;
            MovePlatform();
            CheckDespawn();
        }

        private void MovePlatform()
        {
            Vector2 targetPos = transform.position;
            targetPos.y += m_MoveSpeed * Time.deltaTime;
            transform.position = targetPos;
        }

        private void CheckDespawn()
        {
            if (transform.position.y >= 1.9f)
            {
                DeactivateEnemy();
                PoolManager.Pools["Platform"].Despawn(transform);
            }
        }

        public void DeactivateEnemy()
        {
            if (currentEnemy != null)
            {
                PoolManager.Pools["Enemy"].Despawn(currentEnemy.transform);
                currentEnemy = null;
            }
        }

        public void SpawnEnemy(Transform enemyPrefab)
        {
            if (currentEnemy != null) return;
            if (Random.value > 0.5f)
            {
                Transform enemyTransform = PoolManager.Pools["Enemy"].Spawn(enemyPrefab, Vector3.zero, Quaternion.identity);
                if (enemyTransform != null)
                {
                    currentEnemy = enemyTransform.gameObject;
                    currentEnemy.transform.SetParent(transform);
                    currentEnemy.transform.localPosition = new Vector2(0, 0.15f);
                    currentEnemy.GetComponent<Enemy>().SetOnEnemyDie(DeactivateEnemy);
                }
            }
        }

        public void SetPlatformSpeed(float speed)
        {
            m_MoveSpeed = speed;
        }
    }
}
