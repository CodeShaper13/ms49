public class PopupConfirmQuit : PopupWindow {

    public void callback_exit() {
        Main.instance.ShutdownWorld();
    }
}
