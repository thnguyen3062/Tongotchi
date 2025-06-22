using Core.Utils;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private bool ddol = true;
    [SerializeField] private AudioClip[] m_AudioClips;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource vfxSound;
    private bool isMute;
    private ICallback.CallFunc2<bool> onSoundChange;
    public bool IsMute => isMute;

    private void Awake()
    {
        if (Instance == null)
        {
            if (ddol) DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isMute = !PlayerData.Instance.data.isSoundOn;
        backgroundMusic.volume = isMute ? 0 : 1;
        vfxSound.volume = isMute ? 0 : 1;
        PlayBackgroundMusic("01. Background");
        onSoundChange?.Invoke(PlayerData.Instance.data.isSoundOn);
    }

    public void AddSoundChangeCallback(ICallback.CallFunc2<bool> callback)
    {
        onSoundChange += callback;
    }

    public void PlayVFX(string audioName)
    {
        LoggerUtil.Logging("PLAY_VFX", $"Audio name: {audioName}");
        try
        {
            vfxSound.clip = GetAudioClip(audioName);
            vfxSound.Play();
        }
        catch(System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void PlayBackgroundMusic(string audioName, bool isLoop = true)
    {
        backgroundMusic.clip = GetAudioClip(audioName);
        backgroundMusic.Play();
        backgroundMusic.loop = isLoop;
    }

    public void ToggleSoundSetting()
    {
        PlayerData.Instance.data.isSoundOn = !PlayerData.Instance.data.isSoundOn;
        vfxSound.volume = PlayerData.Instance.data.isSoundOn ? 1 : 0;
        backgroundMusic.volume = PlayerData.Instance.data.isSoundOn ? 1 : 0;
        onSoundChange?.Invoke(PlayerData.Instance.data.isSoundOn);
    }

    private AudioClip GetAudioClip(string audioName)
    {
        foreach(AudioClip clip in m_AudioClips)
        {
            if(clip.name.Equals(audioName))
                return clip;
        }
        Debug.LogError($"No clip with name {audioName} exist!");
        return null;
    }
}
