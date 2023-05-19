using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopupLoadGame : PopupWindow {

    [SerializeField]
    private GameObject _entryPrefab = null;
    [SerializeField]
    private RectTransform _entryParent = null;
    [SerializeField]
    private Selectable[] _buttons = null;

    private List<SaveListEntry> saveEntires;
    [HideInInspector]
    public SaveListEntry selectedEntry;

    private void Awake() {
        this.saveEntires = new List<SaveListEntry>();
    }

    protected override void onOpen() {
        base.onOpen();

        List<SaveFile> cachedSaves = Main.instance.GetAllSaves(true);

        foreach(SaveFile save in cachedSaves) {
            SaveListEntry entry = GameObject.Instantiate(
                this._entryPrefab,
                this._entryParent).GetComponent<SaveListEntry>();
            entry.gameObject.SetActive(true);
            entry.SetSave(save);

            this.saveEntires.Add(entry);
        }
    }

    protected override void onUpdate() {
        base.onUpdate();
    
        foreach(var btn in this._buttons) {
            btn.interactable = this.selectedEntry != null;
        }
    }

    protected override void onClose() {
        base.onClose();

        this.selectedEntry = null;
        foreach(SaveListEntry entry in this.saveEntires) {
            GameObject.Destroy(entry.gameObject);
        }
        this.saveEntires.Clear();
    }

    public void Callback_RenameSave() {
        if(this.selectedEntry == null) {
            return;
        }

        PopupRename popup = Main.instance.FindPopup<PopupRename>();
        if(popup != null) {
            popup.open();
            popup.setTargetSave(this.selectedEntry.saveFile);
        }
    }

    public void Callback_DeleteSave() {
        if(this.selectedEntry == null) {
            return;
        }

        PopupConfimDialog popup = Main.instance.FindPopup<PopupConfimDialog>();
        if(popup != null) {
            popup.headerText = "Delete Save";
            popup.messageText = string.Format("Delete \"{0}\"?\nIt will be deleted permanently!", this.selectedEntry.saveFile.saveName);
            popup.yesCallback = this.selectedEntry.saveFile.delete;

            popup.openAdditive();
        }
    }

    public void Callback_OpenSaveFolder() {
        // TODO this won't work in a build.
#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(Main.SAVE_DIR);
#endif
    }
}
