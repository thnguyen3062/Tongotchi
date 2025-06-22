using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFrameByFrameHandler : MonoBehaviour
{
    [SerializeField] private RobotFBFData[] data;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float animationSpeed = 0.1f;

    private int currentAnimationIndex = 0;
    private float changeAnimationTime;
    private float frameTimer;
    private int frameIndex;

    private string[][] robotAnimSprites = new string[4][]
    {
        new string[5]
        {
            "cleaning robot ANIM-Sheet_0",
            "cleaning robot ANIM-Sheet_1",
            "cleaning robot ANIM-Sheet_2",
            "cleaning robot ANIM-Sheet_3",
            "cleaning robot ANIM-Sheet_4",
        },
        new string[6]
        {
            "cleaning robot ANIME 2-Sheet_0",
            "cleaning robot ANIME 2-Sheet_1",
            "cleaning robot ANIME 2-Sheet_2",
            "cleaning robot ANIME 2-Sheet_3",
            "cleaning robot ANIME 2-Sheet_4",
            "cleaning robot ANIME 2-Sheet_5"
        },
        new string[16]
        {
            "cleaning robot ANIME 3-Sheet_0",
            "cleaning robot ANIME 3-Sheet_1",
            "cleaning robot ANIME 3-Sheet_2",
            "cleaning robot ANIME 3-Sheet_3",
            "cleaning robot ANIME 3-Sheet_4",
            "cleaning robot ANIME 3-Sheet_5",
            "cleaning robot ANIME 3-Sheet_6",
            "cleaning robot ANIME 3-Sheet_7",
            "cleaning robot ANIME 3-Sheet_8",
            "cleaning robot ANIME 3-Sheet_9",
            "cleaning robot ANIME 3-Sheet_10",
            "cleaning robot ANIME 3-Sheet_11",
            "cleaning robot ANIME 3-Sheet_12",
            "cleaning robot ANIME 3-Sheet_13",
            "cleaning robot ANIME 3-Sheet_14",
            "cleaning robot ANIME 3-Sheet_15",
        },
        new string[5]
        {
            "cleaning robot ANIME idle-Sheet_0",
            "cleaning robot ANIME idle-Sheet_1",
            "cleaning robot ANIME idle-Sheet_2",
            "cleaning robot ANIME idle-Sheet_3",
            "cleaning robot ANIME idle-Sheet_4",
        }
    };

    private void Start()
    {
        //LoadSprites();
        SetRandomAnimation();
        frameTimer = animationSpeed;
    }

    //private void LoadSprites()
    //{
    //    for (int i = 0; i < robotAnimSprites.Length; i++)
    //    {
    //        for (int j = 0; j < robotAnimSprites[i].Length; j++)
    //        {
    //            data[i].sprites[j] = PlayerData.Instance.SpriteAtlas["Robot"].GetSprite(robotAnimSprites[i][j]);
    //        }
    //    }
    //}

    private void Update()
    {
        if (Time.time >= changeAnimationTime)
            SetRandomAnimation();

        frameTimer -= Time.deltaTime;

        if (frameTimer <= 0)
        {
            frameTimer = animationSpeed;

            m_SpriteRenderer.sprite = data[currentAnimationIndex].sprites[frameIndex];

            frameIndex = (frameIndex + 1) % data[currentAnimationIndex].sprites.Length;
        }
    }

    private void SetRandomAnimation()
    {
        currentAnimationIndex = UnityEngine.Random.Range(0, data.Length);

        frameIndex = 0;
        changeAnimationTime = Time.time + UnityEngine.Random.Range(5, 11);
    }

    [Serializable]
    private struct RobotFBFData
    {
        public Sprite[] sprites;
    }
}
