using fNbt;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaveButton : MonoBehaviour {

    [SerializeField]
    private Text saveNameText = null;

    private string savePath;
    private PopupWindow popup;

    private void Awake() {
        this.popup = this.GetComponentInParent<PopupWindow>();
    }

    public void init(string savePath) {
        this.savePath = savePath;

        string s = Path.GetFileNameWithoutExtension(this.savePath);
        string date = Directory.GetLastWriteTime(this.savePath).ToShortDateString();

        this.saveNameText.text = s + " " + date;
    }

    public void callback_load() {
        this.popup.close();

        NbtFile nbtFile = new NbtFile();
        nbtFile.LoadFromFile(savePath);

        Main.instance.createWorld(
            Path.GetFileNameWithoutExtension(savePath),
            nbtFile.RootTag);
    }
}
