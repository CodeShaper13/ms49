using UnityEngine;

public class PopupConfirmQuit : PopupWindow {

    public void callback_quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
