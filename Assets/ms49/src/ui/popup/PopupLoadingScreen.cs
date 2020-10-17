using UnityEngine;
using UnityEngine.UI;

public class PopupLoadingScreen : PopupWindow {

    [SerializeField]
    private Text _textStatus = null;

    public void updateStatus(string msg) {
        if(this._textStatus != null) {
            this._textStatus.text = msg;
        }
    }
}
