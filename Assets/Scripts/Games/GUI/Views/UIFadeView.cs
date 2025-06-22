using DG.Tweening;
using Game;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeView : BaseView
{
    [SerializeField] private Image m_Img;

    public void FadeIn(float duration, Action onCompleted)
    {
        Show();
        if (m_Img != null)
        {
            m_Img.DOFade(1, duration).OnComplete(() => { onCompleted?.Invoke(); });
        }
    }

    public void FadeOut(float duration, Action onCompleted)
    {
        if (m_Img != null)
        {
            m_Img.DOFade(1, duration).OnComplete(() => { 
                onCompleted?.Invoke();
                Hide();
            });
        }
    }
}
