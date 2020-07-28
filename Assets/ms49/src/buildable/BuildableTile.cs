using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Tile", order = 1)]
public class BuildableTile : BuildableBase {

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(1, 1);
    [SerializeField]
    private TileEntry[] tiles = null;
    [SerializeField]
    private CellData singularTile = null;

    // Used by the inspector
    [SerializeField]
    private CellRow[] cells = new CellRow[1];

    public string[,] GetCells() {
        string[,] ret = new string[this.getWidth(), this.getHeight()];

        for(int x = 0; x < this.getWidth(); x++) {
            int y1 = this.getHeight() - 1;
            for(int y = 0; y < this.getHeight(); y++) {
                ret[x, y] = this.cells[x][y1];
                y1--;
            }
        }

        return ret;
    }

    public override int getWidth() {
        return this.gridSize.x;
    }

    public override int getHeight() {
        return this.gridSize.y;
    }

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite) {
        CellData data = this.getTile(0, 0);
        groundSprite = TileSpriteGetter.retrieveSprite(data.groundTile);
        objectSprite = TileSpriteGetter.retrieveSprite(data.objectTile);
    }

    public override bool isRotatable() {
        return this.getTile(0, 0).rotationalOverride;
    }

    public CellData getTile(int x, int y) {
        if(this.getWidth() == 1 && this.getHeight() == 1) {
            return this.singularTile;
        } else {
            if(x < 0 || x >= this.getWidth() || y < 0 || y >= this.getHeight()) {
                return null;
            }

            string s1 = this.GetCells()[x, y];

            // Special case for air
            if(string.IsNullOrWhiteSpace(s1)) {
                return Main.instance.tileRegistry.getAir();
            } else {
                char c = s1[0];

                // Find what tile the char belongs to
                foreach(TileEntry e in this.tiles) {
                    if(e.character == c) {
                        return e.tile;
                    }
                }

                return null;
            }
        }       
    }

    public override void placeIntoWorld(World world, Position pos, Rotation rotation) {
        for(int x = 0; x < this.getWidth(); x++) {
            for(int y = 0; y < this.getHeight(); y++) {
                CellData data = this.getTile(x, y);
                if(data != null) {
                    Position pos1 = pos.add(x, y);
                    world.setCell(pos1, data, rotation);
                    world.liftFog(pos1);
                }
            }
        }
    }

    public override bool isValidLocation(World world, Position pos) {
        for(int x = 0; x < this.getWidth(); x++) {
            for(int y = 0; y < this.getHeight(); y++) {
                Position pos1 = pos.add(x, y);
                if(world.isOutOfBounds(pos1)) {
                    return false;
                }
                if(!world.getCellState(pos1).data.canBuildOver) {
                    return false;
                }
            }
        }

        return true;
    }

    [Serializable]
    public class CellRow {
        [SerializeField]
        private string[] row = new string[1];

        public string this[int i] {
            get {
                return row[i];
            }
        }
    }

    [Serializable]
    public class TileEntry {

        public char character;
        public CellData tile;
    }
}
