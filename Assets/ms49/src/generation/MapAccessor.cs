public class MapAccessor {

    public readonly CellData[] cells;
    public readonly int[] metas;
    public readonly int size;

    public MapAccessor(int mapSize) {
        this.size = mapSize;
        this.cells = new CellData[mapSize * mapSize];
        this.metas = new int[mapSize * mapSize];
    }

    /// <summary>
    /// Sets a Cell.
    /// </summary>
    public void SetCell(int x, int y, CellData tile, Rotation rotation) {
        this.SetCell(x, y, tile, rotation == null ? 0 : rotation.id);
    }

    /// <summary>
    /// Sets a Cell.
    /// </summary>
    public void SetCell(int x, int y, CellData tile, int meta = 0) {
        if(this.OutOfBounds(x, y)) {
            return;
        }

        int index = this.size * x + y;
        this.cells[index] = tile;
        this.metas[index] = meta;
    }

    /// <summary>
    /// Returns the Cell at the passed position.  If the position is
    /// out of bounds, null, is returned.
    /// </summary>
    public CellData GetCell(int x, int y) {
        if(this.OutOfBounds(x, y)) {
            return null;
        }
        return this.cells[this.size * x + y];
    }

    public void SetRotation(int x, int y, Rotation r) {
        if(this.OutOfBounds(x, y)) {
            return;
        }
        this.metas[this.size * x + y] = r.id;
    }

    public bool OutOfBounds(int x, int y) {
        return x < 0 || y < 0 || x >= this.size || y >= this.size;
    }

    public void ApplyToLayer(Layer layer) {
        for(int x = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                int index = this.size * x + y;
                layer.SetCell(
                    x,
                    y,
                    this.cells[index],
                    this.metas[index],
                    false);
            }
        }
    }
}
