using fNbt;
using UnityEngine;

public class CellBehaviorArm : CellBehavior, IHasData {

    private static int armMoveSpeedHash = Animator.StringToHash("ArmMoveSpeed");
    private static int directionHash = Animator.StringToHash("ArmMoveSpeed");

    [SerializeField, Min(0.0001f)]
    private float _moveSpeed = 1f;
    [SerializeField]
    private SpriteRenderer _sr = null;
    [SerializeField]
    private Animator _animator = null;

    private Item heldItem;
    private float timer;
    private bool targetIsOrgin;

    public bool IsArmMoving => this.timer > 0;

    private void Start() {
        this._animator?.SetFloat(armMoveSpeedHash, this._moveSpeed);
    }

    private void Update() {
        if(Pause.IsPaused) {
            return;
        }

        if(this.IsArmMoving) {
            // Arm is moving, animation is playing...
            this.timer -= Time.deltaTime;
            if(this.timer <= 0) {
                // Reached destination!                
            }
        } else {
            if(this.targetIsOrgin) {
                if(this.heldItem == null) {
                    // Try and pick something up.
                    IContainer container = this.GetContainer(this.rotation);
                    if(container != null && !container.IsEmpty) {
                        this.heldItem = container.PullItem();
                    }
                } else {
                    this.timer = this._moveSpeed;
                    this._animator.SetInteger(directionHash, 1);
                    this.targetIsOrgin = false;
                }
            } else {
                if(this.heldItem != null) {
                    // Try and drop item off.
                    IContainer container = this.GetContainer(this.rotation.opposite());
                    if(container != null && !container.IsFull) {
                        if(container.Deposit(this.heldItem)) {
                            this.heldItem = null;
                        }
                    }
                } else {
                    this.timer = this._moveSpeed;
                    this._animator.SetInteger(directionHash, -1);
                    this.targetIsOrgin = true;
                }
            }
        }
    }

    private IContainer GetContainer(Rotation direction) {
        Position position = this.pos + direction;

        foreach(EntityBase entity in this.world.entities.list) {
            if(entity.position == position && entity is IContainer entityContainer) {
                return entityContainer;
            }
        }

        CellBehavior behavior = this.world.GetCellBehavior(position, true);
        if(behavior is IContainer container) {
            return container;
        }

        return null;
    }

    private void UpdateItemSprite() {
        if(this._sr == null) {
            return;
        }

        if(this.heldItem == null) {
            this._sr.sprite = null;
        } else {
            this._sr.sprite = this.heldItem.sprite;
        }
    }

    public void ReadFromNbt(NbtCompound tag) {
        int id = tag.getInt("heldItem");
        if(id != -1) {
            this.heldItem = Main.instance.ItemRegistry[id];
        }

        this.UpdateItemSprite();
    }

    public void WriteToNbt(NbtCompound tag) {
        ItemRegistry itemRegistry = Main.instance.ItemRegistry;
        tag.setTag(
            "heldItem",
            this.heldItem == null ? -1 : itemRegistry.GetIdOfElement(this.heldItem));
    }
}