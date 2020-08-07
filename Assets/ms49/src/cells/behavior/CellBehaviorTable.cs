using UnityEngine;
using fNbt;

public class CellBehaviorTable : CellBehaviorOccupiable, IHasData {

    [SerializeField]
    private CellData chairCell = null;
    [SerializeField]
    private SpriteRenderer plateSpriteRenderer = null;
    [SerializeField]
    private Sprite spriteFullPlate = null;
    [SerializeField]
    private Sprite spriteDirtyPlate = null;

    public EnumPlateState plateState { get; set; } = EnumPlateState.NONE;
    public bool hasChair { get { return this.chair != null; }}

    public CellState chair { get; private set; }
    public Position chairPos { get; private set; }

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);

        this.updateChairFlag();
    }

    public override void onUpdate() {
        base.onUpdate();

        this.plateSpriteRenderer.sprite = this.getPlateSprite();
    }

    public override void onNeighborChange(CellState triggererCell, Position triggererPos) {
        base.onNeighborChange(triggererCell, triggererPos);

        this.updateChairFlag();
    }

    public void readFromNbt(NbtCompound tag) {
        this.plateState = (EnumPlateState)tag.getInt("plateState");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("plateState", (int)this.plateState);
    }

    /// <summary>
    /// Returns true if the table's chair is occupied and hte occupant is in the chair.
    /// </summary>
    public bool isOccupantSitting() {
        return this.chair != null && this.isOccupied() && this.getOccupant().position.distance(this.chairPos) <= 0f;
    }

    private void updateChairFlag() {
        foreach(Rotation r in Rotation.ALL) {
            Position p = this.pos + r;
            CellState state = this.world.getCellState(p);
            if(state != null && state.data == this.chairCell && p != this.chairPos) {
                this.chair = state;
                this.chairPos = p;
                return;
            }
        }

        this.chair = null;
        this.setOccupant(null);
    }

    private Sprite getPlateSprite() {
        switch(this.plateState) {
            case EnumPlateState.NONE:
                return null;
            case EnumPlateState.DIRTY:
                return this.spriteDirtyPlate;
            case EnumPlateState.FULL:
                return this.spriteFullPlate;
        }
        return null;
    }

    public enum EnumPlateState {
        NONE = 0,
        DIRTY = 1,
        FULL = 2,
        CLEAN = 3,
    }
}
