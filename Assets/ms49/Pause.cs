using UnityEngine;

public static class Pause {

    private static bool flag = false;
    private static float oldTimeScale;

    public static bool isPaused() {
        return Pause.flag;
    }

    public static void pause() {
        Pause.func(true);
    }

    public static void unPause() {
        Pause.func(false);
    }

    private static void func(bool isPaused) {
        if(isPaused) {
            Pause.flag = true;

            Pause.oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else {
            Pause.flag = false;

            Time.timeScale = Pause.oldTimeScale;
        }
    }
}
