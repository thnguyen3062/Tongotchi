namespace Game
{
    using UnityEngine;

    public class PetPvPStatLoader : MonoBehaviour
    {
        [SerializeField] private bool loadOnEnable = true;
        [SerializeField] private bool includePvPItem = false;
        [SerializeField] private UINumericField m_HealthStat;
        [SerializeField] private UINumericField m_AttackStat;
        [SerializeField] private UINumericField m_SpeedStat;
        [SerializeField] private UINumericField m_LuckStat;

        protected virtual void OnEnable()
        {
            if (loadOnEnable) LoadStats();
        }

        protected virtual void OnDisable() { }

        public void LoadStats()
        {
            if (PlayerData.Instance.PetData != null && PlayerData.Instance.PetData.petLevel >= 15)
            {
                PlayerData.Instance.CalculateSelectedPetStats(out float healthStat, out float attackStat, out float speedStat, out float luckStat, includePvPItem);

                m_HealthStat.SetFloat(healthStat);
                m_AttackStat.SetFloat(attackStat);
                m_SpeedStat.SetFloat(speedStat);
                m_LuckStat.SetFloat(luckStat);
            }
        }
    }
}