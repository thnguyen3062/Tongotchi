using Core.Utils;
using Game.Websocket;
using Game.Websocket.Commands.Storage;
using Game.Websocket.Model;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private static PlayerData instance;
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
                return instance = new PlayerData();
            return instance;
        }
    }

    public PlayerSaveData data;
    private GamePetData petData;

    private PVPProfileResponse pvpProfile;
    public PVPProfileResponse PVPProfile => pvpProfile;

    public void SetPvpProfile(PVPProfileResponse profile)
    {
        pvpProfile = profile;
    }

    public void SetAttackCount(int attackCount)
    {
        pvpProfile.today_attack_count = attackCount;
    }

    public void AddAttackCountPvpProfile(int count)
    {
        pvpProfile.today_attack_count += count;
    }

    public GamePetData PetData => petData;
    private PlayerInfo playerInfo;
    public UserInfo userInfo;
    private List<LevelRewardInfo> rewardInfo = new List<LevelRewardInfo>();
    private List<ItemData> items = new List<ItemData>();
    public List<PVPShopItemData> itemsPvp = new List<PVPShopItemData>();
    public List<ItemData> Items => items;
    public List<CumulativeExpData> lstCumulativeData = new List<CumulativeExpData>();

    //pvp 
    public List<InventoryPvpItemData> LstUserItemsInventory = new List<InventoryPvpItemData>();
    private BackgroundDataSO backgroundData;
    public BackgroundDataSO BackgroundData => backgroundData;
    public void SetBackgroundDataSO(BackgroundDataSO backgroundData)
    {
        this.backgroundData = backgroundData;
    }

    private Dictionary<string, Sprite> gameItemSpriteDict;
    public Dictionary<string, Sprite> GameItemSpriteDict => gameItemSpriteDict;

    public void SetGameItemSpriteDict(Dictionary<string, Sprite> gameItemSpriteDict)
    {
        this.gameItemSpriteDict = gameItemSpriteDict;
    }

    private Dictionary<string, Sprite[]> gameItemAnimDict;
    public Dictionary<string, Sprite[]> GameItemAnimDict => gameItemAnimDict;

    public void SetGameItemAnimDict(Dictionary<string, Sprite[]> gameItemAnimDict)
    {
        this.gameItemAnimDict = gameItemAnimDict;
    }

    private Dictionary<string, Sprite> uiSpriteDict = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> UISpriteDict => uiSpriteDict;
    public void SetUISpriteDict(Dictionary<string, Sprite> uiSpriteDict)
    {
        this.uiSpriteDict = uiSpriteDict;
    }

    //public ICallback.CallFunc3<CurrencyType, float> onUpdateCurrency;
    public ICallback.CallFunc3<float, float> onUpdateExp;
    public ICallback.CallFunc3<bool, bool> onLevelUp;
    public ICallback.CallFunc3<bool, int> onUpdateBoost;

    private int minigameScore = -1;
    public int MinigameScore => minigameScore;
    public void SetMinigameScore(int score)
    {
        minigameScore = score;
    }

    public PlayerInfo PlayerInfo
    {
        get
        {
            if (playerInfo == null)
                return playerInfo = new PlayerInfo();
            return playerInfo;
        }
    }

    public bool MaxPet
    {
        get
        {
            return userInfo.pets.Count == 5;
        }
    }

    #region Save Load (Archive)
    public void SaveData(Action onSaveCompleted = null, bool needLoadTime = true)
    {
        if (needLoadTime)
        {
            WebSocketRequestHelper.GetTimeOnce((time) =>
            {
                SaveGame(time);
            });

        }
        else
        {
            SaveGame();
        }

        void SaveGame(string time = "")
        {
            if (!string.IsNullOrEmpty(time))
                data.lastSaveGametime = GameUtils.GetSaveDateString(GameUtils.ParseTime(time));

            WebSocketRequestHelper.SaveToCloud(data, onSaveCompleted);
        }
    }

    public void SavePetData(Action<bool> onSaveCompleted = null)
    {
        if (data.selectedPetID == -1)
            return;

        WebSocketRequestHelper.SavePetOnce(PetData, onSaveCompleted);
    }

    public void SavePetDataWithID(GamePetData pet, Action<bool> onSaveCompleted)
    {
        if (pet == null)
            return;

        WebSocketRequestHelper.SavePetOnce(pet, onSaveCompleted);
    }


    public void LoadFromCloud(ICallback.CallFunc2<bool> onLoadCompleted)
    {
        if (userInfo.isFirstTime)
        {
            CreateNewUser(() => onLoadCompleted?.Invoke(true));
        }
        else
        {
            LoadFromCloud(1);
        }

        void LoadFromCloud(int loadCount)
        {
            WebSocketRequestHelper.LoadFromCloudOnce((json) =>
            {
                LoadFromCloudCallback(loadCount, json);
            });
        }

        void LoadFromCloudCallback(int loadCount, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                LoadResponseJsonEmpty(loadCount);
            }
            else
            {
                data = JsonConvert.DeserializeObject<PlayerSaveData>(json);

                ConfigCurrency();
                ConfigBackground();

                if (data.listOwnedPet.Count == 0)
                {
                    LoadPet();
                    return;
                }

                ConvertData(onLoadCompleted);
            }
        }

        void LoadResponseJsonEmpty(int loadCount)
        {
            if (loadCount >= 3)
            {
                onLoadCompleted?.Invoke(false);
                return;
            }

            LoadFromCloud(loadCount++);
        }

        void ConfigCurrency()
        {
            if (data.currencyDict != null)
                return;

            data.currencyDict = new Dictionary<CurrencyType, float>()
            {
                {CurrencyType.Token, 0 },
                {CurrencyType.Diamond, 0 },
                {CurrencyType.Ticket, 1000 }
            };
        }

        void ConfigBackground()
        {
            if (data.ownedBackgroundIds.Count != 0)
                return;

            data.ownedBackgroundIds = new List<int>(2) { 16, 15 };
        }

        void LoadPet()
        {
            LoadSelectedPet(false, (success) =>
            {
                if (!success)
                    return;

                if (petData != null)
                {
                    if (petData.boost.Count > 0)
                    {
                        //DateTime endTime = GameUtils.ParseTime(petData.boost[0].startTime).AddSeconds(petData.boost[0].totalTimeBoost);
                        //petData.boost[0].remainTimeBoost = (float)(endTime - GameUtils.ParseTime(petData.currentTime)).TotalSeconds;
                    }
                }
                onLoadCompleted?.Invoke(true);
            });
        }

        // No longer use
        void ConvertData(ICallback.CallFunc2<bool> completed)
        {
            int count = 0;
            for (int i = 0; i < data.listOwnedPet.Count; i++)
            {
                string petJson = JsonConvert.SerializeObject(GetNewPetData(data.listOwnedPet[i]));
                WebSocketRequestHelper.SavePetOnce(GetNewPetData(data.listOwnedPet[i]), (success) =>
                {
                    if (!success)
                        return;

                    count++;
                    if (count < data.listOwnedPet.Count)
                        return;

                    LoadSelectedPet(false, (success) =>
                    {
                        if (!success)
                            return;

                        completed?.Invoke(true);
                        data.listOwnedPet = new List<SavedPetData>();
                    });
                });
            }
        }

        void CreateNewUser(Action onCompleted)
        {
            data = new PlayerSaveData();
            data.InitData();
            SaveData(onCompleted);
        }
    }

    public void SetPetData(GamePetData data)
    {
        if (data != null)
        petData = data;
    }

    public void LoadSelectedPet(bool showLoading, ICallback.CallFunc2<bool> onCompleted)
    {
        if (data.selectedPetID == -1)
        {
            onCompleted?.Invoke(true);
            return;
        }

        if (userInfo.pets.Contains(data.selectedPetID))
        {
            WebSocketRequestHelper.LoadPetOnce(data.selectedPetID, (pet) =>
            {
                petData = pet;

                if (petData.boost.Count > 0)
                {
                    //DateTime endTime = GameUtils.ParseTime(petData.boost[0].startTime).AddSeconds(petData.boost[0].totalTimeBoost);
                    //petData.boost[0].remainTimeBoost = (float)(endTime - GameUtils.ParseTime(userInfo.currentTime)).TotalSeconds;
                }
                onCompleted?.Invoke(true);
            });
        }
        else
        {
            if (userInfo.pets.Count != 0)
            {
                WebSocketRequestHelper.LoadPetOnce(data.selectedPetID, (pet) =>
                {
                    petData = pet;

                    if (petData.boost.Count > 0)
                    {
                        //DateTime endTime = GameUtils.ParseTime(petData.boost[0].startTime).AddSeconds(petData.boost[0].totalTimeBoost);
                        //petData.boost[0].remainTimeBoost = (float)(endTime - GameUtils.ParseTime(userInfo.currentTime)).TotalSeconds;
                    }
                    onCompleted?.Invoke(true);
                });
            }
            else
            {
                data.selectedPetID = -1;
                onCompleted?.Invoke(true);
            }
        }
    }

    #region convert old data
    private GamePetData GetNewPetData(SavedPetData oldData)
    {
        StatusData status = new StatusData(0, 0, 0, 0)
        {
            happy = Mathf.Clamp(oldData.status.happyValue, 0, 100),
            hygiene = Mathf.Clamp(oldData.status.hygieneValue, 0, 100),
            hunger = Mathf.Clamp(oldData.status.hungerValue, 0, 100),
            maxFeedingExp = oldData.expReceivedToday[ExpReceiveType.Feed],
            maxCleaningExp = oldData.expReceivedToday[ExpReceiveType.Clean],
            maxPlayingExp = oldData.expReceivedToday[ExpReceiveType.Play]
        };
        List<BoostItem> boosts = new List<BoostItem>();

        if (oldData.boostDict.Count > 0)
        {
            boosts.Add(oldData.boostDict[GameUtils.ROBOT_BOOST]);
            //boosts[0].SetStartTime(GameUtils.ParseTime(userInfo.currentTime));
        }

        return new GamePetData()
        {
            petId = oldData.id,
            petPhase = oldData.petPhase,
            poopCount = oldData.poopCount,
            nextPoopTime = oldData.nextPoopTime,
            poopElapedTime = oldData.poopElapedTime,
            currentBackgroundIndex = oldData.currentBackgroundIndex,
            status = status,
            targetTime = oldData.targetTime,
            petLevel = oldData.petLevel,
            petEvolveLevel = oldData.petEvolveLevel,
            petExp = oldData.petExp,
            spawnTime = oldData.lastSleepTime,
            boost = boosts,
        };
    }
    #endregion

    public void LoadData(ICallback.CallFunc onLoadCompleted)
    {
        LoadSocialTask();
        LoadRewardInfo();
        LoadItemData();
        LoadCumulativeExpData();
        LoadItemPvpData();
        /*
        if (data.gotchipoint == -1)
        {
            GetGotchiPoints((gp) =>
            {
                data.gotchipoint = gp;
            });
        }
        if (data.ownedBackgroundIds.Count == 0)
        {
            foreach (PVPShopItemData item in itemsPvp)
            {
                if (item.category == PvpItemCategory.Background && item.TotalValueLv0() == 0)
                    data.ownedBackgroundIds.Add(item.id);
            }
        }
        */
        onLoadCompleted?.Invoke();

        if (data.selectedPetID != -1)
        {
            petData.SetOnLevelUp((isEvolve, canUpdateUI) => onLevelUp?.Invoke(isEvolve, canUpdateUI));
            SetPetLevel();
            onUpdateExp?.Invoke(petData.petExp, petData.GetPetMaxExp);
        }

        /*
        foreach (KeyValuePair<CurrencyType, float> currency in data.currencyDict)
        {
            //onUpdateCurrency?.Invoke(currency.Key, currency.Value);
            LoggerUtil.Logging($"Currency: {currency.Key}", $"Value: {currency.Value}");
            // Remove this after the Diamond request/response is available
            if (currency.Key == CurrencyType.Diamond)
            {
                OnCurrencyChange?.Invoke(CurrencyType.Diamond, (int)currency.Value);
            }
        }
        */
        //SaveData(onSaveCompleted, false);
        //SavePetData();
    }

    private void LoadSocialTask()
    {
        if (data.socialTasks.Length != 0)
            return;

        data.socialTasks = new SocialTask[9];

        // Video
        SocialTask videoTask = new SocialTask
        {
            taskType = TaskType.Video,
            count = 0
        };
        data.socialTasks[0] = videoTask;

        SocialTask inviteTask_1 = new SocialTask
        {
            taskType = TaskType.InviteFriend_1,
            count = 0
        };
        data.socialTasks[1] = inviteTask_1;
        SocialTask inviteTask_2 = new SocialTask
        {
            taskType = TaskType.InviteFriend_2,
            count = 0
        };
        data.socialTasks[2] = inviteTask_2;
        SocialTask followX = new SocialTask
        {
            taskType = TaskType.FollowX,
            count = 0
        };
        data.socialTasks[3] = followX;

        SocialTask youtube = new SocialTask
        {
            taskType = TaskType.FollowYoutube,
            count = 0
        };
        data.socialTasks[4] = youtube;

        SocialTask CMC = new SocialTask
        {
            taskType = TaskType.FollowCMC,
            count = 0
        };
        data.socialTasks[5] = CMC;

        SocialTask insta = new SocialTask
        {
            taskType = TaskType.FollowInstagram,
            count = 0
        };
        data.socialTasks[6] = insta;

        SocialTask channel = new SocialTask
        {
            taskType = TaskType.FollowChannel,
            count = 0
        };
        data.socialTasks[7] = channel;

        SocialTask boostChannel = new SocialTask
        {
            taskType = TaskType.BoostChannel,
            count = 0
        };
        data.socialTasks[8] = boostChannel;
    }

    private void LoadRewardInfo()
    {
        TextAsset m_Json = Resources.Load<TextAsset>("DB/LevelupReward");

        rewardInfo = JsonConvert.DeserializeObject<List<LevelRewardInfo>>(m_Json.text);
    }

    public LevelRewardInfo GetLevelRewardInfo() => rewardInfo[petData.petLevel - 1];
    public LevelRewardInfo GetLevelRewardEvolveInfo() => rewardInfo[petData.petLevel];

    private void LoadCumulativeExpData()
    {
        TextAsset m_Json = Resources.Load<TextAsset>("DB/CumulativeExpData");

        lstCumulativeData = JsonConvert.DeserializeObject<List<CumulativeExpData>>(m_Json.text);
    }

    private void LoadItemData()
    {
        TextAsset json = Resources.Load<TextAsset>("DB/Items");

        items = JsonConvert.DeserializeObject<List<ItemData>>(json.text);
    }

    private void LoadItemPvpData()
    {
        TextAsset json = Resources.Load<TextAsset>("DB/ItemPvpData");

        itemsPvp = JsonConvert.DeserializeObject<List<PVPShopItemData>>(json.text);
    }

    public CumulativeExpData GetCumulativeExpData(int level) => lstCumulativeData[level - 1];
    #endregion

    #region Pet Commands
    public void QueryPetStats(Action<PetStatusesResponse, PetLevelExpResponse> callback)
    {
        WebSocketRequestHelper.CheckPetStatsOnce(PetData.petId, callback);
    }

    public void ClaimFusion(string fusionId, Action<FusionClaimResult, GamePetData> callback = null)
    {
        WebSocketRequestHelper.RequestClaimFusion(userInfo.telegramCode, fusionId, callback);
    }

    public void ChangeBG(int backgroundId, Action callback = null)
    {
        if (data.selectedPetID != -1)
        {
            WebSocketRequestHelper.RequestChangeBG(data.selectedPetID, backgroundId, callback);
        }
        else
        {
            Debug.LogError("Data corruption: Player selected pet id is -1!");
        }
    }
    #endregion

    private void SetPetLevel()
    {
        if (petData.petExp >= petData.GetPetMaxExp)
        {
            petData.SetPetExp(ExpReceiveType.Boost, 0);
        }
    }
    public void ResetPlayMinigame()
    {
        // Check if lastSavedTime is null or empty
        if (string.IsNullOrEmpty(data.lastSaveGametime) || data.selectedPetID == -1)
        {
            ResetMinigamePass();
            return;
        }

        WebSocketRequestHelper.GetTimeOnce((time) =>
        {
            data.turnPlayMinigameRemain = Mathf.Clamp(data.turnPlayMinigameRemain, 0, 5);

            DateTime lastSavedTime = GameUtils.ParseTime(data.lastSaveGametime);
            DateTime currentTime = GameUtils.ParseTime(time);
            int dayDifference = (currentTime.Date - lastSavedTime.Date).Days;
            Debug.Log("lastSavedTime: " + lastSavedTime);
            Debug.Log("currentTime: " + currentTime);
            if (dayDifference != 0)
            {
                ResetMinigamePass();
            }
        });
    }

    private void ResetMinigamePass()
    {
        data.turnPlayMinigameRemain += 5;
        data.turnPlayMinigameRemain = Mathf.Clamp(data.turnPlayMinigameRemain, 0, 5);
        //SaveData();
    }

    public int GetOwnedItemEvolve()
    {
        if (data.ownedItemDict.ContainsKey(GameUtils.ITEM_EVOLVE_ID))
        {
            return data.ownedItemDict[GameUtils.ITEM_EVOLVE_ID].quantity;
        }
        else
        {
            return 0;
        }
    }

    public ItemData GetItemData(int id) => items.FirstOrDefault(x => x.id == id);
    public PVPShopItemData GetItemPvpData(int id) => itemsPvp.FirstOrDefault(x => x.id == id);

    #region Command.Settings
    public static void RequestSetSound(bool isOn, Action<ToggleSoundBody> callback = null)
    {
        WebSocketRequestHelper.RequestToggleSound(isOn, callback);
    }
    #endregion

    #region Currency
    public Action<CurrencyType, int> OnCurrencyChange;

    public void AddCurrency(CurrencyType currency, int amount, Action<TicketChangeResponse> response = null)
    {
        switch(currency)
        {
            case CurrencyType.Ticket:
                if (amount >= 0) 
                    AddTicket(amount, response);
                else
                    DecreaseTicket(-1 * amount, response);
                break;
            case CurrencyType.Diamond:
                if (amount >= 0)
                    AddDiamond(amount, response);
                else
                    DecreaseDiamond(-1 * amount, response);
                break;
        }
    }

    public void GetCurrency(CurrencyType currency, Action<GetCurrencyResponse> onCompleted = null)
    {
        switch (currency)
        {
            case CurrencyType.Ticket:
                GetRemainTickets(onCompleted);
                break;
            case CurrencyType.Diamond:
                GetRemainDiamond(onCompleted);
                break;
            default:
                Debug.LogError($"Unsupported currency {currency.ToString()}");
                break;
        }
    }

    #region Diamond
    public void AddDiamond(int amount, Action<TicketChangeResponse> onCompleted)
    {
        WebSocketRequestHelper.IncreaseDiamond(userInfo.telegramCode, amount, (TicketChangeResponse response) => {
            onCompleted?.Invoke(response);
            if (response.success) OnCurrencyChange?.Invoke(CurrencyType.Diamond, response.diamonds);
        });
    }

    public void DecreaseDiamond(int amount, Action<TicketChangeResponse> onCompleted)
    {
        WebSocketRequestHelper.DecreaseDiamond(userInfo.telegramCode, amount, (TicketChangeResponse response) => {
            onCompleted?.Invoke(response);

            if (response.success) OnCurrencyChange?.Invoke(CurrencyType.Diamond, response.diamonds);
        });
    }

    public void GetRemainDiamond(Action<GetCurrencyResponse> onCompleted = null)
    {
        WebSocketRequestHelper.GetDiamond(userInfo.telegramCode, (GetCurrencyResponse response) => {
            onCompleted?.Invoke(response);
            if (response.success)
            {
                OnCurrencyChange?.Invoke(CurrencyType.Diamond, response.diamond);
            }
        });
    }
    #endregion

    #region Tickets

    public void AddTicket(int amount, Action<TicketChangeResponse> onCompleted)
    {
        WebSocketRequestHelper.IncreaseTicket(userInfo.telegramCode, amount, (TicketChangeResponse response) => {
            onCompleted?.Invoke(response);
            if (response.success) OnCurrencyChange?.Invoke(CurrencyType.Ticket, response.tickets);
        });
    }

    public void DecreaseTicket(int amount, Action<TicketChangeResponse> onCompleted)
    {
        WebSocketRequestHelper.DecreaseTicket(userInfo.telegramCode, amount, (TicketChangeResponse response) => {
            onCompleted?.Invoke(response);

            if (response.success) OnCurrencyChange?.Invoke(CurrencyType.Ticket, response.tickets);
        });
    }

    public void GetRemainTickets(Action<GetCurrencyResponse> onCompleted = null)
    {
        WebSocketRequestHelper.GetTickets(userInfo.telegramCode, (GetCurrencyResponse response) => {
            onCompleted?.Invoke(response);
            if (response.success) OnCurrencyChange?.Invoke(CurrencyType.Ticket, response.tickets);
        });
    }
    #endregion
    #endregion

    #region Pet
    public void OnSelectPetOnStart(GamePetData pet)
    {
        data.selectedPetID = pet.petId;
        petData = pet;
        onUpdateExp?.Invoke(petData.petExp, petData.GetPetMaxExp);
        petData.SetOnLevelUp((isEvolve, canUpdateUI) => onLevelUp?.Invoke(isEvolve, canUpdateUI));
        petData.ScheduleNextPoop();
    }

    //public void OnAddCurrency(CurrencyType type, float value, bool canSave = true)
    //{
    //    if (type == CurrencyType.Ticket && data.boost.Any(item => item.id == GameUtils.TICKET_POTION_BOOST) && value > 0)
    //        value *= 2;

    //    Debug.Log(type.ToString());
    //    Debug.Log(value);
    //    data.currencyDict[type] += value;

    //    // Remove this after the Diamond request/response is available
    //    if (type == CurrencyType.Diamond)
    //    {
    //        OnCurrencyChange?.Invoke(CurrencyType.Diamond, (int)data.currencyDict[type]);
    //    }

    //    onUpdateCurrency?.Invoke(type, data.currencyDict[type]);
    //    SaveData();
    //}

    public bool CanReceiveExp(ExpReceiveType type)
    {
        return petData.CanReceiveExp(type);
    }

    public void OnAddExp(ExpReceiveType type, float value)
    {
        petData.SetPetExp(type, value);
        onUpdateExp?.Invoke(petData.petExp, petData.GetPetMaxExp);

        SavePetData();
    }

    public void OnEvolve(int itemCount, bool isSuccess)
    {
        if (isSuccess)
        {
            OnAddItem(GameUtils.ITEM_EVOLVE_ID, -itemCount);
            petData.OnEvolve();
            //SaveData();
        }
    }

    public void CalculateSelectedPetStats(out float healthStat, out float attackStat, out float speedStat, out float luckStat, bool includeEquipment = true)
    {
        CalculatePetStats(PetData, out healthStat, out attackStat, out speedStat, out luckStat, includeEquipment);
    }

    public void CalculatePetStats(GamePetData pet, out float healthStat, out float attackStat, out float speedStat, out float luckStat, bool includeEquipment = true)
    {
        float itemHealthAdd = includeEquipment ? GetItemValue(PvpSpecificItemCategory.HP, PetData.equipe) : 0;
        float itemAttackAdd = includeEquipment ? GetItemValue(PvpSpecificItemCategory.Dmg, PetData.equipe) : 0;
        float itemSpeedAdd = includeEquipment ? GetItemValue(PvpSpecificItemCategory.Speed, PetData.equipe) : 0;
        float itemLuckAdd = includeEquipment ? GetItemValue(PvpSpecificItemCategory.Luck, PetData.equipe) : 0;

        healthStat = pet.hp + itemHealthAdd;
        attackStat = pet.attack + itemAttackAdd;
        speedStat = pet.speed - itemSpeedAdd;
        luckStat = pet.luck + itemLuckAdd;
    }

    public float GetItemValue(PvpSpecificItemCategory category, InventoryPvpItemData item)
    {
        if (item == null)
        {
            LoggerUtil.Logging("Item is null", TextColor.Green);
            return 0;
        }

        item.GetValuesOfCurrentLevel(out Dictionary<PvpSpecificItemCategory, float> statsDictionary);

        if (statsDictionary.TryGetValue(category, out float value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"Item {item.itemName} does not have stat {category.ToString()}");
            return 0;
        }
    }
    #endregion

    #region ITEMS
    public void AddBoost(int id, Action onCompleted = null)
    {
        if (PetData == null)
        {
            Debug.LogError("Error: Pet data is null!");
            return;
        }

        WebSocketRequestHelper.BuyBoost(id, PetData.petId, (BuyBoostResponse response) => {
            onCompleted?.Invoke();
        });

        /* Archive
        if (id == GameUtils.ROBOT_BOOST)
        {
            BoostItem item = petData.boost.FirstOrDefault(x => x.boostId == id);
            if (item != null)
            {
                //item.OnAddBoost(addedTime);
                //SavePetData();
            }
            else
            {
                WebSocketRequestHelper.GetTimeOnce((time) =>
                {
                    BoostItem item = new BoostItem
                    {
                        id = id,
                        totalTimeBoost = addedTime,
                        remainTimeBoost = addedTime,
                        startTime = time
                    };
                    petData.boost.Add(item);
                    //SavePetData();
                });

            }
        }
        else
        {
            BoostItem item = data.boost.FirstOrDefault(x => x.boostId == id);
            if (item != null)
            {
                //item.OnAddBoost(addedTime);
                //SaveData();
            }
            else
            {
                WebSocketRequestHelper.GetTimeOnce((time) =>
                {
                    BoostItem item = new BoostItem
                    {
                        boostId = id,
                        totalTimeBoost = addedTime,
                        remainTimeBoost = addedTime,
                        startTime = time
                    };
                    data.boost.Add(item);
                    //SaveData();
                });
                
            }
        }
        */
    }

    public void RemoveBoost(int id)
    {
        if (id == GameUtils.ROBOT_BOOST)
        {
        }
        else
        {
            data.boost.RemoveAll(item => item.boostId == id);
        }
        onUpdateBoost?.Invoke(false, id);
    }


    public void OnAddItem(int id, int count)
    {
        if (data.ownedItemDict.ContainsKey(id))
        {
            data.ownedItemDict[id].AddItem(count, () =>
            {
                data.ownedItemDict.Remove(id);
            });
        }
        else
        {
            InventoryItem item = new InventoryItem
            {
                data = GetItemData(id),
                quantity = count
            };
            data.ownedItemDict.Add(id, item);
        }

        SaveData();
    }

    public void RequestBuyItem(int itemId, int quantity, Action<BuyItemResponse> onCompleted = null)
    {
        WebSocketRequestHelper.RequestBuyItem(itemId, quantity, onCompleted);
    }
    #endregion

    #region Cheat_RemoveAtProduction
