using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PetPvpData", menuName = "Data/PetPvpData")]
public class PetPvpSO : ScriptableObject
{
    private static PetPvpSO instance;
    public static PetPvpSO Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<PetPvpSO>("Data/PetPvpData");
            return instance;
        }
    }

    [SerializeField] private PetPvpSpriteData[] petPvpSpriteData;

    public Sprite GetPetPvpSprite(int petId, int petEvolveLevel)
    {
        for (int i = 0; i < petPvpSpriteData.Length; i++)
        {
            if (petPvpSpriteData[i].petId == petId)
            {
                for (int j = 0; j < petPvpSpriteData[i].petPvpSpriteInfo.Length; j++)
                {
                    if (petPvpSpriteData[i].petPvpSpriteInfo[j].petEvolveLevel == petEvolveLevel)
                    {
                        return petPvpSpriteData[i].petPvpSpriteInfo[j].petSprites[0];
                    }
                }
                return null;
            }
        }
        return null;
    }

    public Sprite[] GetPetPvpSprites(int petId, int petEvolveLevel)
    {
        for (int i = 0; i < petPvpSpriteData.Length; i++)
        {
            if (petPvpSpriteData[i].petId == petId)
            {
                for (int j = 0; j < petPvpSpriteData[i].petPvpSpriteInfo.Length; j++)
                {
                    if (petPvpSpriteData[i].petPvpSpriteInfo[j].petEvolveLevel == petEvolveLevel)
                    {
                        return petPvpSpriteData[i].petPvpSpriteInfo[j].petSprites;
                    }
                }
                return null;
            }
        }
        return null;
    }
}

[Serializable]
public struct PetPvpSpriteData
{
    public int petId;
    public PetPvpSpriteInfo[] petPvpSpriteInfo;
}

[Serializable]
public struct PetPvpSpriteInfo
{
    public int petEvolveLevel;
    public Sprite[] petSprites;
}