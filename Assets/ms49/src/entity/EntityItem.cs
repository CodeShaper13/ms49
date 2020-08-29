using fNbt;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EntityItem : EntityBase {

    private SpriteRenderer sr;

    /// <summary> The belt that is moving the item.  May be null. </summary>
    private CellBehaviorConveyorBelt belt;

    public Item item { get; private set; }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.sr = this.GetComponent<SpriteRenderer>();
    }

    public override void onUpdate() {
        base.onUpdate();
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("itemId", Main.instance.itemRegistry.getIdOfElement(this.item));
        //if(this.belt != null) {
        //    tag.setTag("beltPos")
        //}
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.item = Main.instance.itemRegistry.getElement(tag.getInt("itemId"));
        //Position p = new Position
        //this.belt = this.world.getBehavior<CellBehaviorConveyorBelt>(p);
    }

    public void setItem(Item item) {
        this.item = item;
        this.sr.sprite = item == null ? null : item.sprite;
    }
}
