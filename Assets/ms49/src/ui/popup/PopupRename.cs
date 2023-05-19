using TMPro;
using UnityEngine;

public class PopupRename : PopupWindow {

    [SerializeField]
    private TMP_InputField _inputField = null;

    private SaveFile save;

    protected override void onOpen() {
        base.onOpen();
    }

    public void setTargetSave(SaveFile save) {
        this.save = save;

        this._inputField.text = save.saveName;
    }

    public void callback_rename() {
        this.save.rename(this._inputField.text);

        this.Close();
    }
}
