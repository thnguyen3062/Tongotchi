using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Websocket.Model
{
    [System.Serializable]
    public struct PVPShopItemData
    {
        public int id;
        public PvpShopTab tab;
        public string itemName;
        public PvpItemCategory category;

        [JsonConverter(typeof(StringToIntArrayConverter))]
        public PvpSpecificItemCategory[] specificCategory;
        public PvpItemState state;

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
    }

    public static class PVPShopItemDataExtension
    {
        public static float TotalValueLv0(this PVPShopItemData item)
        {
            float total = 0;
            foreach (float v in item.valueLv0)
            {
                total += v;
            }
            return total;
        }

        public static int GetPrice(this PVPShopItemData item)
        {
            return item.category switch
            {
                PvpItemCategory.Common => 500,
                PvpItemCategory.Uncommon => 1500,
                PvpItemCategory.Rare => 3000,
                _ => 0,
            };
        }

        public static void GetStatsValue(this PVPShopItemData data, out Dictionary<PvpSpecificItemCategory, float> statDictionary)
        {
            statDictionary = new Dictionary<PvpSpecificItemCategory, float>();
            float[] statValueArray = GetLevelStats(data, 0);

            if (statValueArray.Length != data.specificCategory.Length)
            {
                Debug.LogError($"Fatal error: the Stat Label array and stat value array is not the same");
                return;
            }

            for (int i = 0; i < data.specificCategory.Length; i++)
            {
                statDictionary.Add(data.specificCategory[i], statValueArray[i]);
            }
        }

        private static float[] GetLevelStats(PVPShopItemData data, int level)
        {
            return level switch
            {
                0 => data.valueLv0,
                1 => data.valueLv1,
                2 => data.valueLv2,
                3 => data.valueLv3,
                4 => data.valueLv4,
                5 => data.valueLv5,
                _ => new float[0]
            };
        }
    }
}