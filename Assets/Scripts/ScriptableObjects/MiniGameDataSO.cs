using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameDataSO", menuName = "Data/MiniGameDataSO")]
public class MiniGameDataSO : ScriptableObject
{
    private static MiniGameDataSO instance;
    public static MiniGameDataSO Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<MiniGameDataSO>("Data/MiniGame");
            return instance;
        }
    }

    public MinigameData baseMiniGameData;
}
