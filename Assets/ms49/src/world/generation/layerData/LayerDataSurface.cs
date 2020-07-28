using UnityEngine;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Layer Generation/Surface", order = 1)]
public class LayerDataSurface : LayerDataBase {

    [SerializeField]
    private CellData treeCell = null;
    [SerializeField]
    [Range(0, 1)]
    public float treeChance = 0.5f;

    public override CellData getFillCell() {
        if(Random.Range(0f, 1f) < this.treeChance) {
            return this.treeCell;
        } else {
            return this.tile;
        }
    }
}
