using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using UnityEngine.ResourceManagement.ResourceProviders;
public class GameUtils
{
    public const float MAX_HEALTH_VALUE = 100;
    public const float MAX_HUNGER_VALUE = 100;
    public const float MAX_HYGIENEV_VALUE = 100;
    public const float MAX_HAPPYNESS_VALUE = 100;

    public const float START_HUNGER_VALUE = 75;
    public const float START_HYGIENEV_VALUE = 75;
    public const float START_HAPPYNESS_VALUE = 75;

    public const int MAX_FEEDING_EXP_RECEIVED = 100/*9999*/;
    public const int MAX_PLAYING_EXP_RECEIVED = 100/*9999*/;
    public const int MAX_CLEANING_EXP_RECEIVED = 68/*9999*/;

    public const float EXP_SWEET = 5;
    public const float EXP_FRUIT = 10;
    public const float EXP_MEAT = 15;

    public const float EXP_BALL = 5;
    public const float EXP_SYNTHETIC = 10;
    public const float EXP_PLUSHIE = 15;

    public const float EXP_BATH = 5;
    public const float EXP_CLEAN_POOP = 2;

    public const int FIRST_EVOLVE_LEVEL = 14;
    public const int SECOND_EVOLVE_LEVEL = 24;
    public const int THIRD_EVOLVE_LEVEL = 36;
    public static int[] EVOLVE_LEVELS = new int[3]
    {
        14,
        24,
        36
    };
    public const int FUSE_LEVEL = -1;

    public const int FIRST_EVOLVE_ITEM = 1;
    public const int SECOND_EVOLVE_ITEM = 2;
    public const int THIRD_EVOLVE_ITEM = 3;

    public const int FIRST_EVOLVE_CHANCE = 100;
    public const int SECOND_EVOLVE_CHANCE = 80;
    public const int THIRD_EVOLVE_CHANCE = 60;

    public const int ITEM_EVOLVE_ID = 28;

    public const int ROBOT_BOOST = 27;
    public const int EVOLVE_POTION = 28;
    public const int TICKET_POTION_BOOST = 30;
    public const int EXP_POTION_BOOST = 29;

    public const int MAX_AFK_REWARD = 30;
    public const int MAX_AFK_HOURS = 8;

    public const int SOCIAL_X_ID = 0;
    public const int SOCIAL_TELEGRAM_ID = 1;
    public const int SOCIAL_DISCORD = 2;
    public const int SOCIAL_INSTAGRAM = 3;
    public const int SOCIAL_YOUTUBE = 4;
    public const int SOCIAL_FOUNDER_X = 5;
    public const int PIN_APP_TELEGRAM = 6;

    public const int TICKET_EXCHANGE_RATE = 920;
    public const int MINIGAME_TICKET_PRICE = 1;
    public const int ITEM_PVP_PRICE = 1;
    public const int ATTACK_COUNT_PRICE = 1;

    public const int TIME_EGG_OPENED = 30;
    public const int TIME_EGG_OPENED_START = 45;

    // public static string GET_FRIEND_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/game/friends";
    // public static string SET_SCORE_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/game/submit";
    // public static string CLAIM_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/game/claim";
    // public static string CREATE_INVOICE_LINK_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/invoice/link";
    // public static string CHECK_INVOICE_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/invoice/status";
    // public static string REFUND_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/invoice/refund";
    // public static string SAVE_GAME_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/storage/save";
    // public static string LOAD_GAME_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/storage/load";
    // //public static string GET_USER_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/user";
    // public static string GET_USER_URL = "https://dovvtl8y26.execute-api.ap-southeast-1.amazonaws.com/v2/auth";
    // public static string REMINDER_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/reminder/schedule";
    // public static string REMINDER_CANCEL_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/reminder/cancel";
    // public const string TIME_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/game/time";
    // public const string SAVE_PET_DATA = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/pet/save";
    // public const string LOAD_PET_DATA = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/pet/load";
    // public const string GET_LEADERBOARD_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/minigame/leaderboard";
    // public const string SUBMIT_LEADERBOARD_URL = "https://6fxdst14e8.execute-api.ap-southeast-1.amazonaws.com/minigame/submit";


