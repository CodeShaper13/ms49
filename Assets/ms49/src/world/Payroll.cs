using fNbt;
using UnityEngine;

public class Payroll : MonoBehaviour, ISaveableSate {

    [SerializeField, Tooltip("How often in seconds Workers are paid")]
    private int _payRate = 60 * 24;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private World world = null;

    private double lastPayTime;

    public string tagName => "payroll";

    private void Update() {
        if(!Pause.isPaused()) {
            if(this.world.time.time > this.lastPayTime + _payRate) {
                // Pay Workers.
                Debug.Log("Paying Workers");
                foreach(EntityBase e in this.world.entities.list) {
                    if(e is EntityWorker) {
                        EntityWorker worker = (EntityWorker)e;
                        if(!worker.isDead) {
                            this.money.value -= worker.info.pay;
                        }
                    }
                }

                this.lastPayTime = this.world.time.time;
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        tag.setTag("lastPayTime", this.lastPayTime);
    }

    public void writeToNbt(NbtCompound tag) {
        this.lastPayTime = tag.getDouble("lastPayTime");
    }
}
