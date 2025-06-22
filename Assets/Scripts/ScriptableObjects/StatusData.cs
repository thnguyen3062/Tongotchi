using System;
using UnityEngine;

[Serializable]
public class StatusData
{
    public float happy;
    public float hygiene;
    public float hunger;
    public float healthValue;
    public float maxFeedingExp;
    public float maxPlayingExp;
    public float maxCleaningExp;

    public StatusData(float happy, float hygiene, float hunger, float health)
    {
        this.happy = happy;
        this.hygiene = hygiene;
        this.hunger = hunger;
        healthValue = health;
        maxFeedingExp = 0;
        maxPlayingExp = 0;
        maxCleaningExp = 0;
    }

    public void UpdateValue(float hunger, float hygiene, float happy)
    {
        this.hunger = hunger;
        this.hygiene = hygiene;
        this.happy = happy;

        CalculateHealth();
    }

    public void UpdateHunger(float amount)
    {
        hunger += amount;
        hunger = Mathf.Clamp(hunger, 0, GameUtils.MAX_HUNGER_VALUE);
    }

    public void UpdateHappy(float amount)
    {
        happy += amount;
        happy = Mathf.Clamp(happy, 0, GameUtils.MAX_HAPPYNESS_VALUE);
    }

    public void UpdateHygiene(float amount)
    {
        hygiene += amount;
        hygiene = Mathf.Clamp(hygiene, 0, GameUtils.MAX_HYGIENEV_VALUE);
    }

    public void CalculateHealth()
    {
        healthValue = (hunger * 0.3f) + (hygiene * 0.4f) + (happy * 0.3f);
        healthValue = Mathf.Clamp(healthValue, 0, GameUtils.MAX_HEALTH_VALUE);
    }
}

[Serializable]
public class OldStatusData
{
    public float happyValue;
    public float hygieneValue;
    public float hungerValue;
}