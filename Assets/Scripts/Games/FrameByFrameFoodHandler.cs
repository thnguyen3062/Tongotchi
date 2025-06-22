using Core.Utils;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameByFrameFoodHandler : MonoBehaviour
{
    private Sprite[] frames; // Array of sprites representing each frame
    [SerializeField] float framesPerSecond = 10.0f; // Frame rate
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;
    private bool isAnimating;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartAnimation(int id)
    {
        frames = PlayerData.Instance.GameItemAnimDict["Item_" + id];// FoodAnimSO.Instance.GetFoodAnim(id).sprites;
        currentFrame = 0;
        timer = 0.0f;
        isAnimating = true;
        spriteRenderer.sprite = frames[currentFrame];
    }

    void Update()
    {
        if (frames.Length == 0)
            return;

        if (!isAnimating)
            return;

        timer += Time.deltaTime;
        if (timer < 1.0f / framesPerSecond)
            return;

        timer -= 1.0f / framesPerSecond;
        currentFrame++;

        if (currentFrame < frames.Length)
        {
            spriteRenderer.sprite = frames[currentFrame];
        }
        else
        {
            isAnimating = false;
            PoolManager.Pools["FoodAnim"].Despawn(transform);
        }
    }
}
