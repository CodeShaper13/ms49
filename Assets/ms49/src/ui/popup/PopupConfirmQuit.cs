using UnityEngine;

public class PopupConfirmQuit : PopupWindow {

    public override bool pauseGameWhenOpen() {
        return true;
    }

    public void callback_quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void callback_cancel() {
        UiManager.singleton.popupPause.open();
    }
}
