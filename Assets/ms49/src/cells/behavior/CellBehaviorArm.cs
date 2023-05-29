using fNbt;
using NaughtyAttributes;
using UnityEngine;

public class CellBehaviorArm : CellBehavior, IHasData {

    private static readonly int armMoveSpeedHash = Animator.StringToHash("ArmMoveSpeed");
    private static readonly int directionHash = Animator.StringToHash("Direction");

    [SerializeField, Min(0.0001f)]
    private float _moveSpeed = 1f;
    [SerializeField, Required]
    private SpriteRenderer _spriteRenderHeldItem = null;
    [SerializeField, Required]
    private Animator _animator = null;

    private Item heldItem;
    private float timer;
    private bool isTargetingFacing;

    public bool IsArmMoving => this.timer > 0;

    private void Start() {
        this._animator.SetFloat(CellBehaviorArm.armMoveSpeedHash, this._moveSpeed);

        this.UpdateItemSprite();
    }

    private void Update() {
        if(Pause.IsPaused) {
            return;
        }

        if(this.IsArmMoving) {
            // Arm is moving, animation is playing...
            this.timer -= Time.deltaTime;
        } else {
            if(this.isTargetingFacing) {
                if(this.heldItem != null) {
                    // Try and drop item off.
                    IContainer container = this.GetContainer(this.rotation);
                    if(container != null && !container.IsFull && container.Deposit(this.heldItem)) {
                        this.SetHeldItem(null);

                        // Return to the source.
                        this.timer = this._moveSpeed;
                        this._animator.SetInteger(directionHash, -1);
                        this.isTargetingFacing = false;
                    } else {
                        // Stop arm.
                        this._animator.SetInteger(directionHash, 0);
                    }
                }
            } else {
                if(this.heldItem == null) {
                    // Try and pick something up.
                    IContainer container = this.GetContainer(this.rotation.opposite());
                    if(container != null && !container.IsEmpty) {
                        this.SetHeldItem(container.PullItem());

                        // Go to destination.
                        this.timer = this._moveSpeed;
                        this._animator.SetInteger(directionHash, 1);
                        this.isTargetingFacing = true;
                    } else {
                        this._animator.SetInteger(directionHash, 0);
                    }
                }
            }
        }
    }

    public void ReadFromNbt(NbtCompound tag) {
        int id = tag.GetInt("heldItem", -1);
        if(id != -1) {
            this.SetHeldItem(Main.instance.ItemRegistry[id]);
        }
    }

    public void WriteToNbt(NbtCompound tag) {
        if(this.heldItem != null) {
            ItemRegistry itemRegistry = Main.instance.ItemRegistry;
            tag.SetTag("heldItem", itemRegistry.GetIdOfElement(this.heldItem));
        }
    }

    private void SetHeldItem(Item item) {
        this.heldItem = item;
        this.UpdateItemSprite();
    }

    private IContainer GetContainer(Rotation direction) {
        Position position = this.pos + direction;

        foreach(EntityBase entity in this.world.entities.list) {
            if(entity.Position == position && entity is IContainer entityContainer) {
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
        this._spriteRenderHeldItem.sprite = this.heldItem == null ? null : this.heldItem.sprite;
    }
}