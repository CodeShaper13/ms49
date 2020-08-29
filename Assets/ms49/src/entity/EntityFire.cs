using UnityEngine;
using fNbt;
using System.Collections;

public class EntityFire : EntityBase {

    [SerializeField]
    private CellData ashCell = null;
    [SerializeField]
    private Vector2 burnTimeRange = new Vector2(1f, 2f);
    [SerializeField]
    private Vector2 fireSpreadRange = new Vector2(1f, 2f);

    private float timeRemaining;

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.timeRemaining = Random.Range(this.burnTimeRange.x, this.burnTimeRange.y);

        this.StartCoroutine(EntityFire.fireSpread(this.world, this.position, this.fireSpreadRange));
    }

    public override void onUpdate() {
        base.onUpdate();

        this.timeRemaining -= Time.deltaTime;
        if(this.timeRemaining <= 0) {
            this.world.setCell(this.position, this.ashCell);
            this.world.tryCollapse(this.position);
            this.world.entities.remove(this);
        }
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("fireTime", this.timeRemaining);
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.timeRemaining = tag.getFloat("fireTime");
    }

    public static IEnumerator fireSpread(World world, Position thisPos, Vector2 fireSpreadRateRange) {
        yield return new WaitForSeconds(Random.Range(fireSpreadRateRange.x, fireSpreadRateRange.y));

        Rotation r = Rotation.ALL[Random.Range(0, 3)];
        Position newFirePos = thisPos + r;

        if(world.getCellState(newFirePos).data.isFlammable) {
            bool spaceFree = true;
            foreach(EntityBase e in world.entities.list) {
                if(e is EntityFire && e.position == newFirePos) {
                    spaceFree = false;
                    break;
                }
            }

            if(spaceFree) {
                world.entities.spawn(newFirePos, 10);
            }
        }
    }
}
