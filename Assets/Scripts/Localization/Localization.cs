using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public static Localization Instance { get; private set; }

    private string currentLanguageAlias = "en";
    private Dictionary<string, Language> languages = new Dictionary<string, Language>();

    private List<L10nText> l10NTexts = new List<L10nText>();

    public string CurrentLanguage
    {
        get
        {
            return languages[currentLanguageAlias].name;
        }
    }
    public List<string> Languages {
        get
        {
            var languageList = new List<string>();
            foreach(var language in languages)
                languageList.Add(language.Value.name);
            return languageList;
        }
    }

    void Awake()
    {
        Instance = this;
        LoadConfig();
        LoadLanguages();
    }

    public string GetText(string alias)
    {
        var language = languages[currentLanguageAlias];
        // That the dict not contains key returns alias.
        if(!language.text.ContainsKey(alias))
            return alias;
        return language.text[alias];
    }
    public void SwitchLanguage(string name)
    {
        foreach(var language in languages)
        {
            if (language.Value.name == name)
            {
                currentLanguageAlias = language.Key;
                Config.Instance.SetValue("Localization", "Language", currentLanguageAlias);
            }
        }
        UpdateL10nText();
    }

    private void LoadConfig()
    {
        var alias = Config.Instance.GetValue("Localization", "Language", currentLanguageAlias);
        currentLanguageAlias = alias;
    }
    private void LoadLanguages()
    {
        var languageJsons = Resources.LoadAll<TextAsset>(Path.Languages);
        foreach(var languageJson in languageJsons)
        {
            var language = JsonConvert.DeserializeObject<Language>(languageJson.text);
            languages[languageJson.name] = language;
        }
    }

    public void SubscribeL10nText(L10nText text)
    {
        l10NTexts.Add(text);
    }
    public void UnsubcribeL10nText(L10nText text)
    {
        l10NTexts.Remove(text);
    }
    private void UpdateL10nText()
    {
        foreach (var text in l10NTexts)
            text.UpdateText();
    }
}
