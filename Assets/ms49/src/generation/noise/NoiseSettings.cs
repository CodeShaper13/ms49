using UnityEngine;
using System;

[Serializable]
public class NoiseSettings {

    [SerializeField, Min(0)]
    private float _scale = 20f;
    [SerializeField, Min(0)]
    private float _lacunarity = 2f;
    [SerializeField, Min(0)]
    private float _persistance = 0.5f;
    [SerializeField, Range(1, 10)]
    private int _octaves = 3;

    public float scale => this._scale;
    public float lacunarity => this._lacunarity;
    public float persistance => this._persistance;
    public int octaves => this._octaves;
}
