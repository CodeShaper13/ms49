using UnityEngine;

public class FeatureOreRndWalk : FeatureBase {

    [SerializeField]
    private CellData _cell = null;
    [SerializeField, Range(0, 1)]
    private float _veinsPerSquare = 0.005f;
    [SerializeField, MinMaxSlider(1, 100)]
    private Vector2Int _veinSize = new Vector2Int(5, 8);

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        float area = accessor.size * accessor.size;
        int rockCount = Mathf.RoundToInt(area * this._veinsPerSquare);
        
        for(int i = 0; i < rockCount; i++) {
            Vector2Int pos = new Vector2Int(
                rnd.Next(1, accessor.size - 1),
                rnd.Next(1, accessor.size - 1));
            int rockSize = rnd.Next(this._veinSize.x, this._veinSize.y + 1);

            Rotation lastRotation = Rotation.ALL[rnd.Next(0, 3)];
            for(int j = 0; j < rockSize; j++) {
                accessor.SetCell(pos.x, pos.y, this._cell);

                lastRotation = rnd.Next(0, 2) == 0 ? lastRotation.clockwise() : lastRotation.counterClockwise();

                pos += lastRotation.vector;
            }
        }
    }
}