#if UNITY_EDITOR
    public void CheatPetLevel(int level)
    {
        WebSocketRequestHelper.RequestCheatLevel(userInfo.telegramCode, data.selectedPetID, level);
    }
#endif
#endregion

    // Create new pet slot request.
    public void AddNewPetSlot(ICallback.CallFunc3<bool, GamePetData> onPurchaseCompleted)
    {
        bool hackFusionStat = false;

        // vua co them slot vua co them egg . random egg
        int randomPet = UnityEngine.Random.Range(0, 5);

        while (userInfo.pets.Any(x => x == randomPet))
        {
            randomPet = UnityEngine.Random.Range(0, 5);
        }

        int level = hackFusionStat ? 45 : 0;
        int evovleLevel = hackFusionStat ? 3 : 0;
        int id = hackFusionStat ? 1 : randomPet;
        
        StatusData status = new StatusData(GameUtils.START_HAPPYNESS_VALUE, GameUtils.START_HYGIENEV_VALUE, GameUtils.START_HUNGER_VALUE, GameUtils.MAX_HEALTH_VALUE);

        WebSocketRequestHelper.GetTimeOnce((time) =>
        {
            var countdownDuration = TimeSpan.FromSeconds(GameUtils.TIME_EGG_OPENED);
            var targetTime = GameUtils.ParseTime(time).Add(countdownDuration);

            var targetTimeString = GameUtils.GetSaveDateString(targetTime);

            GamePetData pet = new GamePetData
            {
                petId = id,
                poopCount = 0,
                petPhase = PetPhase.Hatching,
                petLevel = level,
                petEvolveLevel = evovleLevel,
                currentBackgroundIndex = 0,
                status = status,
                targetTime = targetTimeString
            };

            SavePetDataWithID(pet, (success) =>
            {
                if (!success)
                {
                    onPurchaseCompleted?.Invoke(false, null);
                    return;
                }

                userInfo.pets.Add(pet.petId);
                onPurchaseCompleted?.Invoke(true, pet);
            });
        });
    }

    /*
    List<GamePetData> listPetData = new List<GamePetData>(UISelectPetPopup.instance.currentListPets)
    {
        pet
    };
    UISelectPetPopup.instance.currentListPets = listPetData.ToArray();
    */

    public void GetGotchiPoints(ICallback.CallFunc2<int> onCompleted)
    {
        int score = 0;
        int count = 0;
        for (int i = 0; i < userInfo.pets.Count; i++)
        {
            WebSocketRequestHelper.LoadPetOnce(userInfo.pets[i], (pet) =>
            {
                if (pet == null)
                    return;

                count++;
                float point = pet.petExp;
                if (pet.petLevel > 0)
                    point += GetCumulativeExpData(pet.petLevel).CumulativeExp;
                score += (int)point;
                if (count >= userInfo.pets.Count)
                {
                    score += (int)(data.currentClaimedRefferalReward + (data.claimedLevel == 0 ? 0 : (int)GameUtils.MAX_VALUE_EXP_REFERRAL[data.claimedLevel - 1]));
                    onCompleted?.Invoke(score);
                }
            });
        }
    }
}

