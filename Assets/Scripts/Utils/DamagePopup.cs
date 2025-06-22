using DG.Tweening;
using PathologicalGames;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI damageText;
    private Color color;

    public static DamagePopup Create(Transform trans, Transform parent, float damageAmount, bool isCritical, bool isAttacker)
    {
        Transform damagePopupTransform = PoolManager.Pools["DamagePopup"].Spawn(trans, parent);
        damagePopupTransform.localPosition = Vector3.zero;

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(Mathf.RoundToInt(damageAmount), isCritical, isAttacker);
        
        return damagePopup;
    }

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(int damageAmount, bool isCritical, bool isAttacker)
    {
        damageText.text = damageAmount.ToString();

        string textHex = isCritical ? "#FF0000" : (isAttacker ? "#FFFFFF" : "FF0000"); //"#FF7700"
        if (ColorUtility.TryParseHtmlString(textHex, out color))
        {
            color.a = 1;
            damageText.color = color;
        }

        RunAnim();
    }

    private void RunAnim()
    {
        float targetY = transform.position.y + 0.3f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(targetY, 1.5f))
            .Insert(1, damageText.DOFade(0, 0.5f))
            .OnComplete(() => PoolManager.Pools["DamagePopup"].Despawn(transform));
    }
}
