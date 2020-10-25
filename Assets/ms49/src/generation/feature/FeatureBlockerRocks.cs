using UnityEngine;

public class FeatureBlockerRocks : FeatureBase {

    [SerializeField]
    private CellData blockerRockCell = null;
    [SerializeField, MinMaxSlider(0, 100)]
    private Vector2Int rockCount = new Vector2Int(10, 20);
    [SerializeField, MinMaxSlider(1, 10)]
    private Vector2Int rockSize = new Vector2Int(3, 5);
    //[SerializeField, Min(0)]
    //private int oblongScale;

    public override void generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        if(!layerData.generateBlockerRocks) {
            return;
        }

        int rockCount = rnd.Next(this.rockCount.x, this.rockCount.y + 1);
        for(int i = 0; i < rockCount; i++) {
            int x = rnd.Next(1, accessor.size - 1);
            int y = rnd.Next(1, accessor.size - 1);

            // Pick the size.
            int width = rnd.Next(this.rockSize.x, this.rockSize.y + 1);
            int height = rnd.Next(this.rockSize.x, this.rockSize.y + 1);
            //int height = width + rnd.Next(-this.oblongScale, this.oblongScale + 1);
            //if(rnd.Next(0, 2) == 0) {
            //    // Swap width and height
            //    int temp = width;
            //    width = height;
            //    height = temp;
            //}

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
                        
                        && rnd.Next(0, 3) == 0)
                    {
                        continue;
                    }

                    if(accessor.getCell(cellX, cellY) != Main.instance.tileRegistry.getAir()) {
                        accessor.setCell(cellX, cellY, this.blockerRockCell);
                    }
                }
            }
        }
    }
}
