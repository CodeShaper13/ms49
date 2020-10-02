using fNbt;
using UnityEngine;

public class Payroll : MonoBehaviour, ISaveableState {

    [SerializeField, Tooltip("How often in seconds Workers are paid")]
    private int _payRate = 60 * 24;
    [SerializeField]
    private IntVariable _money = null;
    [SerializeField]
    private World _world = null;

    public double lastPayTime { get; private set; }
    public int payRate => this._payRate;

    public string tagName => "payroll";

    private void Update() {
        if(!Pause.isPaused()) {
            if(this._world.time.time > this.lastPayTime + _payRate) {
                // Pay Workers.
                foreach(EntityBase e in this._world.entities.list) {
                    if(e is EntityWorker) {
                        EntityWorker worker = (EntityWorker)e;
                        if(!worker.isDead) {
                            this._money.value -= worker.info.pay;
                        }
                    }
                }

                this.lastPayTime = this._world.time.time;
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        this.lastPayTime = tag.getDouble("lastPayTime");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("lastPayTime", this.lastPayTime);
    }
}
