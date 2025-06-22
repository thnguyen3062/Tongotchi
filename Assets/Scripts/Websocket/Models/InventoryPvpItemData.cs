using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Websocket.Model
{
    public class InventoryPvpItemData
    {
        public string _id;
        public int current_level;
        public int id;
        public int quantity;
        public string tab;
        public string itemName;
        public PvpItemCategory category;

        [JsonConverter(typeof(StringToIntArrayConverter))]
        public PvpSpecificItemCategory[] specificCategory;

        public string state;
        [JsonConverter(typeof(StringToFloatArrayConverter))]
        public float[] valueLv0;

        public int requiredCopyLv1;

        [JsonConverter(typeof(StringToFloatArrayConverter))]
        public float[] valueLv1;

        public int requiredCopyLv2;

        [JsonConverter(typeof(StringToFloatArrayConverter))]
        public float[] valueLv2;

        public int requiredCopyLv3;

        [JsonConverter(typeof(StringToFloatArrayConverter))]
        public float[] valueLv3;

        public int requiredCopyLv4;

        [JsonConverter(typeof(StringToFloatArrayConverter))]
        public float[] valueLv4;

        public int requiredCopyLv5;

        [JsonConverter(typeof(StringToFloatArrayConverter))]
        public float[] valueLv5;

        public string itemInfo;

        public void GetValuesOfCurrentLevel(out Dictionary<PvpSpecificItemCategory, float> stats)
        {
            GetValues(current_level, out stats);
        }

        public void GetValues(int level, out Dictionary<PvpSpecificItemCategory, float> stats)
        {
            stats = new Dictionary<PvpSpecificItemCategory, float>();
            float[] statValueArray = GetLevelStats(level);

            if (specificCategory == null)
            {
                Debug.LogError($"SpecificCategory field of item {id} is null");
                return;
            }

            if (statValueArray == null)
            {
                Debug.LogError($"StatValueArray field of item {id} is null"); return;
            }

            if (statValueArray.Length != specificCategory.Length)
            {
                Debug.LogError($"Fatal error: the Stat Label array and stat value array is not the same");
                LoggerUtil.Logging("DETAIL", $"Item Name: {itemName}\nLevel: {level}\nStat size: {statValueArray.Length}\nValue_0: {valueLv0.Length}");
                return;
            }

            for (int i = 0; i < specificCategory.Length; i++)
            {
                stats.Add(specificCategory[i], statValueArray[i]);
            }
        }

        public float[] GetLevelStats(int level)
        {
            return level switch
            {
                0 => valueLv0,
                1 => valueLv1,
                2 => valueLv2,
                3 => valueLv3,
                4 => valueLv4,
                5 => valueLv5,
                _ => new float[0]
            };
        }

        public int GetRequiredCopy(int level)
        {
            int requiredCopy = 0;

            switch (level)
            {
                case 0:
                    requiredCopy = 0;
                    break;
                case 1:
                    requiredCopy = requiredCopyLv1;
                    break;
                case 2:
                    requiredCopy = requiredCopyLv2;
                    break;
                case 3:
                    requiredCopy = requiredCopyLv3;
                    break;
                case 4:
                    requiredCopy = requiredCopyLv4;
                    break;
                case 5:
                    requiredCopy = requiredCopyLv5;
                    break;
            }
            return requiredCopy;

        }
        public float GetUpgradeParam(uint categoryIndex, int level)
        {
            float param = 0;
            switch (level)
            {
                case 0:
                    param = valueLv1[categoryIndex] - valueLv0[categoryIndex];
                    break;
                case 1:
                    param = valueLv2[categoryIndex] - valueLv1[categoryIndex];
                    break;
                case 2:
                    param = valueLv3[categoryIndex] - valueLv2[categoryIndex];
                    break;
                case 3:
                    param = valueLv4[categoryIndex] - valueLv3[categoryIndex];
                    break;
                case 4:
                    param = valueLv5[categoryIndex] - valueLv4[categoryIndex];
                    break;
                case 5:
                    param = 0;
                    break;
            }
            return param;
        }
    }
}