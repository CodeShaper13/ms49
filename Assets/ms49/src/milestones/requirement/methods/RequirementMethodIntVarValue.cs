using UnityEngine;

[CreateAssetMenu(
    fileName = "Requirement",
    menuName = "MS49/Milestone Requirement/Int Amout",
    order = 1)]
public class RequirementMethodIntVarValue : RequirementMethodBase {

    [SerializeField]
    private IntVariable intVar = null;

    public override int getProgress(World world) {
        return this.intVar != null ? this.intVar.value : 0;
    }
}
