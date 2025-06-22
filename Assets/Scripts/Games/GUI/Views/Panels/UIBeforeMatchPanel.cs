using System.Collections;
using Game;
using Game.Websocket.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBeforeMatchPanel : BaseView
{
    [Header("Ochi")]
    [SerializeField] private TextMeshProUGUI m_OchiNameText;
    [SerializeField] private TextMeshProUGUI m_OchiRankingText;
    [SerializeField] private Image m_OchiPetIcon;
    [Header("Tongo")]
    [SerializeField] private TextMeshProUGUI m_TongoNameText;
    [SerializeField] private TextMeshProUGUI m_TongoRankingText;
    [SerializeField] private Image m_TongoPetIcon;

    public void SetBeforeMatch(PvpCombat combat)
    {
        bool isOchi = combat.attacker.pvp.faction.Equals("ochi");

        m_OchiNameText.text = isOchi ? combat.attacker.pvp.first_name : combat.defender.pvp.first_name;
        m_OchiRankingText.text = (isOchi ? combat.attacker.pvp.ranking_point : combat.defender.pvp.ranking_point).ToString();
        string ochiPet = isOchi ? $"PetPvp/IdleAnim/{combat.attacker.petId}_{combat.attacker.petEvolveLevel}_Front_Idle" : $"PetPvp/IdleAnim/{combat.defender.petId}_{combat.defender.petEvolveLevel}_Front_Idle";
        m_OchiPetIcon.sprite = Resources.Load<Sprite>(ochiPet);

        m_TongoNameText.text = isOchi ? combat.defender.pvp.first_name : combat.attacker.pvp.first_name;
        m_TongoRankingText.text = (isOchi ? combat.defender.pvp.ranking_point : combat.attacker.pvp.ranking_point).ToString();
        string tongoPet = isOchi ? $"PetPvp/IdleAnim/{combat.defender.petId}_{combat.defender.petEvolveLevel}_Front_Idle" : $"PetPvp/IdleAnim/{combat.attacker.petId}_{combat.attacker.petEvolveLevel}_Front_Idle";
        m_TongoPetIcon.sprite = Resources.Load<Sprite>(tongoPet);

        //m_BeforeMatch.SetActive(true);

        StartCoroutine(BeforeMatchRoutine(combat));
    }

    private IEnumerator BeforeMatchRoutine(PvpCombat combat)
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.StartPvpCombat(combat);
        //m_CombatHandler.gameObject.SetActive(true);
        //m_CombatHandler.InitCombat(combat);
        Hide();
    }
}
