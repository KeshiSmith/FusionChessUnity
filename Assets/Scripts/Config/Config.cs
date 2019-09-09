using UnityEngine;

public class Config : MonoBehaviour
{
    public static Config Instance { get; private set; }

    private IniParser iniParser;

    void Awake()
    {
        Instance = this;
        iniParser = new IniParser(Path.ConfigFileName);
    }

    public string GetValue(string section, string key, string defaultValue="")
    {
        return iniParser.GetValue(section, key, defaultValue);
    }
    public void SetValue(string section, string key, string value)
    {
        iniParser.SetValue(section, key, value);
    }
}
