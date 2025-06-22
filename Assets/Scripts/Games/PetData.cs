using Spine.Unity;
using System;
using UnityEngine;

[Serializable]
public class PetData
{
    public int id;
    public string[] petName;
    public SkeletonDataAsset eggSkeletonData;
    public SkeletonDataAsset[] skeletonData;
    public RuntimeAnimatorController[] skeletonAnimator;
    public float hunger;
    public float hygiene;
    public float hapiness;
    public float energy;
    public float health;

    public void SetHungerValue(float value)
    {
        hunger += value;
    }

    public void SetHygieneValue(float value)
    {
        hygiene += value;
    }

    public void SetHapinessValue(float value)
    {
        hapiness += value;
    }

    public void SetEnergyValue(float value)
    {
        energy += value;
    }

    public void SetHealthValye(float value)
    {
        health += value;
    }
}