#region player save data
[Serializable]
public class PlayerSaveData
{
    public Dictionary<int, InventoryItem> ownedItemDict = new Dictionary<int, InventoryItem>();
    public Dictionary<CurrencyType, float> currencyDict = new Dictionary<CurrencyType, float>()
        {
            {CurrencyType.Token, 0 },
            {CurrencyType.Diamond, 0 },
            {CurrencyType.Ticket, 1000 }
        };
    public List<BoostItem> boost = new List<BoostItem>();
    public List<SavedPetData> listOwnedPet = new List<SavedPetData>();
    public int selectedPetID = -1;
    public string firstClaimedDailyReward;
    public string lastClaimedDailyReward;
    public int dayCollected = -1;
    public bool isFarming;
    public string targetFarmTime;
    public GameState gameState = GameState.None;
    public List<int> starterTask = new List<int>();
    public bool isTutorialDone;
    public int turnPlayMinigameRemain = 5;
    public int turnPlayerMinigameMax = 5;
    public string lastSaveGametime;
    public bool isSoundOn = true;
    public bool socialQuestCompleted;
    public float currentClaimedRefferalReward = 0;
    public int claimedLevel = 0;
    public TutorialPhase tutorialPhase;
    public TutorialPhase completedPhase;
    public string reminderCode;
    public bool isComeFromBlum;
    public bool blumSendToAnalytic = false;
    public SocialTask[] socialTasks = new SocialTask[0];
    public int gotchipoint = -1;
    public List<int> ownedBackgroundIds = new List<int>(2) { 16, 15 };

