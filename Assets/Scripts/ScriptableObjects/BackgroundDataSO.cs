using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundData", menuName = "Data/BackgroundData")]
public class BackgroundDataSO : ScriptableObject
{
    private static BackgroundDataSO instance;
    public static BackgroundDataSO Instance
    {
        get
        {
            if (instance == null)
                return instance = PlayerData.Instance.BackgroundData;
            return instance;
        }
    }

    public BackgroundData[] backgrounds;

       public Sprite[] GetBackgroundSprite(int index)
    {
        foreach (var background in backgrounds)
        {
            if (background.id == index)
                return background.backgroundSprites;
        }
        return null;
    }
}

[Serializable]
public struct BackgroundData
{
    public int id;
    public Sprite[] backgroundSprites;
}