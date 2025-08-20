using UnityEngine;

public class AndroidToast
{
    static AndroidJavaClass pluginClass;

    static AndroidToast()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            pluginClass = new AndroidJavaClass("com.hartaku.ayobelajarhitung.ToastPlugin");
            pluginClass.CallStatic("setActivity", activity);
        }
#endif
    }

    public static void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        pluginClass.CallStatic("showToast", message);
#else
        Debug.Log("Toast: " + message);
#endif
    }
}