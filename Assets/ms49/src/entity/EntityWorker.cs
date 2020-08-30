using UnityEngine;
using fNbt;
using System.Text;

public class EntityWorker : EntityBase, IClickable {

    [SerializeField]
    private string _typeName = "???";

    [Space]

    [SerializeField]
    protected Canvas tooltipCanvas = null;
    [SerializeField]
    protected ParticleSystem sleepingEffect = null;

    public UnlockableStat hunger;
    public UnlockableStat energy;
    public UnlockableStat temperature;
    public AiManager aiManager;

    public Vector2 posLastFrame { get; private set; }
    public string typeName { get { return this._typeName; } }
    public bool isDead { get; private set; }
    public bool isSleeping { get; private set; }

    public MoveHelper moveHelper { get; private set; }
    public WorkerStats stats { get; private set; }
    public WorkerAnimator animator { get; private set; }

    private void OnMouseEnter() {
        this.tooltipCanvas.enabled = true;
    }

    private void OnMouseExit() {
        this.tooltipCanvas.enabled = false;
    }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.moveHelper = this.GetComponent<MoveHelper>();
        this.stats = new WorkerStats();
        this.animator = this.GetComponentInChildren<WorkerAnimator>();

        this.OnMouseExit(); // Hide Canvas
    }

    public override void onUpdate() {
        base.onUpdate();

        if(this.isDead) {
            this.animator.playClip("Dead");
        }
        else {
            this.aiManager.updateAi();
            this.moveHelper.update();

            if(this.posLastFrame != this.worldPos) {
                // Miner has moved since last frame.
            }

            this.posLastFrame = this.worldPos;

            // Kill the Worker if there heath or energy is too low.
            if(this.hunger.value <= this.hunger.minValue || this.energy.value <= this.energy.minValue || this.temperature.value >= this.temperature.maxValue) {
                this.kill();
            }
        }
    }

    public void kill() {
        this.isDead = true;

        this.aiManager.stopAllTasks();

        this.onDeath();
    }

    public void setSleeping(bool isSleeping) {
        this.isSleeping = isSleeping;
        if(isSleeping) {
            this.sleepingEffect.Play();
        }
        else {
            this.sleepingEffect.Stop();
        }
    }

    public virtual void writeWorkerInfo(StringBuilder sb) {
        sb.AppendLine("Energy: " + (int)this.energy.value);
        sb.AppendLine("Hunger: " + (int)this.hunger.value);

        if(Main.DEBUG) {
            sb.Append(this.aiManager.generateDebugText());
        }
    } 

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("energy", this.energy.value);
        tag.setTag("hunger", this.hunger.value);
        tag.setTag("temperature", this.temperature.value);
        tag.setTag("isDead", this.isDead);
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.energy.value = tag.getFloat("energy");
        this.hunger.value = tag.getFloat("hunger");
        this.temperature.value = tag.getFloat("temperature");
        this.isDead = tag.getBool("isDead");
    }

    /// <summary>
    /// Called when the Worker dies
    /// </summary>
    public virtual void onDeath() { }

    /// <summary>
    /// Called when the Worker is right clicked.
    /// </summary>
    public virtual void onRightClick() {
        PopupWorkerStats popup = Resources.FindObjectsOfTypeAll<PopupWorkerStats>()[0];
        if(popup != null) {
            popup.open();
            popup.setWorker(this);
        }
    }

    /// <summary>
    /// Called when the Worker is left clicked.
    /// </summary>
    public virtual void onLeftClick() { }

    /// <summary>
    /// Called when the Worker is clicked with the middle mouse button.
    /// </summary>
    public virtual void onMiddleClick() { }
}