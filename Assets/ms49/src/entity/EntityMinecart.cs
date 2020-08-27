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
    [SerializeField]
    private SpriteRenderer fillRenderer = null;
    [SerializeField]
    private GameObject explodeParticlePrefab = null;

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

            if(state.behavior is CellBehaviorDetectorRail) {
                ((CellBehaviorDetectorRail)state.behavior).setTripped();
            }

            Vector2 cellCenter = this.position.vec2 + new Vector2(0.5f, 0.5f);

            switch(railData.moveType) {
                case CellDataRail.EnumRailMoveType.STRAIGHT:
                    this.move(this.facing.vector);
                    break;
                case CellDataRail.EnumRailMoveType.CROSSING:
                    this.move(this.facing.vector);
                    break;
                case CellDataRail.EnumRailMoveType.CURVE:
                    Rotation startRot = state.rotation;
                    Rotation endRot = state.rotation.clockwise();

                    // If the Minecart is moving "backwards" along curve, switch the values.
                    if(this.facing.axis != state.rotation.axis) {
                        Rotation temp1 = startRot;
                        startRot = endRot;
                        endRot = temp1;
                    }

                    // Move the cart.
                    this.move(this.facing.vector);

                    float dis = Vector2.Distance(this.worldPos, cellCenter + startRot.vectorF * 0.5f);
                    if(dis > 0.5f) {
                        this.facing = endRot;
                        this.worldPos = cellCenter;
                        this.move(this.facing.vectorF * (dis - 0.5f));
                    }
                    break;
                case CellDataRail.EnumRailMoveType.STOPPER:
                    this.move(this.facing.vector);

                    float d = Vector2.Distance(this.worldPos, cellCenter + state.rotation.opposite().vectorF * 0.5f);
                    if(this.facing == state.rotation && d > 0.25f) {
                        this.facing = this.facing.opposite();
                        this.move(this.facing.vectorF * (d - 0.5f));
                    }

                    // TODO does the cart ever overshoot?

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

            // Set main sprite
            this.sr.sprite = this.facing.axis == EnumAxis.Y ? this.sprites.frontEmpty : this.sprites.sideEmpty;
            this.sr.flipX = this.facing == Rotation.LEFT;

            // Set fill sprite
            if(this.inventory.isEmpty()) {
                this.fillRenderer.sprite = null;
            } else {
                this.fillRenderer.sprite = this.facing.axis == EnumAxis.Y ? this.sprites.frontFull : this.sprites.sideFull;
                this.fillRenderer.flipX = this.facing == Rotation.LEFT;
                this.fillRenderer.color = this.world.mapGenData.getLayerFromDepth(this.depth).getGroundTint(this.position.x, this.position.y);
            }
        }
    }

    /// <summary>
    /// Causes the Minecart to explode, removing it from the world and playing a particle effect.
    /// </summary>
    public void explode() {
        this.world.particles.spawn(this.position, this.explodeParticlePrefab);
        this.world.entities.remove(this);
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
            if(rot.axis == EnumAxis.Y) {
                return empty ? this.frontEmpty : this.frontFull;
            } else {
                return empty ? this.sideEmpty : this.sideFull;
            }
        }
    }
}
