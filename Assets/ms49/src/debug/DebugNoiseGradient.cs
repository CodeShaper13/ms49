using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class DebugNoiseGradient : MonoBehaviour {

    [SerializeField]
    private int _mapSize = 96;
    [SerializeField]
    private string _seed = "";
    [SerializeField]
    private Gradient gradient = null;

    [Space]

    public Tilemap tilemap;
    public TileBase tile;

    [Space]

    [SerializeField]
    public NoiseSettings noiseSettings;

    private void Update() {
        float[,] noiseMap = NoisemapGenerator.computeNoiseMap(
            this.noiseSettings,
            this._seed.GetHashCode(),
            this._mapSize,
            0);

        // Color the tilemap.
        for(int x = 0; x < this._mapSize; x++) {
            for(int y = 0; y < this._mapSize; y++) {
                float f = noiseMap[x, y];

                Color c = gradient.Evaluate(f);
                //c = new Color(f, f, f);

                this.tilemap.SetColor(new Vector3Int(x, y, 0), c);
            }
        }
    }    
}
