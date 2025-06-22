using Core.Utils;
using Game.Websocket;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{

    // string testData ="query_id=AAHzQS9PAAAAAPNBL09rZ3ja&user=%7B%22id%22%3A1328497139%2C%22first_name%22%3A%22N%22%2C%22last_name%22%3A%22K%22%2C%22username%22%3A%22kinnguyen2901%22%2C%22language_code%22%3A%22en%22%2C%22allows_write_to_pm%22%3Atrue%7D&auth_date=1731401983&hash=8fbbc89a5b59e022e36c402d1abd3f1fb1de67e4476939f9e19a1a0a1e44322f";
    //string testData = "user=%7B%22id%22%3A563356286%2C%22first_name%22%3A%22Bui%22%2C%22last_name%22%3A%22Chau%22%2C%22username%22%3A%22buichaulb%22%2C%22language_code%22%3A%22en%22%2C%22allows_write_to_pm%22%3Atrue%7D&chat_instance=-8688721815640911464&chat_type=supergroup&start_param=ref_1234566&auth_date=1722187031&hash=8e898d755b4c057636706ffd992b697ed3ab248462a57c45e0f25c6ea490d5e3";
    string testData = "query_id=AAF-IpQhAAAAAH4ilCFahjJZ&user=%7B%22id%22%3A563356286%2C%22first_name%22%3A%22Bui%22%2C%22last_name%22%3A%22Chau%22%2C%22username%22%3A%22buichaulb%22%2C%22language_code%22%3A%22en%22%2C%22allows_write_to_pm%22%3Atrue%2C%22photo_url%22%3A%22https%3A%5C%2F%5C%2Ft.me%5C%2Fi%5C%2Fuserpic%5C%2F320%5C%2FCVXCnbV650eHS5G_EFgVYFklwndTabFPVAUDBi8qDaA.svg%22%7D&auth_date=1733760245&signature=L0Jqznr0ejLMLuYsljH1XLylrEDmUr2ExJ5UPBUuBJ3-y5vZgLOrwrBCNaiSlVZeAJS954Y82_95OdLNwUP7DQ&hash=65804ecf5999dcedaffd9fb056bf7be6157641f4427d0a2fab598026ce68f408";
    //string testData = "user=%7B%22id%22%3A7311786k393%2C%22first_name%22%3A%22%C4%90%C3%A0o%20%C4%90%C3%ACnh%22%2C%22last_name%22%3A%22Th%E1%BA%AFng%22%2C%22language_code%22%3A%22en%22%2C%22allows_write_to_pm%22%3Atrue%7D&chat_instance=2318745374025619458&chat_type=private&start_param=ref_01J3YVZ37DDPX2PM5QDGP47V57&auth_date=1722322963&hash=694749615b3b5ce1d37975a9daa1c351b7fcf5e8c16b586b02612c7adf6f8130";
    [SerializeField] private GameObject m_Block;
    private int count;
    [SerializeField] private Image m_ProgressImage;
    [SerializeField] private TextMeshProUGUI m_ProgressTextResult;
    [SerializeField] private TextMeshProUGUI m_ProgressText;
    [SerializeField] private TestAccountConfigSO accConfig;
    private int currentLoadCount;
    private const float maxLoadCount = 7;

#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void getUser();
    [DllImport("__Internal")]
    private static extern void connectSocket();
#endif
    private void Start()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(1f);

#if (UNITY_EDITOR || UNITY_ANDROID)
        string data = JsonConvert.SerializeObject(new PostData(accConfig.GetAccountInfo(), ""));
        ReceiveUserID(data);
#else
    connectSocket();
    getUser();
#endif
    }

    public void ReceiveUserID(string data)
    {
        PostData postData = JsonConvert.DeserializeObject<PostData>(data);
        Debug.Log("Received Query: " + postData.query + " | Command: " + postData.command);
        m_ProgressText.text = "Loading User Info";
        currentLoadCount++;
        HttpsConnect.instance.GetUser(postData.query, postData.command, () =>
        {
            //PlayerData.Instance.userInfo.maintenance = true;
            m_ProgressText.text = "Loading game info";
            currentLoadCount++;
            if (PlayerData.Instance.userInfo.maintenance)
            {
                m_Block.gameObject.SetActive(true);
                if (GameUtils.userNames.Contains(PlayerData.Instance.userInfo.telegramCode))
                {
                    LoadGameData();
                    return;
                }
            }
            else
            {
                LoadGameData();
            }

        });

        void LoadGameData()
        {
            WebSocketConnector.Instance.ConnectSocket(PlayerData.Instance.userInfo.accessToken, (success) =>
            {
                if (!success)
                {
                    return;
                }

                m_ProgressText.text = "Loading Save game";
                currentLoadCount++;

                PlayerData.Instance.LoadFromCloud((success) =>
                {
                    if (!success)
                    {
                        m_ProgressImage.gameObject.SetActive(true);
                        m_ProgressText.text = "Load Data Failed, Please try again!";
                        return;
                    }

                    if (!PlayerData.Instance.data.blumSendToAnalytic)
                    {
                        if (!string.IsNullOrEmpty(PlayerData.Instance.userInfo.referrer))
                        {
                            if (PlayerData.Instance.data.isComeFromBlum)
                                FirebaseAnalytics.instance.LogCustomEvent("user_from_blum");
                            else
                                FirebaseAnalytics.instance.LogCustomEvent("user_from_refer", JsonConvert.SerializeObject(new CustomEventWithVariable(PlayerData.Instance.userInfo.referrer)));
                        }
                        PlayerData.Instance.data.blumSendToAnalytic = true;
                    }


                    //  PlayerData.Instance.data.currencyDict[CurrencyType.Diamond] = 900;
                    // PlayerData.Instance.SaveData();

                    currentLoadCount++;
                    StartCoroutine(LoadBackgroundRoutine(() =>
                    {
                        count++;
                        currentLoadCount++;
                    }));
                    StartCoroutine(LoadItemSpriteRoutine(() =>
                    {
                        count++;
                        currentLoadCount++;
                    }));
                    StartCoroutine(LoadItemAnimRoutine(() =>
                    {
                        count++;
                        currentLoadCount++;
                    }));
                });
            });
        }
    }

    private void Update()
    {
        if (currentLoadCount >= 3)
        {
            m_ProgressImage.fillAmount = currentLoadCount / maxLoadCount;
            m_ProgressText.text = $"Downloading Assets {Mathf.FloorToInt(currentLoadCount / maxLoadCount * 100)}%";
        }

        if (count < 3)
            return;

        m_Block.SetActive(false);
        SceneManager.LoadSceneAsync("GameScene");
        count = 0;
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(1);
    }

    private class PostData
    {
        public string query;
        public string command;
        public PostData(string query, string command)
        {
            this.query = query;
            this.command = command;
        }
    }

    private string[] backgroundAddresses = new string[6]
    {
        "Assets/AddressableResources/Backgrounds/Background_0.png",
        "Assets/AddressableResources/Backgrounds/Background_1.png",
        "Assets/AddressableResources/Backgrounds/Background_2.png",
        "Assets/AddressableResources/Backgrounds/Background_3.png",
        "Assets/AddressableResources/Backgrounds/Background_4.png",
        "Assets/AddressableResources/Backgrounds/Background_5.png"
    };

    private IEnumerator LoadBackgroundRoutine(ICallback.CallFunc onCompleted)
    {
        AsyncOperationHandle<BackgroundDataSO> backgroundSOHandler = Addressables.LoadAssetAsync<BackgroundDataSO>("Assets/AddressableResources/Backgrounds/BackgroundData.asset");
        yield return backgroundSOHandler;

        if (backgroundSOHandler.Status == AsyncOperationStatus.Succeeded)
        {
            BackgroundDataSO dataSO = backgroundSOHandler.Result;
            for (int i = 0; i < backgroundAddresses.Length; i++)
            {
                AsyncOperationHandle<IList<Sprite>> backgroundHandler = Addressables.LoadAssetAsync<IList<Sprite>>(backgroundAddresses[i]);
                yield return backgroundHandler;

                if (backgroundHandler.Status == AsyncOperationStatus.Succeeded)
                {
                    dataSO.backgrounds[i].backgroundSprites = backgroundHandler.Result.ToArray();
                }
                else
                {
                    Debug.Log($"Cannot load {backgroundAddresses[i]}");
                }
            }

            PlayerData.Instance.SetBackgroundDataSO(dataSO);
            onCompleted?.Invoke();
        }
        else
        {
            Debug.Log($"Cannot load BackgroundSO");
        }

        Addressables.Release(backgroundSOHandler);
    }

    Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
    private IEnumerator LoadItemSpriteRoutine(ICallback.CallFunc onCompleted)
    {
        AsyncOperationHandle<IList<Sprite>> spriteHandler = Addressables.LoadAssetsAsync<Sprite>("GameItem", null);
        yield return spriteHandler;

        if (spriteHandler.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite[] sprites = spriteHandler.Result.ToArray();
            foreach (Sprite sprite in sprites)
            {
                if (!spriteDict.ContainsKey(sprite.name))
                    spriteDict.Add(sprite.name, sprite);
            }
            PlayerData.Instance.SetGameItemSpriteDict(spriteDict);
            onCompleted?.Invoke();
        }
        else
        {
            Debug.Log("Cannot load Item Sprite ");
        }
    }

    Dictionary<string, Sprite[]> animDict = new Dictionary<string, Sprite[]>();
    private AsyncOperationHandle<IList<Sprite>> animHandle;

    private IEnumerator LoadItemAnimRoutine(ICallback.CallFunc onCompleted)
    {
        //Caching.ClearCache();
        AsyncOperationHandle<IList<IResourceLocation>> locationHandler = Addressables.LoadResourceLocationsAsync("ItemAnim");
        yield return locationHandler;

        if (locationHandler.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (IResourceLocation resourceLocation in locationHandler.Result)
            {
                /*AsyncOperationHandle<IList<Sprite>> */
                animHandle = Addressables.LoadAssetAsync<IList<Sprite>>(resourceLocation.PrimaryKey);
                yield return animHandle;

                if (animHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (!animDict.ContainsKey(Path.GetFileNameWithoutExtension(resourceLocation.PrimaryKey)))
                        animDict.Add(Path.GetFileNameWithoutExtension(resourceLocation.PrimaryKey), animHandle.Result.ToArray());
                }
                else
                {
                    Debug.Log($"Cannot load {resourceLocation.PrimaryKey}");
                }
                Addressables.Release(animHandle);
            }
            PlayerData.Instance.SetGameItemAnimDict(animDict);
            onCompleted?.Invoke();
        }
        else
        {
            Debug.Log("Cannot load anim address");
        }
    }

    private IEnumerator LoadUISpriteRoutine(ICallback.CallFunc onCompleted)
    {
        AsyncOperationHandle<IList<Sprite>> spriteHandle = Addressables.LoadAssetsAsync<Sprite>("UI", null);
        yield return spriteHandle;

        if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
            foreach (Sprite sprite in spriteHandle.Result)
            {
                dict.Add(sprite.name, sprite);
            }

            PlayerData.Instance.SetUISpriteDict(dict);
            onCompleted?.Invoke();
        }
        else
        {
            Debug.LogError("Cannot load spriteUI");
        }
    }

    //private IEnumerator LoadSpriteAtlasRoutine(ICallback.CallFunc onCompleted)
    //{
    //    AsyncOperationHandle<IList<SpriteAtlas>> spriteAtlasHandler = Addressables.LoadAssetsAsync<SpriteAtlas>("SpriteAtlas", null);
    //    yield return spriteAtlasHandler;

    //    if (spriteAtlasHandler.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        Dictionary<string, SpriteAtlas> atlasDict = new Dictionary<string, SpriteAtlas>();
    //        foreach (SpriteAtlas spriteAtlas in spriteAtlasHandler.Result)
    //        {
    //            atlasDict.Add(spriteAtlas.name, spriteAtlas);
    //        }

    //        PlayerData.Instance.SetSpriteAtlas(atlasDict);
    //        onCompleted?.Invoke();
    //    }
    //    else
    //    {
    //        Debug.Log("Cannot load Sprite atlas");
    //    }
    //}
}
