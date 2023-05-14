using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Creates edge walls around the map.
/// </summary>
public class FeatureEdgeWalls : FeatureBase {

    [SerializeField]
    private CellData _cell = null;
    [SerializeField, Min(1)]
    private int _wallThickness = 3;
    [SerializeField]
    private AnimationCurve _cellPlaceChance = null;

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        this.MakeWall(0, 0, Rotation.UP, accessor);
        this.MakeWall(0, accessor.size - 1, Rotation.RIGHT, accessor);
        this.MakeWall(accessor.size - 1, accessor.size - 1, Rotation.DOWN, accessor);
    }

    private void MakeWall(int startX, int startY, Rotation wallDirection, MapAccessor accessor) {
        Position startPos = new Position(startX, startY, 0);

        for(int i = 0; i < this._wallThickness; i++) {
            float odds = this.GetOdds(i);

            Position pos = startPos;

            for(int j = 0; j < accessor.size; j++) {
                pos += wallDirection;

                if(Random.Range(0f, 1f) <= odds) {
                    accessor.SetCell(pos.x, pos.y, this._cell);
                }
            }

            startPos += wallDirection.clockwise();
        }
    }

    private float GetOdds(int i) {
        return this._cellPlaceChance.Evaluate(1f - (i / (this._wallThickness - 1f)));
    }

    [Button]
#pragma warning disable IDE0051 // Remove unused private members
    private void PrintOdds() {
#pragma warning restore IDE0051 // Remove unused private members
        Debug.LogFormat("Odds:");
        for(int i = 0; i < this._wallThickness; i++) {
            Debug.LogFormat("[{0}]: {1}", i, this.GetOdds(i));
        }
    }
}
