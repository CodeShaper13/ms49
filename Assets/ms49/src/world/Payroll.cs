using fNbt;
using UnityEngine;

public class Payroll : MonoBehaviour, ISaveableState {

    [SerializeField, Tooltip("How often in seconds Workers are paid")]
    private int _payRate = 60;
    [SerializeField]
    private World _world = null;

    public double lastPayTime { get; private set; }
    public int payRate => this._payRate;

    public string saveableTagName => "payroll";

    private void Update() {
        if(!Pause.IsPaused) {
            if(this._world.time.time > this.lastPayTime + _payRate) {
                // Pay Workers.

                if(!CameraController.instance.inCreativeMode) {
                    IntVariable money = this._world.money;

                    foreach(EntityBase e in this._world.entities.list) {
                        if(e is EntityWorker) {
                            EntityWorker worker = (EntityWorker)e;
                            if(!worker.isDead) {
                                if(money.value < worker.info.pay) {
                                    money.value = 0;
                                    // Play unhappy effect.
                                    worker.emote.startEmote(new Emote("angry", 2f).setTooltip("Didn't get paid"));
                                } else {
                                    money.value -= worker.info.pay;
                                }
                            }
                        }
                    }
                }

                this.lastPayTime = this._world.time.time;
            }
        }
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.lastPayTime = tag.getDouble("lastPayTime");
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.setTag("lastPayTime", this.lastPayTime);
    }
}
