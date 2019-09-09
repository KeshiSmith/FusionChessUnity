using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio Instance { get; private set; }

    public AudioSource musicSource, soundSource;
    public AudioClip[] sounds;

    private bool isMute = false;
    [Range(0, 100)]
    private int musicVolume = 100;
    [Range(0, 100)]
    private int soundVolume = 100;

    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();

    public bool Mute
    {
        get
        {
            return isMute;
        }
        set
        {
            isMute = value;
            UpdateMute();
        }
    }
    public int MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;
            UpdateMusicVolumn();
        }
    }
    public int SoundVolume
    {
        get
        {
            return soundVolume;
        }
        set
        {
            soundVolume = value;
            UpdateSoundVolumn();
        }
    }

    void Awake()
    {
        Instance = this;
        LoadConfig();
    }
    void Start()
    {
        LoadSounds();
    }

    public void PlaySound(string name)
    {
        if(soundDict.ContainsKey(name))
            soundSource.PlayOneShot(soundDict[name]);
    }

    private void LoadConfig()
    {
        var config = Config.Instance;
        var mute = config.GetValue("Audio", "Mute", "0");
        Mute = (mute == "1");
        var musicValume = config.GetValue("Audio", "MusicVolumn", "100");
        MusicVolume = int.Parse(musicValume);
        var soundValume = config.GetValue("Audio", "SoundVolumn", "100");
        SoundVolume = int.Parse(soundValume);
    }
    private void LoadSounds()
    {
        foreach (var sound in sounds)
            soundDict[sound.name] = sound;
    }
    private void UpdateMute()
    {
        musicSource.mute = Mute;
        soundSource.mute = Mute;

        if (!Mute)
            musicSource.Play();

        var config = Config.Instance;
        var mute = Mute ? "1" : "0";
        config.SetValue("Audio", "Mute", mute);
    }
    private void UpdateMusicVolumn()
    {
        musicSource.volume = MusicVolume / 100.0f;

        var config = Config.Instance;
        var volumn = MusicVolume.ToString();
        config.SetValue("Audio", "MusicVolumn", volumn);
    }
    private void UpdateSoundVolumn()
    {
        soundSource.volume = SoundVolume / 100.0f;

        var config = Config.Instance;
        var volumn = SoundVolume.ToString();
        config.SetValue("Audio", "SoundVolumn", volumn);
    }
}
