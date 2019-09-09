using UnityEngine;

public class Path : MonoBehaviour
{
    public static string DataPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return Application.persistentDataPath + "/";
            return "";
        }
    }
    public static string ConfigFileName = "Fusion Chess.ini";

    public static string Languages = "Languages";
    public static string Sounds = "Sounds";
}
