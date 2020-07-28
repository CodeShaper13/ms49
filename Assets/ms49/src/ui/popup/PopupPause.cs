using UnityEngine;

public class PopupPause : PopupWindow {

    public override bool pauseGameWhenOpen() {
        return true;
    }

    public void callback_resume() {
        this.close();
    }

    public void callback_saveGame() {
        GameObject.FindObjectOfType<GameSaver>().saveGame(GameSaver.SAVE_FILE_NAME);
        this.close();
    }

    public void callback_quit() {
        UiManager.singleton.popupConfirmQuit.open();
    }


    public void callback_options() {
        UiManager.singleton.popupOptions.open();
    }
}