    public static string GET_FRIEND_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/game/friends";
    public static string SET_SCORE_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/game/submit";
    public static string CLAIM_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/game/claim";
    public static string CREATE_INVOICE_LINK_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/invoice/link";
    public static string CHECK_INVOICE_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/invoice/status";
    public static string REFUND_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/invoice/refund";
    public static string SAVE_GAME_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/storage/save";
    public static string LOAD_GAME_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/storage/load";
    public static string GET_USER_URL = "https://dovvtl8y26.execute-api.ap-southeast-1.amazonaws.com/v2/auth";
    public static string REMINDER_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/reminder/schedule";
    public static string REMINDER_CANCEL_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/reminder/cancel";
    public const string TIME_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/game/time";
    public const string SAVE_PET_DATA = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/pet/save";
    public const string LOAD_PET_DATA = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/pet/load";
    public const string GET_LEADERBOARD_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/minigame/leaderboard";
    public const string SUBMIT_LEADERBOARD_URL = "https://811wx740aa.execute-api.ap-southeast-1.amazonaws.com/minigame/submit";

    public const string YOUTUBE_LINK = "https://www.youtube.com/@tongotchicrypto";
    public const string TELEGRAM_LINK = "https://t.me/tongotchicrypto";
    public const string X_LINK = "https://x.com/tongotchi";
    public const string FOUNDER_X_LINK = "https://x.com/papagotchi";
    public const string INSTAGRAM_LINK = "https://www.instagram.com/tongotchicrypto/";
    public const string CMC_LINK = "https://coinmarketcap.com/community/profile/tongotchi/";

    public static float[] MAX_VALUE_EXP_REFERRAL = new float[7]
    {
        500,
        1000,
        3000,
        12000,
        60000,
        360000,
        2520000
    };

    public static string[] userNames = new string[5]
    {
        "1653473941",
        "670005738",
        "7311786393",
        "563356286",
        "388581253"
    };

    /// <summary>
    /// Gain performance on regular code execution outside of editor by disabling Debug logger.
    /// </summary>
    //    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    //    public static void DisableLoggerOutsideOfEditor()
    //    {
    //#if UNITY_EDITOR || DEVELOPMENT_BUILD
    //        Debug.unityLogger.logEnabled = true;
    //#else
    //            Debug.unityLogger.logEnabled = false;
    //#endif
    //    }

    public static Color GetMainColor(Texture2D texture)
    {
        return GetDominantColor(texture);
    }

    private static Color GetDominantColor(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        int clusterCount = 5; // Number of clusters to find the dominant colors
        Color[] centroids = KMeans(pixels, clusterCount);
        return GetMostFrequentColor(centroids, pixels);
    }

    private static Color[] KMeans(Color[] pixels, int k)
    {
        // Initialize centroids
        Color[] centroids = new Color[k];
        for (int i = 0; i < k; i++)
        {
            centroids[i] = pixels[UnityEngine.Random.Range(0, pixels.Length)];
        }

        bool hasChanged = true;
        Color[] newCentroids = new Color[k];
        int[] counts = new int[k];

        while (hasChanged)
        {
            hasChanged = false;

            // Assign pixels to the nearest centroid
            foreach (Color pixel in pixels)
            {
                int closestCentroid = FindClosestCentroid(pixel, centroids);
                newCentroids[closestCentroid] += pixel;
                counts[closestCentroid]++;
            }

            // Calculate new centroids
            for (int i = 0; i < k; i++)
            {
                if (counts[i] > 0)
                {
                    newCentroids[i] /= counts[i];
                    if (newCentroids[i] != centroids[i])
                    {
                        hasChanged = true;
                    }
                }
                else
                {
                    newCentroids[i] = pixels[UnityEngine.Random.Range(0, pixels.Length)];
                }
            }

            centroids = (Color[])newCentroids.Clone();
            newCentroids = new Color[k];
            counts = new int[k];
        }

        return centroids;
    }

