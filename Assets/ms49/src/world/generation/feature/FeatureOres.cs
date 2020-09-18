using UnityEngine;

public class FeatureOres : FeatureBase {

    private Vector2Int[] dirs = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    public override void generate(System.Random rnd, LayerDataBase layerData, MapAccessor accessor) {
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
            CellData c = accessor.getCell(x, y);
            if(c is CellDataMineable) {
                accessor.setCell(x, y, cell);
            }

            Vector2Int v = this.dirs[rnd.Next(0, this.dirs.Length)];
            x += v.x;
            y += v.y;
        }
    }
}
