using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodAnim", menuName = "Data/FoodAnim")]
public class FoodAnimSO : ScriptableObject
{
    private static FoodAnimSO instance;

    public static FoodAnimSO Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<FoodAnimSO>("Data/FoodAnim");
            return instance;
        }
    }

    [SerializeField] private FoodAnimData[] m_FoodAnim;

    public FoodAnimData GetFoodAnim(int id)
    {
        for(int i = 0; i< m_FoodAnim.Length; i++)
        {
            if (m_FoodAnim[i].id == id)
                return m_FoodAnim[i];
        }
        return null;
    }
}

[Serializable]
public class FoodAnimData
{
    public int id;
    public Sprite[] sprites;
}