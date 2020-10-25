using UnityEngine;
using System;
using fNbt;

public class EntityMinecart : EntityBase {

    [SerializeField]
    private MinecartSprites sprites = null;
    [SerializeField, Min(0), Tooltip("How fast the mine cart moves in meters per second.")]
    private float movementSpeed = 1f;
    [SerializeField]
    private SpriteRenderer _cartRenderer = null;
    [SerializeField]
    private SpriteRenderer _fillRenderer = null;
    [SerializeField]
    private GameObject explodeParticlePrefab = null;
    [SerializeField]
    private Inventory _inventory = null;

    private IMinecartInteractor cartInteractor;

    public Inventory inventory => this._inventory;

    public override void onUpdate() {
        base.onUpdate();

        if(this.cartInteractor != null) {
            Vector2 target = this.cartInteractor.getCartStopPoint();

            if(this.worldPos == target) {
                // In front of interactor.
                this.cartInteractor.minecart = this;
            } else {
                // Not in front of the interactor, move towards it.
                this.worldPos = (Vector3)Vector2.MoveTowards(this.worldPos, target, this.movementSpeed * Time.deltaTime);
            }
        } else {
            // Move the Minecart:
            CellState state = this.world.getCellState(this.position);
            if(state.data is CellDataRail) {

                // Trip detector rail if above.
                if(state.behavior is CellBehaviorDetectorRail) {
                    ((CellBehaviorDetectorRail)state.behavior).setTripped(this);
                }

                // Check if there is an instance of IMinecartInteractor below or next to the rails.
                if(!this.checkForInteractor(null)) {
                    foreach(Rotation r in Rotation.ALL) {
                        bool foundInteractor = this.checkForInteractor(r);
                        if(foundInteractor) {
                            break;
                        }
                    }
                }  

                Vector2 cellCenter = this.position.vec2 + new Vector2(0.5f, 0.5f);

                // Save the position at the start of the frame
                Vector3 startingPos = this.worldPos;

                switch(((CellDataRail)state.data).moveType) {
                    case CellDataRail.EnumRailMoveType.STRAIGHT:
                        this.move(this.rotation.vector);
                        break;
                    case CellDataRail.EnumRailMoveType.CROSSING:
                        this.move(this.rotation.vector);
                        break;
                    case CellDataRail.EnumRailMoveType.CURVE:
                        Rotation startRot = state.rotation;
                        Rotation endRot = state.rotation.clockwise();

                        // If the Minecart is moving "backwards" along curve, switch the values.
                        if(this.rotation.axis != state.rotation.axis) {
                            Rotation temp1 = startRot;
                            startRot = endRot;
                            endRot = temp1;
                        }

                        // Move the cart.
                        this.move(this.rotation.vector);

                        float dis = Vector2.Distance(this.worldPos, cellCenter + startRot.vectorF * 0.5f);
                        if(dis > 0.5f) {
                            this.rotation = endRot;
                            this.worldPos = cellCenter;
                            this.move(this.rotation.vectorF * (dis - 0.5f));
                        }
                        break;
                    case CellDataRail.EnumRailMoveType.STOPPER:
                        this.move(this.rotation.vector);

                        float d = Vector2.Distance(this.worldPos, cellCenter + state.rotation.opposite().vectorF * 0.5f);
                        if(this.rotation == state.rotation && d > 0.25f) {
                            this.rotation = this.rotation.opposite();
                            this.move(this.rotation.vectorF * (d - 0.5f));
                        }

                        // TODO does the cart ever overshoot?

                        break;
                }

                // Check if the Minecart has hit another one.
                ContactFilter2D filter = new ContactFilter2D();
                filter.layerMask = LayerMask.GetMask("MinecartPhysics");
                int overlappingCarts = this.GetComponent<BoxCollider2D>().OverlapCollider(filter, new Collider2D[1]);
                if(overlappingCarts > 0) {
                    // It has moved and is now touching another cart, put it back to where it started.
                    this.worldPos = startingPos;
                }
            }
            else {
                // Not on a rail, explode.
                this.explode();
            }
        }
    }

    private void LateUpdate() {
        if(!Pause.isPaused()) {

            // Set main sprite
            this._cartRenderer.sprite = this.rotation.axis == EnumAxis.Y ? this.sprites.frontEmpty : this.sprites.sideEmpty;
            this._cartRenderer.flipX = this.rotation == Rotation.LEFT;

            // Set fill sprite
            if(this.inventory.isEmpty()) {
                this._fillRenderer.sprite = null;
            } else {
                this._fillRenderer.sprite = this.rotation.axis == EnumAxis.Y ? this.sprites.frontFull : this.sprites.sideFull;
                this._fillRenderer.flipX = this.rotation == Rotation.LEFT;
            }
        }
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("inventory", this.inventory.writeToNbt());
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.inventory.readFromNbt(tag.getCompound("inventory"));
    }

    public override bool isDestroyable() {
        return true;
    }

    /// <summary>
    /// Causes the Minecart to explode, removing it from the world
    /// and playing a particle effect.
    /// </summary>
    public void explode() {
        this.world.particles.spawn(this.position, this.explodeParticlePrefab);
        this.world.entities.remove(this);
    }

    public void release() {
        this.cartInteractor = null;
    }

    /// <summary>
    /// Checks if there is a CellBehavior implementing
    /// IMinecartInteracter in the direction that is passed.  If
    /// there is one, true is returned.
    /// 
    /// If null is passed for rotation, the cell the cart is on is
    /// checked.
    /// <returns></returns>
    private bool checkForInteractor(Rotation rotation) {
        CellState adjacentState = this.world.getCellState(rotation == null ? this.position : this.position + rotation);
        if(adjacentState != null && adjacentState.behavior is IMinecartInteractor) {
            IMinecartInteractor cartInteracter = (IMinecartInteractor)adjacentState.behavior;

            if(cartInteracter.shouldCartInteract(this)) {
                this.cartInteractor = cartInteracter;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Moves the minecart in the passed direction.  Move speed and delta time is factored in here.
    /// </summary>
    private void move(Vector2 dir) {
        this.transform.position += (Vector3)(dir * this.movementSpeed * Time.deltaTime);
    }

    public override void onRightClick() {
        PopupContainer popup = Main.instance.findPopup<PopupContainer>();
        if(popup != null) {
            popup.open();
            popup.setInventory(this.inventory);
        }
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
