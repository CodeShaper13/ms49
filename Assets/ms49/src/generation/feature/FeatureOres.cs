public class FeatureOres : FeatureBase {

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        int chunkCount = accessor.size / 16;

        for(int chunkPosX = 0; chunkPosX < chunkCount; chunkPosX++) {
            for(int chunkPosY = 0; chunkPosY < chunkCount; chunkPosY++) {

                foreach(OreSettings setting in layerData.oreSpawnSettings) {
                    if(setting != null && setting.cell != null) {
                        for(int i = 0; i < setting.veinsPerChunk; i++) {
                            this.makeVein(rnd, setting.cell, setting, accessor, chunkPosX, chunkPosY);
                        }
                    }
                }
            }
        }
    }

    private void makeVein(System.Random rnd, CellData oreCell, OreSettings setting, MapAccessor accessor, int i1, int j1) {
        int chunkX = rnd.Next(0, 16); // Don't start on edge cells.
        int chunkY = rnd.Next(0, 16);

        int size = rnd.Next(setting.size.x, setting.size.y + 1);

        Rotation lastDir = null;
        for(int i = 0; i < size; i++) {
            int x = (i1 * 16) + chunkX;
            int y = (j1 * 16) + chunkY;

            CellData c = accessor.GetCell(x, y);
            if(c is CellDataMineable) {
                accessor.SetCell(x, y, oreCell);
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
