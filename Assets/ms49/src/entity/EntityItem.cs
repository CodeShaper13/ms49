using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EntityItem : EntityBase {

    private SpriteRenderer sr;
    private Item _item;

    public Item item {
        get {
            return this._item;
        } set {
            this._item = value;
            this.sr.sprite = value == null ? null : value.sprite;
        }
    }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.sr = this.GetComponent<SpriteRenderer>();
    }
}
