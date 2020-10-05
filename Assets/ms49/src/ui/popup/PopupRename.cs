using UnityEngine;

public class PopupRename : PopupWindow {

    [SerializeField]
    private InputFieldSaveName inputSaveName = null;

    private SaveFile save;

    protected override void onOpen() {
        base.onOpen();
    }

    public void setTargetSave(SaveFile save) {
        this.save = save;

        this.inputSaveName.text = save.saveName;
    }

    public void callback_rename() {
        this.save.rename(inputSaveName.text);

        this.close();
    }
}
