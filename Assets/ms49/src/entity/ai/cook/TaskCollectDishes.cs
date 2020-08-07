/// <summary>
/// Executes if the owner has no plate in hand and there is
/// a table with a dirt plate.
/// 
/// This will make the owner travel to a table with a dirty
/// plate and pick it up.
/// </summary>
public class TaskCollectDishes : TaskBase<EntityCook> {

    private CellBehaviorTable table;

    public TaskCollectDishes(EntityCook owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        if(this.table == null || this.table.plateState != CellBehaviorTable.EnumPlateState.DIRTY) {
            return false;
        } else {
            return true;
        }
    }

    public override void preform() {
        if(this.owner.position.distance(this.table.pos) <= 1) {
            this.owner.plateState = CellBehaviorTable.EnumPlateState.DIRTY;
            this.table.plateState = CellBehaviorTable.EnumPlateState.NONE;
        }
    }

    public override bool shouldExecute() {
        if(this.owner.plateState == CellBehaviorTable.EnumPlateState.NONE) {
            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
                if(table.plateState == CellBehaviorTable.EnumPlateState.DIRTY || (table.plateState == CellBehaviorTable.EnumPlateState.FULL && !table.isOccupied())) {
                    if(this.moveHelper.setDestination(table.pos, true) != null) {
                        this.table = table;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
