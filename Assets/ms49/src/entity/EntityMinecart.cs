using UnityEngine;
using System;
using fNbt;

[RequireComponent(typeof(SpriteRenderer))]
public class EntityMinecart : EntityBase {

    [SerializeField]
    private MinecartSprites sprites = null;
    [SerializeField, Min(1)]
    private int maxCapacity = 4;
    [SerializeField, Min(0), Tooltip("How fast the mine cart moves in meters per second.")]
    private float movementSpeed = 1f;

    private SpriteRenderer sr;
    public Inventory inventory { get; private set; }
    public Rotation facing { get; set; }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.sr = this.GetComponent<SpriteRenderer>();
        this.inventory = new Inventory(this.maxCapacity);
        this.facing = Rotation.RIGHT;
    }

    public override void onUpdate() {
        base.onUpdate();

        // Movement physics:
        CellState state = this.world.getCellState(this.position);
        if(state.data is CellDataRail) {
            // Move on the track
            CellDataRail railData = (CellDataRail)state.data;

            switch(railData.moveType) {
                case CellDataRail.EnumRailMoveType.STRAIGHT:
                    this.move(this.facing.vector);
                    break;
                case CellDataRail.EnumRailMoveType.CROSSING:
                    this.move(this.facing.vector);
                    break;
                case CellDataRail.EnumRailMoveType.CURVE:
                    // Get the distance from the middle
                    Position cellPos = this.position;
                    Vector2 cellCenter = cellPos.vec2 + new Vector2(0.5f, 0.5f);

                    Rotation startRot = state.rotation;
                    Rotation endRot = state.rotation.clockwise();
                    float disToStart = Vector2.Distance(this.worldPos, cellCenter + state.rotation.vectorF * 0.5f);
                    float disToEnd = Vector2.Distance(this.worldPos, cellCenter + state.rotation.clockwise().vectorF * 0.5f);

                    if(this.facing.axis != state.rotation.axis) {
                        // Minecart is moving backwards along curve, switch the values.
                        float temp = disToStart;
                        Rotation temp1 = startRot;
                        disToStart = disToEnd;
                        startRot = endRot;
                        disToEnd = temp;
                        endRot = temp1;
                    }

                    // Move the cart
                    this.move(this.facing.vector);

                    float dis = Vector2.Distance(this.worldPos, cellCenter + startRot.vectorF * 0.5f);
                    if(dis > 0.5f) {
                        this.facing = endRot;
                        this.worldPos = cellCenter;
                        this.move(this.facing.vectorF * (dis - 0.5f));
                    }
                    break;
            }
        }
        else {
            // Not on a rail, explode.
            this.explode();
        }
    }

    private void move(Vector2 dir) {
        this.transform.position += (Vector3)(dir * this.movementSpeed * Time.deltaTime);
    }

    private void LateUpdate() {
        if(!Pause.isPaused()) {

            // Update sprite renderer
            this.sr.sprite = this.sprites.getSprite(this.facing, this.inventory.isEmpty());
            if(this.facing.vector.x != 0) { // Side to side
                this.sr.flipX = this.facing == Rotation.LEFT;
            }
        }
    }

    public void explode() {
        print("Minecart Explode!");
        this.world.removeEntity(this);
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("inventory", this.inventory.writeToNbt());
        tag.setTag("rotation", this.facing.id);
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.inventory.readFromNbt(tag.getCompound("inventory"));
        this.facing = Rotation.ALL[Mathf.Clamp(tag.getInt("rotation"), 0, 3)]; // Clamp for safety
    }

    [Serializable]
    public class MinecartSprites {

        public Sprite sideEmpty;
        public Sprite sideFull;
        public Sprite frontEmpty;
        public Sprite frontFull;

        public Sprite getSprite(Rotation rot, bool empty) {
            if(rot.vector.x == 0) { // Facing up or down
                return empty ? this.frontEmpty : this.frontFull;
            } else {
                return empty ? this.sideEmpty : this.sideFull;
            }
        }
    }
}
