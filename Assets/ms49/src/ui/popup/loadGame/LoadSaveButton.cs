using fNbt;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaveButton : MonoBehaviour {

    [SerializeField]
    private Text saveNameText = null;

    private SaveFile saveFile;

    public void init(SaveFile saveFile) {
        this.saveFile = saveFile;

        this.saveNameText.text =
            this.saveFile.saveName +
            " " +
            this.saveFile.lastPlayTime.ToShortDateString();
    }

    public void callback_load() {
        this.GetComponentInParent<PopupWindow>().close();

        NbtFile nbtFile = new NbtFile();
        nbtFile.LoadFromFile(this.saveFile.path);

        Main.instance.StartWorld(
            Path.GetFileNameWithoutExtension(this.saveFile.path),
            nbtFile.RootTag);
    }

    public void callback_rename() {
        PopupRename popup = Main.instance.findPopup<PopupRename>();
        if(popup != null) {
            popup.open();
            popup.setTargetSave(this.saveFile);
        }
    }

    public void callback_delete() {
        PopupDeleteSave popup = Main.instance.findPopup<PopupDeleteSave>();
        if(popup != null) {
            popup.open();
            popup.setTargetSave(this.saveFile);
        }
    }
}
