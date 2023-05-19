public class PopupPause : PopupWorldReference {

    protected override void onOpen() {
        base.onOpen();

        Pause.PauseGame();
    }

    protected override void onClose() {
        base.onClose();

        Pause.UnpauseGame();
    }

    public void Callback_SaveGame() {
        this.world.SaveGame();

        this.Close();
    }

    public void Callback_ExitGame() {
        PopupConfimDialog dialog = Main.instance.FindPopup<PopupConfimDialog>();
        if(dialog != null) {
            dialog.headerText = "Exit Game?";
            dialog.messageText = "Progress will not be saved.";
            dialog.yesCallback = Main.instance.ShutdownWorld;
            dialog.openAdditive();
        }
    }
}
