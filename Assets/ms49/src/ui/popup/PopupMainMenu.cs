using TMPro;
using UnityEngine;

public class PopupMainMenu : PopupWindow {

    [SerializeField]
    private TMP_Text _textVersion = null;

    private void Awake() {
        if(this._textVersion != null) {
            this._textVersion.text = string.Format("Version: {0}", Application.version);
        }
    }

    public void Callback_QuitGame() {
        PopupConfimDialog dialog = Main.instance.FindPopup<PopupConfimDialog>();
        if(dialog != null) {
            dialog.headerText = "Quit?";
            dialog.messageText = "Quit Game?";
            dialog.yesCallback = ExitGame;
            dialog.openAdditive();
        }
    }

    private void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
