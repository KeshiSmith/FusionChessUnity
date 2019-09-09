using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class IniParser
{
    private bool opened = false;
    private string filePath = null;
    private Dictionary<string, Dictionary<string, string>> datas
        = new Dictionary<string, Dictionary<string, string>>();

    public bool Opened
    {
        get
        {
            return opened;
        }
    }
    public string[] Sections
    {
        get
        {
            var keys = new List<string>();
            foreach (string key in datas.Keys)
                keys.Add(key);
            return keys.ToArray();
        }
    }

    public IniParser()
    {
    }
    public IniParser(string filePath)
    {
        Open(filePath);
    }
    ~IniParser()
    {
        if (opened)
            Close();
    }

    public void Open(string fileName)
    {
        filePath = Path.DataPath + fileName;
        LoadDataFormFile();
        opened = true;
    }
    public void Close()
    {
        SaveData();
        filePath = null;
        datas.Clear();
        opened = false;
    }
    public string GetValue(string section, string key, string defaultValue = "")
    {
        Assert.AreEqual(true, opened);
        if (datas.ContainsKey(section) && datas[section].ContainsKey(key))
            return datas[section][key];
        SetValue(section, key, defaultValue);
        return defaultValue;
    }
    public void SetValue(string section, string key, string value)
    {
        Assert.AreEqual(true, opened);
        if (!datas.ContainsKey(section))
            datas[section] = new Dictionary<string, string>();
        datas[section][key] = value;
    }

    private void LoadDataFormFile()
    {
        var fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        using (var streamReader = new StreamReader(fileStream))
        {
            string line = null, section = null;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (isBlankLine(line) || isComments(line))
                    continue;
                if (isSection(line))
                {
                    // Add new Section
                    section = line.Substring(1, line.Length - 2).Trim();
                    datas[section] = new Dictionary<string, string>();
                }
                else
                {
                    // Key-Value line must Contains '='
                    var index = line.IndexOf('=');
                    if (index == -1)
                        continue;
                    // Add new Key-Value
                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1, line.Length - index - 1).Trim();
                    datas[section][key] = value;
                }
            }
        }
        fileStream.Close();
    }
    private bool isBlankLine(string line)
    {
        return line.Length <= 0;
    }
    private bool isComments(string line)
    {
        return line.StartsWith(";");
    }
    private bool isSection(string line)
    {
        return line.StartsWith("[") && line.EndsWith("]");
    }
    private void SaveData()
    {
        // Delete old file.
        if (File.Exists(filePath))
            File.Delete(filePath);
        // Rewrite new file.
        var fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        using (var streamWriter = new StreamWriter(fileStream))
        {
            foreach (string section in datas.Keys)
            {
                var data = datas[section];
                if (data.Count != 0)
                {
                    streamWriter.WriteLine(string.Format("[{0}]", section));
                    foreach (var key in datas[section].Keys)
                        streamWriter.WriteLine(string.Format("{0} = {1}", key, data[key]));
                }
            }
            streamWriter.Flush();
        }
        fileStream.Close();
    }
}
