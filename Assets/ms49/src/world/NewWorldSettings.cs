using System;
using UnityEngine;

[Serializable]
public class NewWorldSettings {

    [SerializeField]
    private int _mapSize;
    [SerializeField]
    private string _seed;
    [SerializeField]
    private bool _creativeEnabled;

    public bool creativeEnabled => this._creativeEnabled;

    public NewWorldSettings(string seed, int mapSize, bool creativeEnabled) {
        this._seed = seed;
        this._mapSize = mapSize;
        this._creativeEnabled = creativeEnabled;
    }

    public int getMapSize() {
        return (int)Mathf.Pow(2, Mathf.Clamp(this._mapSize, 0, 2) + 5);
    }

    public int getSeed() {
        return this._seed.GetHashCode();
    }
}
