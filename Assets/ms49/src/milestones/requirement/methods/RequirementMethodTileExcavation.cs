using UnityEngine;

[CreateAssetMenu(
    fileName = "Requirement",
    menuName = "MS49/Milestone/Requirement/Tile Excavation",
    order = 1)]
public class RequirementMethodTileExcavation : RequirementMethodBase {

    //[SerializeField]
    //private CellData cell = null;

    public override int getProgress(World world) {
        return 0; //world.stoneExcavated;
    }
}
