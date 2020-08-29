using UnityEngine;

public class PopupPause : PopupWorldReference {

    public void callback_resume() {
        this.close();
    }

    public void callback_saveGame() {
        this.world.saveGame();

        this.close();
    }
}
