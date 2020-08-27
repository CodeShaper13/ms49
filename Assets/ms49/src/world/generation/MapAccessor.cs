public class MapAccessor {

    public CellData[] cells {
        get; private set;
    }

    public int size {
        get; private set;
    }

    public MapAccessor(int mapSize) {
        this.cells = new CellData[mapSize * mapSize];
        this.size = mapSize;
    }

    /// <summary>
    /// Sets a Cell.  Pass null to set air.
    /// </summary>
    public void setCell(int x, int y, CellData tile) {
        if(!this.inBounds(x, y)) {
            return;
        }
        this.cells[this.getFlattenedIndice(x, y)] = tile == null ? Main.instance.tileRegistry.getAir() : tile;
    }

    public CellData getCell(int x, int y) {
        if(!this.inBounds(x, y)) {
            return null;
        }
        return this.cells[this.getFlattenedIndice(x, y)];
    }

    public bool inBounds(int x, int y) {
        return x >= 0 && y >= 0 && x < size && y < size;
    }

    private int getFlattenedIndice(int x, int y) {
        return this.size * x + y;
    }
}
