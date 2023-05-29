using NaughtyAttributes;
using UnityEngine;

public class FeatureLargeTrack : FeatureBase {

    [SerializeField, Required]
    private CellData _largeRail = null;
    [SerializeField, Min(0)]
    private int _distanceFromBottom = 10;

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        if(this._largeRail != null) {
            for(int x = 0; x < accessor.size; x++) {
                accessor.SetCell(x, this._distanceFromBottom + 1, this._largeRail, Rotation.UP);
                accessor.SetCell(x, this._distanceFromBottom, this._largeRail, Rotation.DOWN);
            }
        }
    }
}