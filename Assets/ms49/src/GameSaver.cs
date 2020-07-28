using UnityEngine;
using System.IO;
using fNbt;

public class GameSaver : MonoBehaviour {

    public const string SAVE_FILE_NAME = "saveGame01.nbt";

    public bool tryLoadSave = true;

    private void Start() {
        World world = GameObject.FindObjectOfType<World>();

        if(this.tryLoadSave && File.Exists(GameSaver.SAVE_FILE_NAME)) {
            NbtCompound rootTag = this.loadGame();
            world.initialize(rootTag);
        } else {
            world.initialize(null);
        }
    }

    public NbtCompound loadGame() {
        NbtFile nbtFile = new NbtFile();
        nbtFile.LoadFromFile(GameSaver.SAVE_FILE_NAME);

        return nbtFile.RootTag;
    }

    public void saveGame(string saveName) {
        NbtCompound nbt = new NbtCompound();
        World world = GameObject.FindObjectOfType<World>();
        NbtCompound rootTag = new NbtCompound("root");
        rootTag.Add(world.writeToNbt());
        NbtFile nbtFile = new NbtFile(rootTag);
        nbtFile.SaveToFile(saveName, NbtCompression.None);
    }
}
