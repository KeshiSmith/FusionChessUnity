using UnityEngine;

namespace Android
{
    public class Content
    {
        public static AndroidJavaObject GetCurrentActivity()
        {
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            return currentActivity;
        }
    }
}
