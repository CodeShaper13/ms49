using System;
using UnityEngine;

public static class Pause {

    private static float oldTimeScale;

    public static bool IsPaused { get; private set; }

    [Obsolete("Use property instead")]
    public static bool isPaused() {
        return Pause.IsPaused;
    }

    public static void PauseGame() {
        if(!Pause.IsPaused) {
            Pause.IsPaused = true;

            Pause.oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    public static void UnpauseGame() {
        if(Pause.IsPaused) {
            Pause.IsPaused = false;

            Time.timeScale = Pause.oldTimeScale;
        }
    }
}
