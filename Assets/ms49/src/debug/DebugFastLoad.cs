using fNbt;
using System.IO;
using UnityEngine;

public class DebugFastLoad : MonoBehaviour {

    [Tooltip("If blank, the game is not saved.")]
    public string saveName = "temp";
    public NewWorldSettings settings = null;

    private void Start() {
#if UNITY_EDITOR
        // Load world instantly.
        string path = Main.SAVE_DIR + this.saveName + Main.SAVE_EXTENSION;
        if(File.Exists(path)) {
            NbtFile nbtFile = new NbtFile();
            nbtFile.LoadFromFile(path);

            Main.instance.StartWorld(name, nbtFile.RootTag);
        }
        else {
            Main.instance.StartWorld(
                this.saveName,
                this.settings);
        }
    }
#endif
}