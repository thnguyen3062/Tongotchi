using Core.Utils;
using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryHandler : MonoBehaviour
{
    [SerializeField] private Image m_StoryImage;
    [SerializeField] private Sprite[] m_StorySprite;
    [SerializeField] private GameObject m_StoryTextBackground;
    [SerializeField] private TextMeshProUGUI m_StoryText;
    [SerializeField] private StoryContent[] m_StoryContent;
    [SerializeField] private Button m_NextButton;
    [SerializeField] private Button m_SkipButton;

    private string[] storyAdresses = new string[3]
    {
        "1st scene",
        "2nd scene",
        "3rd scene"
    };
    [SerializeField] private float textSpeedTime;
    private WaitForSeconds textSpeed;
    private int currentStory;
    private int currentCount = 0;
    private ICallback.CallFunc onCompleted;

    [SerializeField] private bool canNext = false;

    public StoryHandler SetOnCompleted(ICallback.CallFunc func) { onCompleted = func; return this; }

    private void Start()
    {
        //InitStory();

        m_NextButton.onClick.AddListener(EndSentence);
        m_SkipButton.onClick.AddListener(OnSkipStory);
    }

    public void InitStory()
    {
        GameManager.Instance.UIManager.FadeScreen.gameObject.SetActive(false);
        textSpeed = new WaitForSeconds(textSpeedTime);
        StartCoroutine(StartStoryRoutine());
    }

    private void OnSkipStory()
    {
        m_SkipButton.gameObject.SetActive(false);
        m_NextButton.onClick.RemoveAllListeners();
        m_StoryTextBackground.SetActive(false);
        m_StoryImage.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            GameManager.Instance.UIManager.FadeScreen.gameObject.SetActive(true);
            PoolManager.Pools["Popup"].Despawn(transform);
            onCompleted?.Invoke();
        });
    }

    private IEnumerator StartStoryRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        StartStory();
    }
    // Story
    private void StartStory()
    {
        m_SkipButton.gameObject.SetActive(true);
        m_StoryImage.sprite = m_StorySprite[currentStory];
        m_StoryImage.DOFade(1, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            m_StoryTextBackground.SetActive(true);
            StartCoroutine(TypeSentences(m_StoryContent[currentStory].contents[currentCount]));
        });
    }

    private void EndSentence()
    {
        if (!canNext)
            return;

        canNext = false;

        if (currentCount < m_StoryContent[currentStory].contents.Length - 1)
        {
            currentCount++;
            StartCoroutine(TypeSentences(m_StoryContent[currentStory].contents[currentCount]));
            return;
        }

        currentStory++;
        currentCount = 0;

        m_StoryTextBackground.SetActive(false);
        m_StoryImage.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (currentStory >= storyAdresses.Length)
            {
                GameManager.Instance.UIManager.FadeScreen.gameObject.SetActive(true);
                PoolManager.Pools["Popup"].Despawn(transform);
                onCompleted?.Invoke();
                return;
            }
            StartStory();
        });
    }

    private IEnumerator TypeSentences(string content)
    {
        m_StoryText.text = string.Empty;

        foreach (char c in content.ToCharArray())
        {
            m_StoryText.text += c;
            if (m_StoryText.isTextOverflowing)
                m_StoryText.alignment = TextAlignmentOptions.BottomLeft;
            else
                m_StoryText.alignment = TextAlignmentOptions.TopLeft;

            yield return textSpeed;
        }
        yield return new WaitForSeconds(0.5f);
        //}
        canNext = true;
    }
}

[Serializable]
public struct StoryContent
{
    public string[] contents;
}