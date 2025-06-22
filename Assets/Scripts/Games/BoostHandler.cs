using Core.Utils;
using Game.Websocket;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BoostHandler : MonoBehaviour
{
    public ICallback.CallFunc3<List<BoostItem>, List<BoostItem>> OnBoostsLoaded;
    public ICallback.CallFunc3<int, float> OnUpdateProgress;

    private NativeArray<float> boostRemainTimes;
    private NativeArray<float> petBoostRemainTimes;
    private int boostCount = -1;
    private int petBoostCount = -1;
    private volatile bool loadedCompleted;


    private void OnDestroy()
    {
        if (boostRemainTimes.IsCreated)
            boostRemainTimes.Dispose();
        if (petBoostRemainTimes.IsCreated)
            petBoostRemainTimes.Dispose();

        if (GameManager.Instance) GameManager.OnGameTimeChange -= OnGameTimeChange;
    }

    /// <summary>
    /// Called every second.
    /// </summary>
    /// <param name="passedTime"></param>
    private void OnGameTimeChange(int passedTime)
    {
        if (loadedCompleted && UpdateBoostsCondition())
        {
            var job = new CalculateBoostJob(boostRemainTimes, petBoostRemainTimes, 1);

            int maxBoosts = Mathf.Max(boostCount, petBoostCount);
            JobHandle handle = job.Schedule(maxBoosts, 64);
            handle.Complete();

            if (petBoostRemainTimes.Length > 0 && PlayerData.Instance.PetData.boost.Count > 0 && PlayerData.Instance.PetData.boost.Count == petBoostRemainTimes.Length)
            {
                for (int i = 0; i < petBoostRemainTimes.Length; i++)
                {
                    PlayerData.Instance.PetData.boost[i].remainingTime = petBoostRemainTimes[i];
                    int id = PlayerData.Instance.PetData.boost[i].boostId;
                    float time = PlayerData.Instance.PetData.boost[i].remainingTime;

                    OnUpdateProgress?.Invoke(id, time);

                    if (petBoostRemainTimes[i] <= 0)
                    {
                        ReloadBoosts();
                    }
                }
            }
            
            if (boostRemainTimes.Length > 0 && PlayerData.Instance.data.boost.Count > 0 && PlayerData.Instance.data.boost.Count == boostRemainTimes.Length)
            {
                for (int i = 0; i < boostRemainTimes.Length; i++)
                {
                    PlayerData.Instance.data.boost[i].remainingTime = boostRemainTimes[i];
                    OnUpdateProgress?.Invoke(PlayerData.Instance.data.boost[i].boostId, PlayerData.Instance.data.boost[i].remainingTime);
                    if (boostRemainTimes[i] <= 0)
                    {
                        ReloadBoosts();
                    }
                }
            }
        }
    }

    public void ReloadBoosts()
    {
        if (QueryBoostsCondition())
        {
            GameManager.OnGameTimeChange -= OnGameTimeChange;
            loadedCompleted = false;
            WebSocketRequestHelper.RequestQueryBoost(PlayerData.Instance.data.selectedPetID, (GetBoostsResponse response) =>
            {
                if (response.success)
                {
                    PlayerData.Instance.data.boost = response.player_boosts;
                    PlayerData.Instance.PetData.boost = response.pet_boosts;

                    if (petBoostRemainTimes.IsCreated)
                    {
                        petBoostRemainTimes.Dispose();
                    }
                    petBoostCount = PlayerData.Instance.PetData.boost.Count;
                    petBoostRemainTimes = new NativeArray<float>(petBoostCount, Allocator.Persistent);
                    for (int i = 0; i < petBoostRemainTimes.Length; i++)
                    {
                        petBoostRemainTimes[i] = PlayerData.Instance.PetData.boost[i].remainingTime;
                    }

                    if (boostRemainTimes.IsCreated)
                    {
                        boostRemainTimes.Dispose();
                    }

                    boostCount = PlayerData.Instance.data.boost.Count;
                    boostRemainTimes = new NativeArray<float>(boostCount, Allocator.Persistent);
                    for (int i = 0; i < boostRemainTimes.Length; i++)
                    {
                        boostRemainTimes[i] = PlayerData.Instance.data.boost[i].remainingTime;
                    }

                    loadedCompleted = true;
                    OnBoostsLoaded?.Invoke(response.player_boosts, response.pet_boosts);

                    LoggerUtil.Logging("QUERY_BOOSTS", $"PetBoostCount={response.player_boosts.Count}\nPlayerBoostCount={response.pet_boosts.Count}");
                    GameManager.OnGameTimeChange += OnGameTimeChange;
                }
                else
                {
                    Debug.LogError("Boost response error. Check log for more detail!");
                }
            });
        }
    }

    private bool QueryBoostsCondition()
    {
        return PlayerData.Instance.data.selectedPetID != -1;
    }

    private bool UpdateBoostsCondition()
    {
        return QueryBoostsCondition() && (PlayerData.Instance.data.boost.Count != 0 || PlayerData.Instance.PetData.boost.Count != 0);
    }

    [BurstCompile]
    public struct CalculateBoostJob : IJobParallelFor
    {
        public NativeArray<float> BoostRemainTimes;
        public NativeArray<float> PetBoostRemainTimes;
        public float DeltaTime;

        public CalculateBoostJob(NativeArray<float> boostRemainTimes, NativeArray<float> petBoostRemainTimes, float deltaTime)
        {
            BoostRemainTimes = boostRemainTimes;
            PetBoostRemainTimes = petBoostRemainTimes;
            DeltaTime = deltaTime;
        }

        public void Execute(int index)
        {
            if (index < PetBoostRemainTimes.Length) PetBoostRemainTimes[index] -= DeltaTime;

            if (index < BoostRemainTimes.Length) BoostRemainTimes[index] -= DeltaTime;
        }
    }
}