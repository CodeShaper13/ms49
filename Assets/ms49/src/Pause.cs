using UnityEngine;

public static class Pause {

    private static bool flag = false;
    private static float oldTimeScale;

    public static bool isPaused() {
        return Pause.flag;
    }

    public static void pause() {
        if(!Pause.isPaused()) {
            Pause.flag = true;

            Pause.oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    public static void unPause() {
        if(Pause.isPaused()) {
            Pause.flag = false;

            Time.timeScale = Pause.oldTimeScale;
        }
    }
}
