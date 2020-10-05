using System.IO;
using System;

public class SaveFile {

    public string path { get; private set; }

    public string saveName => Path.GetFileNameWithoutExtension(this.path);
    public DateTime lastPlayTime => Directory.GetLastWriteTime(this.path);

    public SaveFile(string path) {
        this.path = path;
    }

    public void delete() {
        //try {
            File.Delete(this.path);
        //} catch (IOException) {
        //    Debug.LogWarning("Could not delete save " + this.saveName);
        //}
    }

    public void rename(string newName) {
        File.Move(this.path, Main.SAVE_DIR + newName + Main.SAVE_EXTENSION);
    }
}
