public class MapAccessor {

    public CellData[] cells { get; private set; }
    public int[] metas { get; private set; }
    public int depth { get; private set; }
    public int size { get; private set; }

    public MapAccessor(int mapSize, int depth) {
        this.cells = new CellData[mapSize * mapSize];
        this.metas = new int[mapSize * mapSize];
        this.depth = depth;
        this.size = mapSize;
    }

    /// <summary>
    /// Sets a Cell.  Pass null to set air.
    /// </summary>
    public void SetCell(int x, int y, CellData tile, Rotation rotation = null) {
        if(!this.IsInBounds(x, y)) {
            return;
        }
        
        int index = this.GetFlattenedIndice(x, y);

        this.cells[index] = tile == null ? Main.instance.CellRegistry.GetAir() : tile;
        if(rotation != null) {
            this.metas[index] = rotation.id;
        }
    }

    /// <summary>
    /// Sets a Cell.  Pass null to set air.
    /// </summary>
    public void SetCell(int x, int y, CellData tile, int meta) {
        if(!this.IsInBounds(x, y)) {
            return;
        }

        int index = this.GetFlattenedIndice(x, y);

        this.cells[index] = tile == null ? Main.instance.CellRegistry.GetAir() : tile;
        this.metas[index] = meta;
    }

    public CellData GetCell(int x, int y) {
        if(!this.IsInBounds(x, y)) {
            return null;
        }
        return this.cells[this.GetFlattenedIndice(x, y)];
    }

    public void SetRotation(int x, int y, Rotation r) {
        if(!this.IsInBounds(x, y)) {
            return;
        }
        this.metas[this.GetFlattenedIndice(x, y)] = r.id;
    }

    public bool IsInBounds(int x, int y) {
        return x >= 0 && y >= 0 && x < this.size && y < this.size;
    }

    public void ApplyToLayer(Layer layer) {
        for(int x = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                int index = this.GetFlattenedIndice(x, y);
                layer.setCell(
                    x,
                    y,
                    this.cells[index],
                    this.metas[index],
                    false);
            }
        }
    }

    private int GetFlattenedIndice(int x, int y) {
        return this.size * x + y;
    }
}
