using UnityEngine;

public class PopupPause : PopupWindow {

    public void callback_resume() {
        this.close();
    }

    public void callback_saveGame() {
        GameObject.FindObjectOfType<GameSaver>().saveGame(GameSaver.SAVE_FILE_NAME);
        this.close();
    }
}
