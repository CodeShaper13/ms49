using System.Collections.Generic;
using UnityEngine;

public class CellBehaviorConveyorBelt : CellBehavior {

    private const float speed = 1;

    private List<EntityItem> trackedItems;

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);

        this.trackedItems = new List<EntityItem>();
    }

    public override void onUpdate() {
        base.onUpdate();

        // Move items along belt
        for(int i = this.trackedItems.Count - 1; i >= 0; i--) {
            EntityItem e = this.trackedItems[i];

            e.worldPos += (this.rotation.vectorF * speed) * Time.deltaTime;

            CellBehavior behavior = this.world.getBehavior(this.pos + this.rotation);
            Vector2 conveyorMiddle = this.pos.vec2 + (Vector2.one / 2);
            float distance = Vector2.Distance(conveyorMiddle, e.transform.position);

            if(behavior is CellBehaviorConveyorBelt) {
                CellBehaviorConveyorBelt nextBelt = (CellBehaviorConveyorBelt)behavior;

                // There is a belt next
                float checkDis;
                if(nextBelt.rotation.axis == EnumAxis.Y) {
                    checkDis = 1f;
                } else {
                    checkDis = this.rotation == Rotation.UP ? 0.9f : 1.1f;
                }

                if(distance > checkDis) {
                    CellBehaviorConveyorBelt belt =
                        this.world.getBehavior<CellBehaviorConveyorBelt>(this.pos + this.rotation);

                    this.trackedItems.RemoveAt(i);
                    belt.startTracking(e);
                }
            } else if(behavior is CellBehaviorDepositPoint) {
                if(distance > 0.5f) {
                    this.trackedItems.RemoveAt(i);
                    this.world.entities.remove(e);
                    ((CellBehaviorDepositPoint)behavior).deposit(e.item);
                }
            } else {
                // Nothing next
                if(distance > 0.5f) {
                    // Destory item
                    this.trackedItems.RemoveAt(i);
                    this.world.entities.remove(e);
                    print("Item Bang!");
                }
            }
        }
    }

    public void startTracking(EntityItem item) {
        this.trackedItems.Add(item);
    }

    public void addItem(Item item) {
        float yOffset = this.rotation.axis == EnumAxis.X ? 0.4f : 0f;
        Vector2 v = this.rotation.opposite().vectorF * 0.5f + new Vector2(0, yOffset);

        EntityItem e = (EntityItem)this.world.entities.spawn(this.pos.vec2 + v, this.pos.depth, 3);
        e.item = item;
        this.startTracking(e);
    }
}
