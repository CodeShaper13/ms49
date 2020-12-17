using UnityEngine;

public class FeatureBedrockWalls : FeatureBase {

    [SerializeField]
    private CellData _bedrock = null;

    public override void generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        int end = accessor.size - 1;

        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                if(x == 0 || y == 0 || x == end || y == end) {
                    accessor.setCell(x, y, this._bedrock);
                }
            }
        }
    }
}
