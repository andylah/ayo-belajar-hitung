package com.hartaku.ayobelajarhitung;

import android.widget.Toast;
import com.unity3d.player.UnityPlayer;
import android.app.Activity;

public class ToastPlugin {
    private static Activity activity;

    public static void setActivity(Activity act) {
        activity = act;
    }

    public static void showToast(final String message) {
        if (activity != null) {
            activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(activity, message, Toast.LENGTH_SHORT).show();
                }
            });
        }
    }
}