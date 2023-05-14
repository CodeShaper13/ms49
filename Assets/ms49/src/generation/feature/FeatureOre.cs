using UnityEngine;

public class FeatureOre : FeatureBase {

    [Space]

    [SerializeField]
    private CellData _cell = null;

    [Space]

    [SerializeField, MinMaxSlider(1, 32)]
    private Vector2Int _veinSize = new Vector2Int(1, 1);
    [SerializeField, Min(1)]
    private int _minSaturation = 50;
    [SerializeField, Min(1)]
    private int _maxSaturation = 100;

    [Space]

    [SerializeField]
    private int _chunkSize = 16;
    [SerializeField]
    private int _veinsPerChunk = 1;

    private void OnValidate() {
        if(this._minSaturation > this._maxSaturation) {
            this._minSaturation = this._maxSaturation;
        }
    }

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        int chunkCount = accessor.size / this._chunkSize;

        for(int chunkPosX = 0; chunkPosX < chunkCount; chunkPosX++) {
            for(int chunkPosY = 0; chunkPosY < chunkCount; chunkPosY++) {

                for(int i = 0; i < this._veinsPerChunk; i++) {
                    this.GenerateVein(rnd, accessor, chunkPosX, chunkPosY);
                }
            }
        }
    }

    private void GenerateVein(System.Random rnd, MapAccessor accessor, int i1, int j1) {
        int chunkX = rnd.Next(0, 16); // Don't start on edge cells.
        int chunkY = rnd.Next(0, 16);

        int size = rnd.Next(this._veinSize.x, this._veinSize.y + 1);

        Rotation lastDir = null;
        for(int i = 0; i < size; i++) {
            int x = (i1 * 16) + chunkX;
            int y = (j1 * 16) + chunkY;

            CellData c = accessor.GetCell(x, y);
            if(c is CellDataMineable) { // Only replace stone and other ores.
                accessor.SetCell(
                    x, y,
                    this._cell,
                    rnd.Next(this._minSaturation, this._maxSaturation + 1));
            }

            Rotation r = Rotation.ALL[rnd.Next(0, Rotation.ALL.Length)];

            if(lastDir != null && lastDir == r) {
                r = r.opposite();
            }

            chunkX += r.vector.x;
            chunkY += r.vector.y;
        }
    }
}