using UnityEngine;
using System.Collections.Generic;

public class PopupLoadGame : PopupWindow {

    [SerializeField]
    private RectTransform worldTileWrapperObj = null;
    [SerializeField]
    private GameObject saveTilePrefab = null;

    protected override void onOpen() {
        base.onOpen();

        List<string> cachedSaves = Main.instance.getAllSaves(true);

        foreach(string save in cachedSaves) {
            GameObject g = GameObject.Instantiate(this.saveTilePrefab, this.worldTileWrapperObj);
            g.GetComponent<LoadSaveButton>().init(save);
        }

        this.worldTileWrapperObj.sizeDelta = new Vector2(this.worldTileWrapperObj.sizeDelta.x, (cachedSaves.Count * 130) + 10);
    }

    protected override void onClose() {
        base.onClose();

        foreach(Transform t in this.worldTileWrapperObj) {
            GameObject.Destroy(t.gameObject);
        }
    }
}
