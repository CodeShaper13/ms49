using UnityEngine;

public class FeatureRareSkull : FeatureBase {

    [SerializeField]
    private CellData _skull = null;

    public override void generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        if(this._skull == null) {
            Debug.LogWarning("Skull not set for world generation feature FeatureRareSkull");
        } else {
            // Try up to 1000 times to place the skull.
            for(int i = 0; i < 1000; i++) {
                int x = Random.Range(0, accessor.size);
                int y = Random.Range(32, accessor.size); // Don't let it be near the bottom and in a starting plot.
                if(accessor.getCell(x, y) is CellDataMineable) {
                    accessor.setCell(x, y, this._skull);
                    break;
                }
            }
        }
    }
}
