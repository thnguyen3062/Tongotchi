using Game.Websocket.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatHandler : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI m_AttackerName;
    [SerializeField] private TextMeshProUGUI m_AttackerScore;
    [SerializeField] private Image m_AttackerImg;
    [SerializeField] private TextMeshProUGUI m_DefenderName;
    [SerializeField] private TextMeshProUGUI m_DefenderScore;
    [SerializeField] private Image m_DefenderImg;
    [Header("Combat")]
    [SerializeField] private float delayResultUI = 1;
    [SerializeField] private Image m_DefenderHealthBar;
    [SerializeField] private Image m_DefenderActionBar;
    [SerializeField] private Image m_AttackerHealthBar;
    [SerializeField] private Image m_AttackerActionBar;
    [SerializeField] private TextMeshProUGUI m_TimerText;

    [SerializeField] private PetCombatFBF m_Attacker;
    [SerializeField] private PetCombatFBF m_Defender;

    [SerializeField] private Transform m_DamagePopup;

    [SerializeField] private Transform[] m_DamagePopupPosition;

    private float defenderMaxHealthValue;
    private float defenderMaxActionValue;
    private float attackerMaxHealthValue;
    private float attackerMaxActionValue;

    private float defenderActionValue;
    private float attackerActionValue;

    private List<PvpTurn> attackerTurns = new List<PvpTurn>();
    private int attackerTurnCount;
    private List<PvpTurn> defenderTurns = new List<PvpTurn>();
    private int defenderTurnCount;

    private PvpCombat combat;
    private float timer;
    private const float MAX_COMBAT_TIME = 20f;

    private bool isCombatStarted;

    [Header("Result")]
    [SerializeField] private GameObject m_ResultPopup;
    [SerializeField] private Image m_PetImage;
    [SerializeField] private Image m_FactionImage;
    [SerializeField] private Sprite m_FactionOchi;
    [SerializeField] private Sprite m_FactionTongo;
    [SerializeField] private Sprite m_WinBackground;
    [SerializeField] private Sprite m_LoseBackground;
    [SerializeField] private Sprite m_DrawBackground;
    [SerializeField] private Image m_ResultBackground;
    [SerializeField] private Button m_ResultConfirmButton;

    [SerializeField] private TextMeshProUGUI m_ResultTitle;
    [SerializeField] private TextMeshProUGUI m_RankingText;
    [SerializeField] private TextMeshProUGUI m_RewardText;
    [SerializeField] private TextMeshProUGUI m_TicketTmp;
    [SerializeField] private TextMeshProUGUI m_ExpTmp;

    private enum ResultType
    {
        Win,
        Lose,
        Draw
    }

    private void Start()
    {
        m_ResultConfirmButton.onClick.AddListener(() =>
        {
            m_ResultPopup.SetActive(false);
            gameObject.SetActive(false);
        });
    }

    public void InitCombat(PvpCombat combat)
    {
        this.combat = combat;

        m_AttackerName.text = combat.attacker.pvp.first_name;
        m_AttackerScore.text = combat.attacker.pvp.ranking_point.ToString();
        string attackerPet = $"PetPvp/IdleAnim/{combat.attacker.petId}_{combat.attacker.petEvolveLevel}_Front_Idle";
        m_AttackerImg.sprite = Resources.Load<Sprite>(attackerPet);

        m_DefenderName.text = combat.defender.pvp.first_name;
        m_DefenderScore.text = combat.defender.pvp.ranking_point.ToString();
        string defenderPet = $"PetPvp/IdleAnim/{combat.defender.petId}_{combat.defender.petEvolveLevel}_Front_Idle";
        m_DefenderImg.sprite = Resources.Load<Sprite>(defenderPet);

        timer = MAX_COMBAT_TIME;
        m_TimerText.text = Mathf.RoundToInt(timer).ToString();

        attackerTurnCount = 0;
        defenderTurnCount = 0;
        attackerTurns = new List<PvpTurn>();
        defenderTurns = new List<PvpTurn>();

        defenderMaxActionValue = combat.defender.speed;
        defenderMaxHealthValue = combat.defender.hp;

        attackerMaxActionValue = combat.attacker.speed;
        attackerMaxHealthValue = combat.attacker.hp;

        UpdateAttackerAction(attackerMaxActionValue);
        UpdateAttackerHealth(attackerMaxHealthValue);
        UpdateDefenderAction(defenderMaxActionValue);
        UpdateDefenderHealth(defenderMaxHealthValue);

        //Remove this logic when BE is fixed.
        int determinedEvolveLevel = combat.attacker.petEvolveLevel < 1 ? 1 : combat.attacker.petEvolveLevel;

        string attackerPetIdleName = $"PetPvp/IdleAnim/{combat.attacker.petId}_{combat.attacker.petEvolveLevel}_back_idle";
        string attackerPetAttackName = $"PetPvp/AttackAnim/{combat.attacker.petId}_{combat.attacker.petEvolveLevel}_back_atk";

        string defenderPetIdleName = $"PetPvp/IdleAnim/{combat.defender.petId}_{determinedEvolveLevel}_front_idle";
        string defenderPetAttackName = $"PetPvp/AttackAnim/{combat.defender.petId}_{determinedEvolveLevel}_front_atk";

        m_Attacker.SetSprites(attackerPetIdleName, attackerPetAttackName);
        m_Defender.SetSprites(defenderPetIdleName, defenderPetAttackName);

        foreach (var turn in combat.turns)
        {
            if (turn.from.Equals("attacker"))
                attackerTurns.Add(turn);
            else
                defenderTurns.Add(turn);
        }

        StartCoroutine(StartCombatRoutine());
    }

    private IEnumerator StartCombatRoutine()
    {
        isCombatStarted = false;

        yield return new WaitForSeconds(2);

        isCombatStarted = true;
    }

    private void Update()
    {
        if (!isCombatStarted)
            return;

        timer -= Time.deltaTime;
        m_TimerText.text = Mathf.RoundToInt(timer).ToString();
        if (timer <= 0)
        {
            isCombatStarted = false;
            StartCoroutine(DelayResultUI(ResultType.Draw));
            return;
        }

        if (attackerTurnCount < attackerTurns.Count)
        {
            attackerActionValue -= Time.deltaTime;
            UpdateAttackerAction(attackerActionValue);
            if (attackerActionValue <= 0)
            {
                OnAttackerAttack();
                attackerActionValue = attackerMaxActionValue;
            }
        }

        if (defenderTurnCount < defenderTurns.Count)
        {
            defenderActionValue -= Time.deltaTime;
            UpdateDefenderAction(defenderActionValue);
            if (defenderActionValue <= 0)
            {
                OnDefenderAttack();
                defenderActionValue = defenderMaxActionValue;
            }
        }
    }

    private void UpdateDefenderHealth(float value)
    {
        m_DefenderHealthBar.fillAmount = value / defenderMaxHealthValue;

        if (value <= 0)
        {
            m_Defender.OnPetDie();
            isCombatStarted = false;
            StartCoroutine(DelayResultUI(ResultType.Win));
            return;
        }
        m_Defender.OnTakeDamage();
    }

    private void UpdateAttackerAction(float value)
    {
        m_AttackerActionBar.fillAmount = value / attackerMaxActionValue;
    }

    private void UpdateDefenderAction(float value)
    {
        m_DefenderActionBar.fillAmount = value / defenderMaxActionValue;
    }

    private void UpdateAttackerHealth(float value)
    {
        m_AttackerHealthBar.fillAmount = value / attackerMaxHealthValue;

        if (value <= 0)
        {
            m_Attacker.OnPetDie();
            isCombatStarted = false;
            StartCoroutine(DelayResultUI(ResultType.Lose));
            return;
        }
        m_Attacker.OnTakeDamage();
    }

    private void OnAttackerAttack()
    {
        m_Attacker.OnPetAttack();
        UpdateDefenderHealth(attackerTurns[attackerTurnCount].defender_hp);
        DamagePopup.Create(m_DamagePopup, m_DamagePopupPosition[0], attackerTurns[attackerTurnCount].damage, attackerTurns[attackerTurnCount].critical, true);
        attackerTurnCount++;
    }

    private void OnDefenderAttack()
    {
        m_Defender.OnPetAttack();
        UpdateAttackerHealth(defenderTurns[defenderTurnCount].attacker_hp);
        DamagePopup.Create(m_DamagePopup, m_DamagePopupPosition[1], defenderTurns[defenderTurnCount].damage, defenderTurns[defenderTurnCount].critical, false);
        defenderTurnCount++;
    }

    private void OnDisable()
    {
        m_Attacker.Refresh();
        m_Defender.Refresh();
    }

    private IEnumerator DelayResultUI(ResultType resultType)
    {
        yield return new WaitForSeconds(delayResultUI);
        ShowResult(resultType);
    }

    private void ShowResult(ResultType resultType)
    {
        string hexColor;

        if (resultType == ResultType.Win)
        {
            m_ResultBackground.sprite = m_WinBackground;
            hexColor = "#00FF11";
            m_ResultTitle.text = "Victory";
            m_TicketTmp.text = "+100";
            m_ExpTmp.text = "| +10 EXP";
        }
        else if (resultType == ResultType.Lose)
        {
            m_ResultBackground.sprite = m_LoseBackground;
            hexColor = "#FF0000";
            m_ResultTitle.text = "Defeated";
            m_TicketTmp.text = "+10";
            m_ExpTmp.text = "| +2 EXP";
        }
        else
        {
            m_ResultBackground.sprite = m_DrawBackground;
            hexColor = "#FFFF00";
            m_ResultTitle.text = "Draw";
            m_TicketTmp.text = "+25";
            m_ExpTmp.text = "| +3 EXP";
        }

        m_PetImage.sprite = Resources.Load<Sprite>($"PetPvp/IdleAnim/{combat.attacker.petId}_{combat.attacker.petEvolveLevel}_Front_Idle");
        m_FactionImage.sprite = combat.attacker.pvp.faction.Equals("tongo") ? m_FactionTongo : m_FactionOchi;
        // set result text here

        int finalPoint = combat.attacker.pvp.ranking_point + combat.attacker_inc_ranking_point;
        if (finalPoint < 0)
        {
            finalPoint = 0;
        }
        m_RankingText.text = $"Ranking: {finalPoint}<color={hexColor}>({combat.attacker_inc_ranking_point})</color>";
        m_ResultPopup.SetActive(true);
    }
}