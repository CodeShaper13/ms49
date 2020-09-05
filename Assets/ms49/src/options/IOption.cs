using UnityEngine;

public interface IOption {

    string name { get; set; }

    GameObject controlObj { get; set; }

    void setupControlObj(GameObject obj);

    /// <summary>
    /// Applies the option's value to game.  This makes the option
    /// take effect.
    /// </summary>
    void applyValue();

    /// <summary>
    /// Writes the option's value to disk.
    /// </summary>
    void write();

    /// <summary>
    /// Reads the option's value from disk.
    /// </summary>
    void read();
}
