using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetData", menuName = "Data/PetData")]
public class PetDataSO : ScriptableObject
{
    private static PetDataSO instance;
    public static PetDataSO Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<PetDataSO>("Data/PetData");
            return instance;
        }
    }

    public PetData basePetData;
}