    private static int FindClosestCentroid(Color pixel, Color[] centroids)
    {
        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < centroids.Length; i++)
        {
            float distance = Vector4.Distance(pixel, centroids[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private static Color GetMostFrequentColor(Color[] centroids, Color[] pixels)
    {
        int[] counts = new int[centroids.Length];

        foreach (Color pixel in pixels)
        {
            int closestCentroid = FindClosestCentroid(pixel, centroids);
            counts[closestCentroid]++;
        }

        int maxCount = 0;
        int dominantIndex = 0;
        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > maxCount)
            {
                maxCount = counts[i];
                dominantIndex = i;
            }
        }

        return centroids[dominantIndex];
    }

    public static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid hex string");
            return Color.black; // Return black if the hex string is invalid
        }
    }

    public static string FormatCurrency(float value)
    {
        value = Mathf.RoundToInt(value);
        if (value >= 1_000_000_000_000)
            return (value / 1_000_000_000_000D).ToString("0.##") + "T";
        if (value >= 1_000_000_000)
            return (value / 1_000_000_000D).ToString("0.##") + "B";
        if (value >= 1_000_000)
            return (value / 1_000_000D).ToString("0.##") + "M";
        if (value >= 1_000)
            return (value / 1_000D).ToString("0.##") + "K";

        return value.ToString();
    }

    private static string[] formats = {
            // ISO 8601 Formats
            "yyyy-MM-ddTHH:mm:ssZ",          // 2024-08-31T15:58:03Z
            "yyyy-MM-ddTHH:mm:ss.fffZ",      // 2024-08-31T15:58:03.593Z
            "yyyy-MM-ddTHH:mm:ss",           // 2024-08-31T15:58:03
            "yyyy-MM-ddTHH:mm",              // 2024-08-31T15:58
            "yyyy-MM-dd",                    // 2024-08-31
            "yyyyMMddTHHmmssZ",              // 20240831T155803Z
            "yyyy-MM-dd'T'HH:mm:ss",         // 2024-08-31T15:58:03
            "yyyy-MM-dd'T'HH:mm:ssK",        // 2024-08-31T15:58:03+00:00
            "yyyy-MM-dd'T'HH:mm:ss.fffK",    // 2024-08-31T15:58:03.593+00:00

            // US Formats
            "MM/dd/yyyy",                    // 08/31/2024
            "MM/dd/yyyy HH:mm:ss",           // 08/31/2024 15:58:03
            "MM-dd-yyyy",                    // 08-31-2024
            "MM-dd-yyyy HH:mm:ss",           // 08-31-2024 15:58:03
            "MM/dd/yy",                      // 08/31/24
            "MM-dd-yy",                      // 08-31-24
            "MM/dd/yyyy hh:mm:ss tt",        // 08/31/2024 03:58:03 PM
            "MM-dd-yyyy hh:mm:ss tt",        // 08-31-2024 03:58:03 PM

            // European Formats
            "dd/MM/yyyy",                    // 31/08/2024
            "dd/MM/yyyy HH:mm:ss",           // 31/08/2024 15:58:03
            "dd-MM-yyyy",                    // 31-08-2024
            "dd-MM-yyyy HH:mm:ss",           // 31-08-2024 15:58:03
            "dd/MM/yy",                      // 31/08/24
            "dd-MM-yy",                      // 31-08-24

            // Asian Formats
            "yyyy/MM/dd",                    // 2024/08/31
            "yyyy/MM/dd HH:mm:ss",           // 2024/08/31 15:58:03
            "yyyy.MM.dd",                    // 2024.08.31
            "yyyy.MM.dd HH:mm:ss",           // 2024.08.31 15:58:03

            // Long and Descriptive Formats
            "dddd, MMMM dd, yyyy",           // Saturday, August 31, 2024
            "MMMM dd, yyyy",                 // August 31, 2024
            "dddd, dd MMMM yyyy",            // Saturday, 31 August 2024
            "dd MMMM yyyy",                  // 31 August 2024
            "yyyy, MMMM dd",                 // 2024, August 31

            // Compact/Short Formats
            "yyMMdd",                        // 240831
            "yyyyMMdd",                      // 20240831
            "ddMMyyyy",                      // 31082024
            "yyyy-MM-ddTHH:mm:ss.fff",       // 2024-08-31T15:58:03.593
            "yyyyMMddHHmmss",                // 20240831155803
            "yyyyMMddTHHmmssZ",              // 20240831T155803Z

            // Time-Only Formats
            "HH:mm:ss",                      // 15:58:03
            "HH:mm",                         // 15:58
            "hh:mm:ss tt",                   // 03:58:03 PM
            "hh:mm tt",                      // 03:58 PM
            "HHmmss",                        // 155803

            // Unix Timestamp
            "t",                             // Unix timestamp in seconds (integer)

            // RFC 2822 Format
            "ddd, dd MMM yyyy HH:mm:ss +0000", // Sat, 31 Aug 2024 15:58:03 +0000

            // Custom Formats
            "yyyy-M-d H:m:s",                // 2024-8-31 15:5:3
            "dd-MMM-yy",                     // 31-Aug-24
            "dd-MMM-yyyy",                   // 31-Aug-2024
            "MMM-dd-yyyy",                   // Aug-31-2024

            // Miscellaneous Formats
            "M/d/yyyy",                      // 8/31/2024
            "M-d-yyyy",                      // 8-31-2024
            "d-M-yyyy",                      // 31-8-2024
            "d/M/yyyy",                      // 31/8/2024
            "yyyy/M/d",                      // 2024/8/31
            "yyyy-M-d",                      // 2024-8-31
            "dd.MM.yyyy",                    // 31.08.2024
            "dd.MM.yy",                      // 31.08.24
            "MM.dd.yyyy",                    // 08.31.2024
            "MM.dd.yy",                      // 08.31.24
            "yy/MM/dd",                      // 24/08/31
            "yy.MM.dd",                      // 24.08.31

            // Time with Time Zone
            "yyyy-MM-ddTHH:mm:sszzz",        // 2024-08-31T15:58:03-07:00
            "yyyy-MM-ddTHH:mm:ssK",          // 2024-08-31T15:58:03+00:00
            "yyyy-MM-ddTHH:mm:ss.ffK",       // 2024-08-31T15:58:03.5+00:00
            "yyyy-MM-ddTHH:mm:ss.ffzzz",     // 2024-08-31T15:58:03.5-07:00
    };

    public static DateTime ParseTime(string time)
    {
        //Debug.Log("BEFORE_PARSE_TIME: " + time);
        bool isSuccess = DateTime.TryParseExact(time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime parsedTime);
        if (!isSuccess)
            Debug.LogError("Error in " + time);
        //Debug.Log("AFTER_PARSE_TIME: " + parsedTime + " | " + parsedTime.Kind);
        return parsedTime;
    }

    public static string GetSaveDateString(DateTime dateTime)
    {
        //Debug.Log("GetSaveDateString: " + dateTime + " | " + dateTime.Kind);
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static string ConvertSecondsToTimeString(int totalSeconds)
    {
        // Convert seconds to hours and minutes
        double hours = totalSeconds / 3600.0; // 3600 seconds in 1 hour
        int minutes = totalSeconds / 60;      // 60 seconds in 1 minute

        // If more than or equal to 1 hour, display as hours
        if (hours >= 1)
        {
            return $"{Math.Round(hours, 1)} Hours"; // Round to 1 decimal
        }
        else
        {
            return $"{minutes} mins"; // Display as minutes
        }
    }
}
