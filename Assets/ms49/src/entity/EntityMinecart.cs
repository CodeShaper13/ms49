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
    public MinecartSprites minecartSprites => this.sprites;

    public override void Update() {
        if(Pause.IsPaused) {
            return;
        }

        base.Update();

        // Explode the cart if it has left the map.
        if(this.world.IsOutOfBounds(this.position)) {
            this.explode();
            return;
        }

        if(this.cartInteractor != null) {
            Vector2 target = this.cartInteractor.GetCartStopPoint();

            if(this.worldPos == target) {
                // In front of interactor.
                this.cartInteractor.minecart = this;
            } else {
                // Not in front of the interactor, move towards it.
                this.worldPos = (Vector3)Vector2.MoveTowards(this.worldPos, target, this.movementSpeed * Time.deltaTime);
            }
        } else {
            // Move the Minecart:

            CellState state = this.world.GetCellState(this.position);
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

                Vector2 cellCenter = this.position.Center;

                // the reason the explode isn't working is because the cart changes direction while on the cell

                switch(((CellDataRail)state.data).MoveType) {
                    case CellDataRail.EnumRailMoveType.STRAIGHT:
                        this.move(this.rotation.vector);                       
                        break;

                    case CellDataRail.EnumRailMoveType.CROSSING:
                        this.move(this.rotation.vector);
                        break;

                    case CellDataRail.EnumRailMoveType.CURVE:
                        this.handleCurveRail(state.Rotation, state.Rotation.clockwise(), cellCenter);

                        break;

                    case CellDataRail.EnumRailMoveType.STOPPER:
                        this.move(this.rotation.vector);

                        float d = Vector2.Distance(this.worldPos, cellCenter + state.Rotation.opposite().vectorF * 0.5f);
                        if(this.rotation == state.Rotation && d > 0.25f) {
                            this.rotation = this.rotation.opposite();
                            this.move(this.rotation.vectorF * (d - 0.5f));
                        }

                        break;

                    case CellDataRail.EnumRailMoveType.MERGER:
                        // TODO explode if coming in from the wrong angle (state.rotation.opposite())

                        Rotation endRot = this.rotation.axis != state.Rotation.axis ? this.rotation : state.Rotation.clockwise();
                        this.handleCurveRail(state.Rotation, endRot, cellCenter);

                        break;
                }
            }
            else {
                // Not on a rail, explode.
                this.explode();
            }
        }
    }


    private void handleCurveRail(Rotation startRot, Rotation endRot, Vector2 cellCenter) {
        // If the Minecart is moving "backwards" along curve, switch the values.
        if(this.rotation.axis != startRot.axis) {
            Rotation temp1 = startRot;
            startRot = endRot;
            endRot = temp1;
        }

        // Move the cart.
        this.move(this.rotation.vector);

        float distFromCurveStart = Vector2.Distance(this.worldPos, cellCenter + (startRot.vectorF * 0.5f));
        if(distFromCurveStart > 0.5f) {
            this.rotation = endRot;
            this.worldPos = cellCenter;
            this.move(this.rotation.vectorF * (distFromCurveStart - 0.5f));
        }
    }

    public override void LateUpdate() {
        base.LateUpdate();

        if(!Pause.IsPaused) {

            // Set main sprite
            this._cartRenderer.sprite = this.rotation.axis == EnumAxis.Y ? this.sprites.frontEmpty : this.sprites.sideEmpty;
            this._cartRenderer.flipX = this.rotation == Rotation.LEFT;

            // Set fill sprite
            if(this.inventory.IsEmpty) {
                this._fillRenderer.sprite = null;
            } else {
                this._fillRenderer.sprite = this.rotation.axis == EnumAxis.Y ? this.sprites.frontFull : this.sprites.sideFull;
                this._fillRenderer.flipX = this.rotation == Rotation.LEFT;
            }
        }
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("inventory", this.inventory.WriteToNbt());
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.inventory.ReadFromNbt(tag.getCompound("inventory"));
    }

    public override bool isDestroyable() {
        return true;
    }

    /// <summary>
    /// Causes the Minecart to explode, removing it from the world
    /// and playing a particle effect.
    /// </summary>
    public void explode() {
        this.world.particles.Spawn(this.position, this.explodeParticlePrefab);
        this.world.entities.Remove(this);
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
        Position p = rotation == null ? this.position : this.position + rotation;

        if(this.world.IsOutOfBounds(p)) {
            return false;
        }

        CellState adjacentState = this.world.GetCellState(p);
        if(adjacentState != null && adjacentState.behavior is IMinecartInteractor) {
            IMinecartInteractor cartInteracter = (IMinecartInteractor)adjacentState.behavior;

            if(cartInteracter.ShouldCartInteract(this)) {
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
        Vector2 destEdgePoint = this.position.Center + (this.rotation.vectorF * 0.5f);
        float dist = Vector2.Distance(this.worldPos, destEdgePoint);

        if(dist <= 0.5f) {
            // the cart is now moving away from the center of a cell and towards another
            Position pos = this.position + this.rotation;
            if(!this.world.IsOutOfBounds(pos)) {
                // check if the cell is occupied by another cart
                foreach(EntityBase e in this.world.entities.list) {
                    if(e is EntityMinecart && e != this && e.position == pos) {
                        // The destinaiton cell is already occuiped.
                        this.worldPos = this.position.Center;
                        return;
                    }
                }
            }
        }

        Vector2 vec = (dir * this.movementSpeed * Mathf.Clamp(Time.deltaTime, 0, 0.1f));
        this.worldPos += vec;
    }

    public override void onDestroy() {
        base.onDestroy();

        this.world.statManager.minecartsDestroyed.increase(1);
    }

    public override void onRightClick() {
        PopupContainer popup = Main.instance.findPopup<PopupContainer>();
        if(popup != null) {
            popup.open();
            popup.SetInventory(this.inventory);
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
