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

    public override void OnBehaviorDestroy() {
        base.OnBehaviorDestroy();

        /*
        for(int i = this.world.entities.count - 1; i >= 0; i--) {
            EntityBase entity = this.world.entities.list[i];
            if(entity is EntityItem && entity.position == this.pos) {
                this.destroyItem(entity);
            }
        }
        */
    }

    public void OnLeverFlip(CellBehavior lever) {
        this.state.Rotation = this.state.Rotation.opposite();
        this.dirty();
    }

    private void Update() {
        this.pullTimer -= Time.deltaTime;

        // Attempt to pull Items from a container.
        if(this.pullTimer <= 0) {
            CellBehavior behavior = this.world.GetCellBehavior(this.pos + this.rotation.opposite(), true);
            if(behavior is IContainer container) {
                if(!container.IsEmpty) {
                    // Spawn Item
                    Vector2 entityPos = this.center + (this.rotation.opposite().vectorF * 0.49f);
                    EntityItem e = (EntityItem)this.world.entities.Spawn(
                        entityPos,
                        this.pos.depth,
                        2); // 2 is EntityItem ID
                    e.SetItem(container.PullItem());

                    this.pullTimer = this.itemPullSpeed;
                }
            }
        }

        // Move Items along the conveyor belt.
        for(int i = this.world.entities.EntityCount - 1; i >= 0; i--) {
            EntityBase entity = this.world.entities.list[i];

            if(entity is EntityItem entityItem && entity.Position == this.pos) {
                Position entityStartPos = entity.Position;

                // If the Entity is on the main axis, move along the belt
                if(
                    (this.rotation.axis == EnumAxis.X && entity.WorldPos.y == this.center.y) ||
                    (this.rotation.axis == EnumAxis.Y && entity.WorldPos.x == this.center.x))
                    {
                    entity.WorldPos += this.rotation.vectorF * conveyorSpeed * Time.deltaTime;

                } else {
                    // Move towards the center
                    if(this.rotation.axis == EnumAxis.X) {
                        // Belt goes side to side
                        float y = Mathf.MoveTowards(entity.WorldPos.y, this.center.y, conveyorSpeed * Time.deltaTime);
                        entity.WorldPos = new Vector2(entity.WorldPos.x, y);
                    } else {
                        // Belt goes up and down
                        float x = Mathf.MoveTowards(entity.WorldPos.x, this.center.x, conveyorSpeed * Time.deltaTime);
                        entity.WorldPos = new Vector2(x, entity.WorldPos.y);
                    }
                }

                if(entity.Position != entityStartPos) {
                    // Entity has moved to a new Cell.

                    CellBehavior behavior = this.world.GetCellBehavior(entity.Position, true);

                    if(behavior is IContainer container) {
                        if(!container.IsFull) {
                            container.Deposit(entityItem.item);
                            this.world.entities.Remove(entity);
                        } else {
                            this.destroyItem(entity);
                        }
                    } else if(!(behavior is CellBehaviorConveyorBelt)) {
                        this.destroyItem(entity);
                    }
                }
            }                
        }        
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.pullTimer = tag.GetFloat("conveyorPullTimer");
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.SetTag("conveyorPullTimer", this.pullTimer);
    }

    private void destroyItem(EntityBase entity) {
        if(this.itemDestoryParticle != null) {
            this.world.particles.Spawn(entity.WorldPos, entity.depth, this.itemDestoryParticle);
        }

        this.world.entities.Remove(entity);
    }
}
