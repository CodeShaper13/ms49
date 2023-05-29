using UnityEngine;
using System;

public class StructureStartingRoom : StructureBase {

    [SerializeField]
    private int _structureY = 0;
    [SerializeField]
    private Prebuilt[] _areas = null;

    public override void GenerateStructure(World world, int depth) {
        int structureX = world.MapSize / 2;

        foreach(Prebuilt prebuilt in this._areas) {
            Position pos = new Position(
                structureX + prebuilt.position.x,
                this._structureY + prebuilt.position.y,
                depth);
            prebuilt.area.placeIntoWorld(world, pos);
        }
    }

    [Serializable]
    private class Prebuilt {

        public PrebuiltArea area = null;
        public Vector2Int position = Vector2Int.zero;
    }
}
