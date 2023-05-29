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

    public override void Initialize(World world, int id) {
        base.Initialize(world, id);

        this.timeRemaining = Random.Range(this.burnTimeRange.x, this.burnTimeRange.y);

        this.StartCoroutine(EntityFire.fireSpread(this.world, this.Position, this.fireSpreadRange));
    }

    public override void Update() {
        if(Pause.IsPaused) {
            return;
        }

        base.Update();

        this.timeRemaining -= Time.deltaTime;
        if(this.timeRemaining <= 0) {
            this.world.SetCell(this.Position, this.ashCell);
            this.world.tryCollapse(this.Position);
            this.world.entities.Remove(this);
        }
    }

    public override void WriteToNbt(NbtCompound tag) {
        base.WriteToNbt(tag);

        tag.SetTag("fireTime", this.timeRemaining);
    }

    public override void ReadFromNbt(NbtCompound tag) {
        base.ReadFromNbt(tag);

        this.timeRemaining = tag.GetFloat("fireTime");
    }

    public static IEnumerator fireSpread(World world, Position thisPos, Vector2 fireSpreadRateRange) {
        yield return new WaitForSeconds(Random.Range(fireSpreadRateRange.x, fireSpreadRateRange.y));

        Rotation r = Rotation.ALL[Random.Range(0, 3)];
        Position newFirePos = thisPos + r;

        if(!world.IsOutOfBounds(newFirePos) && world.GetCellState(newFirePos).data.IsFlammable) {
            bool spaceFree = true;
            foreach(EntityBase e in world.entities.list) {
                if(e is EntityFire && e.Position == newFirePos) {
                    spaceFree = false;
                    break;
                }
            }

            if(spaceFree) {
                world.entities.Spawn(newFirePos, 10);
            }
        }
    }
}
