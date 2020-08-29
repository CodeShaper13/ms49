using UnityEngine;
using UnityEngine.UI;
using fNbt;
using System.Text;

public class EntityWorker : EntityBase, IClickable {

    public const float HUNGER_COST_ALIVE = 0.5f;
    public const float HUNGER_COST_MINE = 5f;

    [SerializeField]
    private string _typeName = "???";

    [Space]

    [SerializeField]
    protected Slider healthSlider = null;
    [SerializeField]
    protected Image healthSliderFill = null;
    [SerializeField]
    protected Slider staminaSlider = null;
    [SerializeField]
    protected ParticleSystem sleepingEffect = null;
    [SerializeField, Tooltip("If this milestone is unlocked, hunger and energy are lowered")]
    private MilestoneData milestone = null;

    /// <summary> The miner's energy from 0 to 100. </summary>
    public float energy { get; private set; }
    /// <summary> The miner's hunger from 0 to 100. </summary>
    public float hunger; // { get; private set; }
    public Vector2 posLastFrame { get; private set; }
    public string typeName { get { return this._typeName; } }
    public bool isDead { get; private set; }
    public bool isSleeping { get; private set; }

    public MoveHelper moveHelper { get; private set; }
    public AiManager aiManager { get; private set; }
    public WorkerStats stats { get; private set; }
    public WorkerAnimator animator { get; private set; }

    private void OnMouseEnter() {
        if(this.statGameplayEnabled()) {
            this.setBarsVisible(true);
        }
    }

    private void OnMouseExit() {
        this.setBarsVisible(false);
    }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.moveHelper = this.GetComponent<MoveHelper>();
        this.stats = new WorkerStats();
        this.animator = this.GetComponentInChildren<WorkerAnimator>();

        this.OnMouseExit(); // Hide ui.

        this.setEnergy(100);
        this.setHunger(100);

        this.aiManager = new AiManager();
        this.aiManager.addTask(1, new TaskSleep(this, this.moveHelper));
        this.aiManager.addTask(2, new TaskFindFood(this, this.moveHelper));
    }

    public override void onUpdate() {
        base.onUpdate();

        if(this.isDead) {
            this.animator.playClip("Dead");
        }
        else {
            this.aiManager.updateAi();

            this.moveHelper.update();

            if(!this.isSleeping) {
                if(this.posLastFrame != this.worldPos) {
                    // Miner has moved since last frame.
                }

                if(this.statGameplayEnabled()) {
                    // Decrease stamina as the miner lives.
                    this.reduceHunger(HUNGER_COST_ALIVE * Time.deltaTime);

                    // Decrease energy as the miner is Awake
                    this.reduceEnergy(0.1f * Time.deltaTime);
                }

                this.posLastFrame = this.worldPos;
            }

            // Kill the Worker if there heath or energy is too low.
            if(this.hunger <= 0 || this.energy <= 0) {
                this.kill();
            }
        }
    }

    public void kill() {
        this.isDead = true;

        this.aiManager.stopAllTasks();

        this.onDeath();
    }

    public void setBarsVisible(bool visible) {
        this.healthSlider.gameObject.SetActive(visible);
        this.staminaSlider.gameObject.SetActive(visible);
    }

    /// <summary>
    /// Reduces the Workers hunger, only if stats are enabled (depending on if the Milestones in the EntityWorker#milestone field is set).
    /// </summary>
    public void reduceEnergy(float amount) {
        if(this.statGameplayEnabled()) {
            this.setEnergy(this.energy - amount);
        }
    }

    /// <summary>
    /// Reduces the Workers hunger, only if stats are enabled (depending on if the Milestones in the EntityWorker#milestone field is set).
    /// </summary>
    public void setEnergy(float amount) {
        amount = Mathf.Clamp(amount, 0, 100);
        this.energy = amount;
        this.healthSlider.value = amount;

        this.healthSliderFill.color = this.energy >= 50 ? Color.green : this.energy <= 25 ? new Color(1, 0.5f, 0) : Color.red;
    }

    /// <summary>
    /// Reduces the Workers hunger, only if stats are enabled (depending on if the Milestones in the EntityWorker#milestone field is set).
    /// </summary>
    public void reduceHunger(float amount) {
        if(this.statGameplayEnabled()) {
            this.setHunger(this.hunger - amount);
        }
    }

    /// <summary>
    /// Increases the Workers hunger, only if stats are enabled (depending on if the Milestones in the EntityWorker#milestone field is set).
    /// </summary>
    public void increaseHunger(float amount) {
        if(this.statGameplayEnabled()) {
            this.setHunger(this.hunger + amount);
        }
    }

    public void setHunger(float amount) {
        amount = Mathf.Clamp(amount, 0, 100);
        this.hunger = amount;
        this.staminaSlider.value = amount;
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
        sb.AppendLine("Energy: " + (int)this.energy);
        sb.AppendLine("Hunger: " + (int)this.hunger);

        if(Main.DEBUG) {
            sb.Append(this.aiManager.generateDebugText());
        }
    } 

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("energy", this.energy);
        tag.setTag("hunger", this.hunger);
        tag.setTag("isDead", this.isDead);
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.setEnergy(tag.getFloat("energy"));
        this.setHunger(tag.getFloat("hunger"));
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
    /// Called when the Worker is clicking with the middle mouse button.
    /// </summary>
    public virtual void onMiddleClick() { }

    private bool statGameplayEnabled() {
        return this.milestone == null || this.milestone.isUnlocked;
    }
}