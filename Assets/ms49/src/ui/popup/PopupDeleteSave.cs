using UnityEngine;
using UnityEngine.UI;

public class PopupDeleteSave : PopupWindow {

    [SerializeField]
    private Text textSaveName = null;
    [SerializeField]
    private PopupLoadGame _loadGamePopup = null;

    private SaveFile save;

    protected override void onClose() {
        base.onClose();

        this._loadGamePopup.openAdditive();
    }

    public void setTargetSave(SaveFile save) {
        this.save = save;

        this.textSaveName.text = this.save.saveName;
    }

    public void callback() {
        this.save.delete();

        this._loadGamePopup.open();
    }
}
