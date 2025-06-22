using UnityEngine;
using DG.Tweening;
using Core.Utils;

public class UISocialBoard : MonoBehaviour
{
    [System.Serializable]
    public class VisibleSettings
    {
        public float screenHeightMin;
        [Range(0, 1)] public float normalRatio;
        [Range(0, 1)] public float afkRatio;
    }

    private RectTransform _rectTrans;

    public enum BoardContentType
    {
        Normal,
        AFK,
        Minigame,
    }

    [SerializeField] private float dotweenDuration = 0.5f;
    [SerializeField] private VisibleSettings[] settings;
    [Header("Sub-Elements")]
    [SerializeField] private Transform m_Buttons;
    [SerializeField] private Transform m_AFKTrans;
    [SerializeField] private Transform m_MinigameTrans;

    private float bottomY;

    private bool isOpened;

    #region Properties
    public RectTransform RectTrans
    {
        get
        {
            if (_rectTrans == null)
            {
                _rectTrans = GetComponent<RectTransform>();
            }
            return _rectTrans;
        }
    }
    public float BottomY
    {
        get
        {
            return -Screen.height / 2f;
        }
    }
    public float HidePositionY
    {
        get
        {
            return -(RectTrans.rect.height - BottomY / 2f);
        }
    }
    public bool IsOpened => isOpened;
    #endregion

    public void ShowBoard(BoardContentType contentType, ICallback.CallFunc onComplete = null)
    {
        transform.DOKill();
        transform.localPosition = new Vector3(transform.localPosition.x, HidePositionY, transform.localPosition.z);
        ShowContent(contentType);

        float visiblePart = RectTrans.rect.height * GetVisibleRatio(contentType);
        float hiddenPart = RectTrans.rect.height - visiblePart;
        float endValue = -hiddenPart;

        isOpened = true;
        transform.DOLocalMoveY(endValue, dotweenDuration).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private void ShowContent(BoardContentType contentType)
    {
        m_Buttons.gameObject.SetActive(contentType == BoardContentType.Normal);
        m_AFKTrans.gameObject.SetActive(contentType == BoardContentType.AFK);
        m_MinigameTrans.gameObject.SetActive(contentType == BoardContentType.Minigame);
    }

    private float GetVisibleRatio(BoardContentType contentType)
    {
        if (settings.Length == 0)
        {
            return 0.5f;
        }

        if (settings.Length == 1)
        {
            return contentType switch
            {
                BoardContentType.AFK => settings[0].afkRatio,
                _ => settings[0].normalRatio,
            };
        }

        foreach (var setting in settings)
        {
            if (Screen.height <= setting.screenHeightMin)
            {
                return contentType switch
                {
                    BoardContentType.AFK => setting.afkRatio,
                    _ => setting.normalRatio,
                };
            }
        }
        return contentType switch
        {
            BoardContentType.AFK => settings[0].afkRatio,
            _ => settings[0].normalRatio,
        };
    }

    public void HideBoard(bool instant = false)
    {
        if (!IsOpened)
        {
            return;
        }

        transform.DOKill();
        if (instant)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, HidePositionY, transform.localPosition.z);
            isOpened = false;
        }
        else
        {
            transform.DOLocalMoveY(HidePositionY, dotweenDuration).OnComplete(() =>
            {
                isOpened = false;
            });
        }
    }
}
