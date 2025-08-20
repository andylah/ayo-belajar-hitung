using UnityEngine;
using UnityEngine.InputSystem; // Input System (new)

public class BackButtonHandler : MonoBehaviour
{
    public float backDelay = 2f; // waktu window double-press (detik)

    private float lastBackTime;
    private bool waitingForSecondPress = false;

    private static BackButtonHandler instance;

    private void Awake()
    {
        // Singleton: pastikan hanya ada 1 instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Pastikan kita tidak dapat warning "DontDestroyOnLoad only works for root GameObjects"
        // Gunakan transform.root untuk mengunci root GameObject apapun tempat script ini dipasang
        DontDestroyOnLoad(transform.root.gameObject);
    }

    private void Update()
    {
        bool backPressed = false;

        // 1) Coba pakai Input System (Keyboard.current)
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.escapeKey.wasPressedThisFrame)
                backPressed = true;
        }

        // 2) Fallback ke legacy Input (berguna di Android: KeyCode.Escape)
        if (!backPressed && Input.GetKeyDown(KeyCode.Escape))
            backPressed = true;

        if (backPressed)
            HandleBackPressed();
    }

    private void HandleBackPressed()
    {
        if (!waitingForSecondPress)
        {
            // pertama kali tekan -> tampilkan toast native
            waitingForSecondPress = true;
            lastBackTime = Time.time;
            ShowAndroidToast("Tekan sekali lagi untuk keluar");
            CancelInvoke(nameof(ResetWaiting));
            Invoke(nameof(ResetWaiting), backDelay);
        }
        else
        {
            // tekan kedua dalam waktu -> keluar aplikasi
            if (Time.time - lastBackTime <= backDelay)
            {
                ExitApp();
            }
            else
            {
                // sudah lewat waktu, anggap seperti tekan pertama lagi
                waitingForSecondPress = true;
                lastBackTime = Time.time;
                ShowAndroidToast("Tekan sekali lagi untuk keluar");
                CancelInvoke(nameof(ResetWaiting));
                Invoke(nameof(ResetWaiting), backDelay);
            }
        }
    }

    private void ResetWaiting()
    {
        waitingForSecondPress = false;
    }

    private void ExitApp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = up.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity != null)
            {
                // Move task to back (baik) lalu finish activity
                activity.Call<bool>("moveTaskToBack", true);
                activity.Call("finish");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("ExitApp Android exception: " + e);
        }
#endif
        Application.Quit();
    }

    private void ShowAndroidToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = up.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                int length = toastClass.GetStatic<int>("LENGTH_SHORT");

                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", activity, message, length);
                    toast.Call("show");
                }));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("ShowAndroidToast failed: " + e);
        }
#else
        // Editor / non-Android fallback
        Debug.Log("[Toast] " + message);
#endif
    }
}
