using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Cycle Tile", order = 1)]
public class BuildableCycleTile : BuildableTile {

    [SerializeField]
    private CellData[] cells;
    [SerializeField]
    private string rotationMsg = "";

    void OnValidate() {
        int resizeSize = -1;

        if(this.cells.Length == 3) {
            resizeSize = 2;
        }
        if(this.cells.Length > 4) {
            resizeSize = 4;
        }

        if(resizeSize != -1) {
            CellData[] array = new CellData[resizeSize];
            Array.Copy(this.cells, array, resizeSize);
            this.cells = array;
        }
    }

    public override bool isRotatable() {
        return this.cells.Length > 1;
    }

    public override string getRotationMsg() {
        return this.cells.Length > 1 ? this.rotationMsg : string.Empty;
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        return base.isValidLocation(world, pos, rotation);
    }
}