    public void InitData()
    {
        currencyDict = new Dictionary<CurrencyType, float>()
        {
            {CurrencyType.Token, 0 },
            {CurrencyType.Diamond, 0 },
            {CurrencyType.Ticket, 1000 }
        };
        isComeFromBlum = PlayerData.Instance.userInfo.referrer.Equals("00VHMMY9M0YBJM4B5DXV50K0QX");
    }
}

[Serializable]
public class BoostItem
{
    public int boostId;
    public string name;
    public float remainingTime;
    public string startTime;
}

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int quantity;

    public void AddItem(int count, ICallback.CallFunc onOutOfStock)
    {
        quantity += count;
        if (quantity <= 0)
        {
            quantity = 0;
            onOutOfStock?.Invoke();
        }
    }
}
#endregion

[Serializable]
public class SocialTask
{
    public TaskType taskType;
    public int count;
    public bool isClaimed;
}

[Serializable]
public class SavedPetData : ICloneable
{
    public int id;
    public PetPhase petPhase;
    public int poopCount;
    public float nextPoopTime;
    public float poopElapedTime;
    public int currentBackgroundIndex = 0;
    public OldStatusData status;
    public string targetTime;
    public int petLevel;
    public int petEvolveLevel;
    public float petExp;
    public string lastSavedTime;
    public string lastSleepTime;
    public Dictionary<int, BoostItem> boostDict = new Dictionary<int, BoostItem>();
    public Dictionary<ExpReceiveType, float> expReceivedToday;

