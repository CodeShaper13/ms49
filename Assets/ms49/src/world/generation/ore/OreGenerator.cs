using System.Collections.Generic;
using UnityEngine;

public class OreGenerator {

    private MapGenerationData.TileReferences tiles;
    private List<Vector2Int> dirs;

    public OreGenerator(MapGenerationData.TileReferences tiles) {
        this.tiles = tiles;
        this.dirs = new List<Vector2Int>();
        this.dirs.Add(Vector2Int.up);
        this.dirs.Add(Vector2Int.down);
        this.dirs.Add(Vector2Int.left);
        this.dirs.Add(Vector2Int.right);
    }

    public void generateOres(System.Random rnd, LayerDataBase layerData, MapAccessor accessor) {
        foreach(OreSettings setting in layerData.oreSpawnSettings) {
            if(setting != null && setting.cell != null) {
                for(int i = 0; i < setting.veinCount; i++) {
                    this.makeVein(rnd, setting.cell, setting, accessor);
                }
            }
        }
    }

    private void makeVein(System.Random rnd, CellData cell, OreSettings setting, MapAccessor accessor) {
        int x = rnd.Next(1, accessor.size); // Don't start on edge cells.
        int y = rnd.Next(1, accessor.size);

        int size = rnd.Next(setting.veinSize.x, setting.veinSize.y + 1);

        for(int i = 0; i < size; i++) {
            if(accessor.getCell(x, y) != Main.instance.tileRegistry.getAir()) {
                accessor.setCell(x, y, cell);
            }

            Vector2Int v = this.dirs[rnd.Next(0, this.dirs.Count)];
            x += v.x;
            y += v.y;
        }
    }
}
