using UnityEngine;
using System.Collections;
public enum PetPvPState
{
    Idle,
    Attack,
    Die,
}
public class PetCombatFBF : PetFBF
{
    [SerializeField] private float attackFramePerSecond = 10;
    [SerializeField] private float dieFramePerSecond = 8;
    [SerializeField] private float damageFadeDuration = 1;

    private Sprite[] petAttackSprites = new Sprite[0];
    private Sprite[] petDieSprite = new Sprite[0];
    protected PetPvPState petState;
    private bool isDead;

    public void SetSprites(string idleName, string attackName)
    {
        SetSprites(idleName);
        petAttackSprites = Resources.LoadAll<Sprite>(attackName);
        petDieSprite = Resources.LoadAll<Sprite>("PetPvp/DieAnim/Anim_Pvp_Die");
    }

    public void OnTakeDamage()
    {
        StartCoroutine(ChangeColor(Color.red, Color.white, damageFadeDuration));
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        m_PetImg.color = startColor;
        while (elapsedTime < duration)
        {
            m_PetImg.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_PetImg.color = endColor; 
    }

    public override void Refresh()
    {
        base.Refresh();
        petAttackSprites = new Sprite[0];
        petDieSprite = new Sprite[0];
        isDead = false;
        petState = PetPvPState.Idle;
        m_PetImg.sprite = null;
    }

    protected override void Update()
    {
        if (petIdleSprites.Length == 0 || petAttackSprites.Length == 0 || petDieSprite.Length == 0 || isDead)
            return;

        timer += Time.deltaTime;
        if (timer >= 1f / framePerSecond)
        {
            timer -= 1f / framePerSecond;
            if (petState == PetPvPState.Idle)
            {
                frameIndex = (frameIndex + 1) % petIdleSprites.Length;
                m_PetImg.sprite = petIdleSprites[frameIndex];
            }
            else if (petState == PetPvPState.Die)
            {
                m_PetImg.sprite = petDieSprite[frameIndex];
                frameIndex++;
                if (frameIndex >= petDieSprite.Length)
                {
                    isDead = true;
                }
            }
            else
            {
                m_PetImg.sprite = petAttackSprites[frameIndex];
                frameIndex++;
                if (frameIndex >= petAttackSprites.Length)
                {
                    ChangePetState(PetPvPState.Idle);
                    return;
                }
            }
        }
    }

    private void ChangePetState(PetPvPState petState)
    {
        frameIndex = 0;
        this.petState = petState;
        timer = 0;
        if (petState == PetPvPState.Idle)
            framePerSecond = idleFramePerSecond;
        else if (petState == PetPvPState.Attack)
            framePerSecond = attackFramePerSecond;
        else if (petState == PetPvPState.Die)
            framePerSecond = dieFramePerSecond;
    }

    public void OnPetAttack()
    {
        ChangePetState(PetPvPState.Attack);
    }

    public void OnPetDie()
    {
        ChangePetState(PetPvPState.Die);
    }
}