    public object Clone() { return MemberwiseClone(); }
}

public class PetSleepStatus
{
    public int RemainTime;
    public bool IsSleeping;
}

[Serializable]
public class GamePetData : ICloneable
{
    public int petId;
    public float hp;
    public float attack;
    public float speed;
    public float luck;
    // cái này trả về info của item luôn, khỏi cần query db =)))))
    public InventoryPvpItemData equipe;
    // cái này trả về _id của item nhé, cứ parse thôi, làm gì tính sau
    public string item_equipe;
    public PetPhase petPhase;
    public int poopCount;
    public float nextPoopTime;
    public float poopElapedTime;
    public int currentBackgroundIndex = 0;
    public StatusData status;
    public string targetTime;
    public int petLevel;
    public int petEvolveLevel;
    public float petExp;
    public string spawnTime;
    public List<BoostItem> boost = new List<BoostItem>();
    public string lastSavedTime;
    public string currentTime;
    public PetPvpData pvp;
    public PetSleepStatus pet_status;
    public bool isSick;
    public bool isDead;

    public string PetName => PetDataSO.Instance.basePetData.petName[petId];

    private ICallback.CallFunc3<bool, bool> onLevelUp;
    public GamePetData SetOnLevelUp(ICallback.CallFunc3<bool, bool> func) { onLevelUp = func; return this; }

