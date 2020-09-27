using System;
using UnityEngine;

[Serializable]
public class NewWorldSettings {

    [SerializeField]
    private EnumMapSize _mapSize;
    [SerializeField]
    private string _seed;
    [SerializeField]
    private bool _creativeEnabled;

    public EnumMapSize mapSize => this._mapSize;
    public bool creativeEnabled => this._creativeEnabled;

    public NewWorldSettings(string seed, EnumMapSize mapSize, bool creativeEnabled) {
        this._seed = seed;
        this._mapSize = mapSize;
        this._creativeEnabled = creativeEnabled;
    }

    public int getSeed() {
        return this._seed.GetHashCode();
    }
}
