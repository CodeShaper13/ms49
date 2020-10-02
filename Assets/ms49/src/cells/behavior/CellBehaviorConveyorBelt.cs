using fNbt;
using UnityEngine;

public class CellBehaviorConveyorBelt : CellBehavior, IHasData, ILeverReciever {

    [SerializeField, Min(0)]
    private float conveyorSpeed = 1;
    [SerializeField, Min(0), Tooltip("How often the conveyor belt can pull an item from a container.")]
    private float itemPullSpeed = 1f;
    [SerializeField]
    private GameObject itemDestoryParticle = null;

    private float pullTimer;

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);
    }

    public override void onDestroy() {
        base.onDestroy();

        for(int i = this.world.entities.count - 1; i >= 0; i--) {
            EntityBase entity = this.world.entities.list[i];
            if(entity is EntityItem && entity.position == this.pos) {
                this.destroyItem(entity);
            }
        }
    }

    public void onLeverFlip(CellBehaviorLever lever) {
        this.state.rotation = this.state.rotation.opposite();
        this.dirty();
    }

    public override void onUpdate() {
        base.onUpdate();

        this.pullTimer -= Time.deltaTime;

        // Attempt to pull Items from a container.
        if(this.pullTimer <= 0) {
            CellBehavior behavior = this.world.getBehavior(this.pos + this.rotation.opposite());
            if(behavior is IContainer) {
                IContainer container = (IContainer)behavior;
                if(!container.isEmpty) {
                    // Spawn Item
                    Vector2 entityPos = this.center + (this.rotation.opposite().vectorF * 0.49f);
                    EntityItem e = (EntityItem)this.world.entities.spawn(
                        entityPos,
                        this.pos.depth,
                        3); // 3 is EntityItem ID
                    e.setItem(container.pullItem());

                    this.pullTimer = this.itemPullSpeed;
                }
            }
        }

        // Move Items along the conveyor belt.
        for(int i = this.world.entities.count - 1; i >= 0; i--) {
            EntityBase entity = this.world.entities.list[i];

            if(entity is EntityItem && entity.position == this.pos) {
                Position entityStartPos = entity.position;

                // If the Entity is on the main axis, move along the belt
                if(
                    (this.rotation.axis == EnumAxis.X && entity.worldPos.y == this.center.y) ||
                    (this.rotation.axis == EnumAxis.Y && entity.worldPos.x == this.center.x))
                    {
                    entity.worldPos += this.rotation.vectorF * conveyorSpeed * Time.deltaTime;

                } else {
                    // Move towards the center
                    if(this.rotation.axis == EnumAxis.X) {
                        // Belt goes side to side
                        float y = Mathf.MoveTowards(entity.worldPos.y, this.center.y, conveyorSpeed * Time.deltaTime);
                        entity.worldPos = new Vector2(entity.worldPos.x, y);
                    } else {
                        // Belt goes up and down
                        float x = Mathf.MoveTowards(entity.worldPos.x, this.center.x, conveyorSpeed * Time.deltaTime);
                        entity.worldPos = new Vector2(x, entity.worldPos.y);
                    }
                }

                if(entity.position != entityStartPos) {
                    // Entity has moved to a new Cell.

                    CellBehavior behavior = this.world.getBehavior(entity.position);

                    if(behavior is IContainer) {
                        IContainer container = (IContainer)behavior;
                        if(!container.isFull) {
                            container.deposit(((EntityItem)entity).item);
                            this.world.entities.remove(entity);
                        }
                    } else if(!(behavior is CellBehaviorConveyorBelt)) {
                        this.destroyItem(entity);
                    }
                }
            }                
        }        
    }

    public void readFromNbt(NbtCompound tag) {
        this.pullTimer = tag.getFloat("conveyorPullTimer");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("conveyorPullTimer", this.pullTimer);
    }

    private void destroyItem(EntityBase entity) {
        if(this.itemDestoryParticle != null) {
            this.world.particles.spawn(entity.worldPos, entity.depth, this.itemDestoryParticle);
        }

        this.world.entities.remove(entity);
    }
}
