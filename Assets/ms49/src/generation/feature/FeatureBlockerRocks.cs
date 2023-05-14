using UnityEngine;

public class FeatureBlockerRocks : FeatureBase {

    [SerializeField]
    private CellData _cell = null;
    [SerializeField, MinMaxSlider(0, 100)]
    private Vector2Int _rockCount = new Vector2Int(10, 20);
    [SerializeField, MinMaxSlider(1, 10)]
    private Vector2Int _rockSize = new Vector2Int(3, 5);
    [SerializeField, Min(0)]
    private int _cornerSkipRate = 3;

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        int rockCount = rnd.Next(this._rockCount.x, this._rockCount.y + 1);
        for(int i = 0; i < rockCount; i++) {
            int x = rnd.Next(1, accessor.size - 1);
            int y = rnd.Next(1, accessor.size - 1);

            // Pick the size.
            int width = rnd.Next(this._rockSize.x, this._rockSize.y + 1);
            int height = rnd.Next(this._rockSize.x, this._rockSize.y + 1);

            // Place Cells.
            for(int x1 = 0; x1 < width; x1++) {
                for(int y1 = 0; y1 < height; y1++) {
                    int cellX = x + x1;
                    int cellY = y + y1;

                    // Randomly skip corners
                    if((
                        (x1 == 0 && y1 == 0) ||
                        (x1 == 0 && y1 == height - 1) ||
                        (x1 == width - 1 && y1 == height - 1) ||
                        (x1 == width - 1 && y1 == 0))
                        
                        && rnd.Next(0, this._cornerSkipRate) == 0)
                    {
                        continue;
                    }

                    if(accessor.GetCell(cellX, cellY) != Main.instance.CellRegistry.GetAir()) {
                        accessor.SetCell(cellX, cellY, this._cell);
                    }
                }
            }
        }
    }
}
