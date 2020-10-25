using UnityEngine;


/// <summary>
/// Executes if the owner has a dirt plate in hand.
/// 
/// This will make them travel to a sink and wash the plate.
/// </summary>
public class TaskWashPlate : TaskMovement<EntityWorker> {

    [SerializeField]
    private float plateWashSpeed = 4f;
    [SerializeField]
    private CookMetaData cookData = null;
    [SerializeField]
    private Sprite _emoteSinkSprite = null;

    private float washTimer;
    private CellBehaviorSink sink;

    public override bool shouldExecute() {
        if(this.cookData.plateState == CellBehaviorTable.EnumPlateState.DIRTY) {
            this.sink = this.calculateAndSetPathToClosest<CellBehaviorSink>(
                false,
                behavior => !behavior.isOccupied(),
                Rotation.UP,
                Position.down);

            if(this.navPath != null) {
                this.sink.setOccupant(this.owner);
                return true;
            } else {
                // Can't find sink, but one is needed.
                this.owner.emote.startEmote(new Emote(this._emoteSinkSprite, 0.1f).setTooltip("Needs a sink to wash dishes"));
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.sink == null || this.cookData.plateState != CellBehaviorTable.EnumPlateState.DIRTY) {
            return false;
        } else {
            return true;
        }
    }

    public override void onAtDestination() {
        this.sink.setFilled(true);

        this.washTimer += Time.deltaTime;
        if(this.washTimer >= this.plateWashSpeed) {
            this.cookData.plateState = CellBehaviorTable.EnumPlateState.CLEAN;
            this.sink.setOccupant(null);
            this.sink.setFilled(false);
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.washTimer = 0f;
        if(this.sink != null) {
            this.sink.setOccupant(null);
            this.sink.setFilled(false);
        }

        this.sink = null;
    }
}
