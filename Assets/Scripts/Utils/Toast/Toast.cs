using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PathologicalGames;

public class Toast : MonoBehaviour
{
    // Fields    
    [SerializeField] private RectTransform m_RectBox;
    [SerializeField] private LayoutElement m_LayoutElm;
    [SerializeField] private TextMeshProUGUI m_TxtContent;
    [SerializeField] private Animator m_Anim;
    [SerializeField] private float yTarget = 200f;

    // Values    
    private const float MAX_WIDTH = 1000f;
    private const float DISTANCE = 1.5f;

    public string message { get; private set; }

    // Methods
    public void AnimComplete() { Destroy(gameObject); }

    private void FillData(string message, float duration, float yOffset = 0f)
    {
        this.message = message;

        m_TxtContent.text = message;
        if (m_TxtContent.preferredWidth > MAX_WIDTH)
            m_LayoutElm.preferredWidth = MAX_WIDTH;

        Vector2 v2 = m_RectBox.anchoredPosition;
        m_RectBox.anchoredPosition = new Vector2(v2.x, yTarget + yOffset);

        float speed = (duration == 0f) ? 1f : (DISTANCE / duration);
        m_Anim.speed = speed;
        m_Anim.Play("Play", -1, 0f);

        transform.SetAsLastSibling();
    }

    private static List<GameObject> cache = new List<GameObject>();
    public static void Show(string message, float duration = 2f, float yOffset = 0f)
    {
        Transform trans = Resources.Load<Transform>("ToastPrefab");
        Transform toast = PoolManager.Pools["Toast"].Spawn(trans, HttpsConnect.instance.m_ToastParents);
        toast.localPosition = Vector3.zero;
        toast.GetComponent<Toast>().FillData(message, duration, yOffset);
    }
}