    public void SetPetStatus(float happy, float hygiene, float hunger)
    {
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        hygiene = Mathf.Clamp(hygiene, 0f, 100f);
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        status.UpdateValue(hunger, hygiene, happy);
    }

    public void SetPetPhase(PetPhase phase) { petPhase = phase; }

    public void SetPoopCount(int count) { poopCount = count; }
    public void SetNextPoopTime(float time) { nextPoopTime = time; }

    public void SetPetBackground(int index) { currentBackgroundIndex = index; }

    public float GetPetMaxExp => Mathf.CeilToInt(2 * Mathf.Pow(petLevel + 1, 1.5f) + 25);

    public object Clone() { return MemberwiseClone(); }

    #region Transfer to server
    public void SetPetExp(ExpReceiveType type, float value)
    {
        if (petLevel >= 99)
            return;

        if (petExp < GetPetMaxExp)
        {
            petExp += value;
            //HttpsConnect.instance.SetScore(value);
            WebSocketRequestHelper.SubmitScore(value);
            SetExpReceivedToday(type, value);
        }

        if (petExp >= GetPetMaxExp)
        {
            if (GameUtils.EVOLVE_LEVELS.Contains(petLevel)) /*petLevel == GameUtils.FIRST_EVOLVE_LEVEL || petLevel == GameUtils.SECOND_EVOLVE_LEVEL || petLevel == GameUtils.THIRD_EVOLVE_LEVEL*/
            {
                // onLevelUp sẽ có 2 giá trị bool, 1 là có thể evolve và 2 là cần update UI
                // chỗ này cho update UI text thành chữ "EVOLVE!!"
                onLevelUp?.Invoke(true, true);
            }
            else
            {
                OnLevelUp();
                onLevelUp?.Invoke(false, false);
            }
        }
    }

