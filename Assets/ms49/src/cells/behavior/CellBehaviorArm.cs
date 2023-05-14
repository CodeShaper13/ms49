using fNbt;
using UnityEngine;

public class CellBehaviorArm : CellBehavior, IHasData {

    [SerializeField, Min(0.0001f)]
    private float _moveSpeed = 1f;
    [SerializeField]
    private SpriteRenderer _sr = null;
    [SerializeField]
    private Animator _animator = null;

    private Item heldItem;

    private void Start() {
        this._animator?.SetFloat("armMoveSpeed", this._moveSpeed);
    }

    public override void onUpdate() {
        base.onUpdate();

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