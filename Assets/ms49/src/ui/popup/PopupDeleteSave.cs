using UnityEngine;
using UnityEngine.UI;

public class PopupDeleteSave : PopupWindow {

    [SerializeField]
    private Text textSaveName = null;

    private SaveFile save;

    public void setTargetSave(SaveFile save) {
        this.save = save;

        this.textSaveName.text = this.save.saveName;
    }

    public void callback() {
        this.save.delete();
        this.close();
    }
}
