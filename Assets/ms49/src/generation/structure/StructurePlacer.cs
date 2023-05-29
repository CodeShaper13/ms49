using UnityEngine;

public class StructurePlacer : StructureBase {

    [SerializeField]
    private StructureData _structure = null;

    public override void GenerateStructure(World world, int depth) {
        if(this._structure != null) {
            this._structure.Generate(world, depth);
        }
    }
}