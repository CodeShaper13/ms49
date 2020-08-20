using UnityEngine;
using fNbt;
using System.Collections;

public class EntityFire : EntityBase {

    [SerializeField]
    private CellData ashCell = null;
    [SerializeField]
    private int entityFireId = 0;
    [SerializeField]
    private Vector2 burnTimeRange = new Vector2(1f, 2f);
    [SerializeField]
    private Vector2 fireSpreadRange = new Vector2(1f, 2f);

    private float timeRemaining;

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.timeRemaining = Random.Range(this.burnTimeRange.x, this.burnTimeRange.y);

        this.StartCoroutine(this.fireSpread());
    }

    public override void onUpdate() {
        base.onUpdate();

        this.timeRemaining -= Time.deltaTime;
        if(this.timeRemaining <= 0) {
            this.world.setCell(this.position, this.ashCell);
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

    private IEnumerator fireSpread() {
        yield return new WaitForSeconds(Random.Range(this.fireSpreadRange.x, this.fireSpreadRange.y));

        Rotation r = Rotation.ALL[Random.Range(0, 3)];
        Position newFirePos = this.position + r;

        bool spaceFree = true;
        foreach(EntityBase e in this.world.entities.entityList) {
            if(e is EntityFire && e.position == newFirePos) {
                spaceFree = false;
                break;
            }
        }

        if(spaceFree && this.world.getCellState(newFirePos).data.isFlammable) {
            this.world.entities.spawn(newFirePos, this.entityFireId);
        }
    }
}
