using UnityEngine;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Layer Generation/Surface", order = 1)]
public class LayerDataSurface : LayerDataBase {

    [SerializeField]
    private PrimitiveRndObject[] rndObjects = null;

    public override CellData getFillCell() {
        CellData cell = this.tile;
        foreach(PrimitiveRndObject pro in this.rndObjects) {
            pro.getRnd(ref cell);
        }

        return cell;
    }
}