    private void OnLevelUp()
    {
        // code hiện popup levelup ở đây
        float remainExp = petExp - GetPetMaxExp;
        petExp = remainExp;
        petLevel++;
        FirebaseAnalytics.instance.LogCustomEvent($"pet_level_up_from{petLevel - 1} to {petLevel}");
    }

    // nếu đủ điều kiện evolve, thì gọi vào hàm này
    public void OnEvolve()
    {
        petEvolveLevel++;
        OnLevelUp();

        // chỗ này là evolve nhưng mà sẽ level up thay vì chuyển UI text
        onLevelUp?.Invoke(true, false);
        FirebaseAnalytics.instance.LogCustomEvent($"pet_evolve_from{petEvolveLevel - 1} to {petEvolveLevel}");

        PlayerData.Instance.SavePetData();
        PlayerData.Instance.SaveData();
    }

    private void SetExpReceivedToday(ExpReceiveType type, float value, bool isReset = false)
    {
        switch (type)
        {
            case ExpReceiveType.Play:
                status.maxPlayingExp = Mathf.Clamp(status.maxPlayingExp, 0, GameUtils.MAX_PLAYING_EXP_RECEIVED);
                break;
            case ExpReceiveType.Clean:
                status.maxCleaningExp = Mathf.Clamp(status.maxCleaningExp, 0, GameUtils.MAX_CLEANING_EXP_RECEIVED);
                break;
            case ExpReceiveType.Feed:
                status.maxCleaningExp = Mathf.Clamp(status.maxFeedingExp, 0, GameUtils.MAX_FEEDING_EXP_RECEIVED);
                break;
        }
    }

    public bool CanReceiveExp(ExpReceiveType type)
    {
        switch (type)
        {
            case ExpReceiveType.Play:
                return status.maxPlayingExp < GameUtils.MAX_PLAYING_EXP_RECEIVED;
            case ExpReceiveType.Feed:
                return status.maxFeedingExp < GameUtils.MAX_FEEDING_EXP_RECEIVED;
            case ExpReceiveType.Clean:
                return status.maxCleaningExp < GameUtils.MAX_CLEANING_EXP_RECEIVED;
            default:
                return true;
        }
    }
    public void ScheduleNextPoop()
    {
        float randomPoopTime = UnityEngine.Random.Range(50, 80);
        SetNextPoopTime(randomPoopTime * 60f);
        poopElapedTime = 0;
    }
    #endregion
}

[Serializable]
public class PlayerInfo
{
    public string clanID;
}

public class PetPvpData
{
    public string faction;
    public string first_name;
    public int ranking_point;
    public string telegram_code;
}