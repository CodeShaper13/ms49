using NaughtyAttributes;
using UnityEngine;

public class FeatureRiver : FeatureBase {

    [SerializeField, Required]
    private CellData _cell = null;
    public float _riverSize = 2;
    public float _riverNoiseFrequency = 0.05f;
    public float _riverNoiseAmplitude = 9;

    [Space]

    [SerializeField]
    private int _editorSeed = 0;

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        int seed = rnd.Next();

        for(int x = 0; x < accessor.size; x++) {
            // River
            float riverY = this.GetRiverY(x, seed) + this._riverSize;
            for(int y = 0; y < riverY; y++) {
                accessor.SetCell(x, y, this._cell);
            }
        }
    }

    private float GetRiverY(float x, int seed) {
        return Mathf.PerlinNoise(x * this._riverNoiseFrequency, seed) * this._riverNoiseAmplitude;
    }

    private void OnDrawGizmosSelected() {
        System.Random rnd = new System.Random(this._editorSeed);
        int seed = rnd.Next();
        Gizmos.color = Color.blue;

        for(int x = 0; x < 128; x++) {
            // River
            float riverY = this.GetRiverY(x, seed);
            for(int y = 0; y < riverY; y++) {
                Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
            }
        }
    }
}