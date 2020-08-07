using UnityEngine;
using UnityEngine.UI;
using fNbt;
using System.Text;

public class EntityWorker : EntityBase, IClickable {

    public const float HUNGER_COST_ALIVE = 0.5f;
    public const float HUNGER_COST_MINE = 5f;

    [SerializeField]
    protected Slider healthSlider = null;
    [SerializeField]
    protected Image healthSliderFill = null;
    [SerializeField]
    protected Slider staminaSlider = null;
    [SerializeField]
    protected ParticleSystem sleepingEffect = null;

    /// <summary> The miner's energy from 0 to 100. </summary>
    public float energy;
    /// <summary> The miner's hunger from 0 to 100. </summary>
    public float hunger;
    protected Vector2 posLastFrame;
    public MoveHelper moveHelper { get; private set; }
    protected AiManager<EntityWorker> aiManager;
    protected bool sleeping;

    public WorkerStats stats { get; private set; }
    public WorkerFaces faces { get; private set; }

    private void OnMouseEnter() {
        this.healthSlider.gameObject.SetActive(true);
        this.staminaSlider.gameObject.SetActive(true);
    }

    private void OnMouseExit() {
        this.healthSlider.gameObject.SetActive(false);
        this.staminaSlider.gameObject.SetActive(false);
    }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.moveHelper = this.GetComponent<MoveHelper>();
        this.faces = this.GetComponent<WorkerFaces>();
        this.stats = new WorkerStats();

        this.OnMouseExit(); // Hide ui.

        this.setEnergy(100);
        this.setHunger(100);

        this.aiManager = new AiManager<EntityWorker>(this);
        this.aiManager.addTask(1, new TaskSleep(this, this.moveHelper));
        this.aiManager.addTask(2, new TaskFindFood(this, this.moveHelper));
    }

    public override void onUpdate() {
        base.onUpdate();

        // Remove dead miners.
        if(this.energy <= 0) {
            this.faces.setFace(WorkerFaces.EnumFace.DEAD);
        } else {
            this.aiManager.updateAi();

            this.moveHelper.update();

            if(!this.isSleeping()) {
                if(this.posLastFrame != this.worldPos) {
                    // Miner has moved since last frame.
                }

                // Decrease stamina as the miner lives.
                this.reduceHunger(HUNGER_COST_ALIVE * Time.deltaTime);

                // Decrease energy as the miner is Awake
                this.reduceEnergy(0.1f * Time.deltaTime);

                this.posLastFrame = this.worldPos;
            }
        }
    }

    public void setBarsVisible(bool visible) {
        this.healthSlider.gameObject.SetActive(visible);
        this.staminaSlider.gameObject.SetActive(visible);
    }

    public float getEnergy() {
        return this.energy;
    }

    public void reduceEnergy(float amount) {
        this.setEnergy(this.getEnergy() - amount);

    }

    public void setEnergy(float amount) {
        amount = Mathf.Clamp(amount, 0, 100);
        this.energy = amount;
        this.healthSlider.value = amount;

        this.healthSliderFill.color = this.energy >= 50 ? Color.green : this.energy <= 25 ? new Color(1, 0.5f, 0) : Color.red;
    }

    public float getHunger() {
        return this.hunger;
    }

    public void reduceHunger(float amount) {
        this.setHunger(this.getHunger() - amount);
    }

    public void setHunger(float amount) {
        amount = Mathf.Clamp(amount, 0, 100);
        this.hunger = amount;
        this.staminaSlider.value = amount;
    }

    public void setSleeping(bool isSleeping) {
        this.sleeping = isSleeping;
        if(isSleeping) {
            this.sleepingEffect.Play();
        }
        else {
            this.sleepingEffect.Stop();
        }
    }

    public bool isSleeping() {
        return this.sleeping;
    }

    public virtual void writeWorkerInfo(StringBuilder sb) {
        sb.AppendLine("Energy: " + (int)this.getEnergy());
        sb.AppendLine("Hunger: " + (int)this.getHunger());

        if(Main.DEBUG) {
            sb.Append(this.aiManager.generateDebugText());
        }
    } 

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("energy", this.getEnergy());
        tag.setTag("hunger", this.getHunger());
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.setEnergy(tag.getFloat("energy"));
        this.setHunger(tag.getFloat("hunger"));
    }

    public void onRightClick() {
        UiManager.singleton.popupStats.open();
        UiManager.singleton.popupStats.setWorker(this);
    }

    public void onLeftClick() { }

    public void onMiddleClick() { }
}
