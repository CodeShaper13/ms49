using UnityEngine;
using System;
using System.Collections.Generic;
using fNbt;

[RequireComponent(typeof(SpriteRenderer))]
public class EntityMinecart : EntityBase {

    [SerializeField]
    private MinecartSprites sprites = null;
    [SerializeField, Min(1)]
    private int maxCapacity = 4;
    [SerializeField, Min(0.0001f), Tooltip("How fast the mine cart moves in meters per second.")]
    private float movementSpeed = 1f;

    private SpriteRenderer sr;
    public Inventory inventory { get; private set; }
    private Rotation facing;

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.sr = this.GetComponent<SpriteRenderer>();
        this.inventory = new Inventory(this.maxCapacity);
        this.facing = Rotation.RIGHT;
    }

    public override void onUpdate() {
        base.onUpdate();

        // Movement physics
        CellState cell = this.world.getCellState(this.position);
        if(cell.data is CellDataRail) {
            this.transform.position += (Vector3)(this.facing.vectorF * this.movementSpeed * Time.deltaTime);
        } else {
            // Not on a rail, explode.
            this.explode();
        }
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
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.inventory.readFromNbt(tag.getCompound("inventory"));
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
