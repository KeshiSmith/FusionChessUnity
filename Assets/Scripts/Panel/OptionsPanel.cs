using System.Collections.Generic;
using UnityEngine.UI;

public class OptionsPanel : AbstractPanel
{
    public Dropdown languageDropdown;
    public Toggle muteToogle;
    public Slider musicVolumnSlider, soundVolumnSlider;

    private bool init = false;
    private List<string> languages;

    public int LanguageIndex
    {
        set
        {
            Language = languages[value];
        }
    }
    public bool Mute{
        get
        {
            return Audio.Instance.Mute;
        }
        set
        {
            Audio.Instance.Mute = value;
        }
    }
    public float MusicVolumn
    {
        get
        {
            return Audio.Instance.MusicVolume/20;
        }
        set
        {
            Audio.Instance.MusicVolume = (int)(value * 20);
        }
    }
    public float SoundVolumn
    {
        get
        {
            return Audio.Instance.SoundVolume/20;
        }
        set
        {
            Audio.Instance.SoundVolume = (int)(value * 20);
            PlayTestSound();
        }
    }

    private string Language
    {
        get
        {
            return Localization.Instance.CurrentLanguage;
        }
        set
        {
            Localization.Instance.SwitchLanguage(value);
        }
    }

    void Start()
    {
        InitDate();
    }

    public void Back()
    {
        Finish();
    }

    private void InitDate()
    {
        languages = Localization.Instance.Languages;
        languageDropdown.AddOptions(languages);
        var currentLanguage = Localization.Instance.CurrentLanguage;
        languageDropdown.value = languages.IndexOf(currentLanguage);

        muteToogle.isOn = Mute;
        musicVolumnSlider.value = MusicVolumn;
        soundVolumnSlider.value = SoundVolumn;
    }
    private void PlayTestSound()
    {
        if (init)
            Audio.Instance.PlaySound("check");
        else
            init = true;
    }
}
