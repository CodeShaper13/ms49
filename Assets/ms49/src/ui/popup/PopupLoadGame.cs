using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PopupLoadGame : PopupWindow {

    [SerializeField]
    private RectTransform worldTileWrapperObj = null;
    [SerializeField]
    private GameObject saveTilePrefab = null;

    protected override void onOpen() {
        base.onOpen();

        List<SaveFile> cachedSaves = Main.instance.getAllSaves(true);

        foreach(SaveFile save in cachedSaves) {
            GameObject g = GameObject.Instantiate(this.saveTilePrefab, this.worldTileWrapperObj);
            g.GetComponent<LoadSaveButton>().init(save);
        }
    }

    protected override void onClose() {
        base.onClose();

        foreach(Transform t in this.worldTileWrapperObj) {
            GameObject.Destroy(t.gameObject);
        }
    }

    public void callback_openSaveFolder() {
        EditorUtility.RevealInFinder(Main.SAVE_DIR);
    }
}
