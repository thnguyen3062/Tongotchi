using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameByFrameBackgroundHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_BackgroundSpriteRenderer;
    [SerializeField] private float framePerSecond;

    private int backgroundIndex = -1;
    private Sprite[] backgroundSprires;
    private int currentFrame;
    private float timer;
    [SerializeField] private BackgroundDataSO backgroundSO;

    private void Update()
    {
        if (m_BackgroundSpriteRenderer == null)
            return;

        if (backgroundSO == null)
            return;

        if (backgroundSO.backgrounds.Length == 0)
            return;

        if (backgroundSprires == null)
            return;

        if (backgroundSprires.Length == 0)
            return;

        if (backgroundIndex == -1)
            return;

        timer += Time.deltaTime;
        if (timer < 1f / framePerSecond)
            return;

        timer -= 1 / framePerSecond;
        currentFrame = (currentFrame + 1) % backgroundSprires.Length;
        m_BackgroundSpriteRenderer.sprite = backgroundSprires[currentFrame];
    }

    public void SetBackgroundIndex(int index)
    {
        int backgroundId = PlayerData.Instance.data.ownedBackgroundIds[index];
        backgroundIndex = 0;
        currentFrame = 0;
        timer = 0;

        if (backgroundSO == null)
            backgroundSO = BackgroundDataSO.Instance;
        backgroundSprires = backgroundSO.GetBackgroundSprite(backgroundId);
        m_BackgroundSpriteRenderer.sprite = backgroundSprires[currentFrame];
    }

    public int GetBackgroundIndex()
    {
        return backgroundIndex;
    }
}
