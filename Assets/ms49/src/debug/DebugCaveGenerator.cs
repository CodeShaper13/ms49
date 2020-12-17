using UnityEngine;

[ExecuteInEditMode]
public class DebugCaveGenerator : MonoBehaviour {

    [SerializeField]
    private FeatureCave feature = null;
    [SerializeField]
    private int mapSize = 96;
    [SerializeField]
    private string seed = "";
    [SerializeField]
    private bool _generateLakes = true;

    private int[,] map;

    private void Update() {
        this.map = this.feature.makeCaves(
            new System.Random(seed.GetHashCode() * (0 + 1)),
            this._generateLakes ? EnumLakeType.WATER : EnumLakeType.NONE,
            this.mapSize);
    }

    private void OnDrawGizmos() {
        if(this.map != null) {
            for(int x = 0; x < this.mapSize; x++) {
                for(int y = 0; y < this.mapSize; y++) {
                    int tile = map[x, y];
                    Color c;
                    if(tile == 1) { // Wall
                        c = Color.black;
                    }
                    else if(tile == 0) { // Air
                        c = Color.white;
                    }
                    else if(tile == 2) { // Water
                        c = Color.blue;
                    }
                    else {
                        c = Color.red;
                    }

                    Gizmos.color = c;
                    Vector3 pos = new Vector3(-this.mapSize / 2 + x + 0.5f, 0, -this.mapSize / 2 + y + 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
