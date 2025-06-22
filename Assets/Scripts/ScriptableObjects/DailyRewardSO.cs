using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DailyRewardData", menuName = "Data/DailyRewardData")]
public class DailyRewardSO : ScriptableObject
{
    private static DailyRewardSO instance;
    public static DailyRewardSO Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<DailyRewardSO>("Data/DailyRewardData");
            return instance;
        }
    }

    public List<DailyRewardData> datas;
}

[Serializable]
public class DailyRewardData
{
    public int id;
    public DailyRewardType type;
    public int count;
}