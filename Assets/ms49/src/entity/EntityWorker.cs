using UnityEngine;
using fNbt;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

public class EntityWorker : EntityBase {

    [SerializeField]
    private Text _nameText = null;
    [SerializeField]
    private GameObject sleepingParticlePrefab = null;

    // References
    public UnlockableStat hunger;
    public UnlockableStat energy;
    public UnlockableStat temperature;
    public UnlockableStat happiness;
    public AiManager aiManager;
    public DirectionalSpriteSwapper hatSpriteSwapper;
    public EmoteBubble emote;
    public EmoteBubble overrideEmoteBubble;

    public WorkerType type { get; private set; }
    public bool isDead { get; private set; }
    public bool isSleeping { get; private set; }

    public PathfindingAgent moveHelper { get; private set; }
    public WorkerInfo info { get; set; }
    public WorkerAnimator animator { get; private set; }

    private Particle sleepParticle;

    private void OnMouseEnter() {
        this._nameText.gameObject.SetActive(true);
    }

    private void OnMouseExit() {
        this._nameText.gameObject.SetActive(false);
    }

    public override void initialize(World world, int id) {
        base.initialize(world, id);

        this.moveHelper = this.GetComponent<PathfindingAgent>();
        this.animator = this.GetComponentInChildren<WorkerAnimator>();

        this.OnMouseExit(); // Hide Canvas
    }

    public override void onUpdate() {
        base.onUpdate();

        // Terrible, make this better
        this._nameText.text = this.info.lastName;
        this.moveHelper.speedMultiplyer = this.info.personality.moveSpeedMultiplyer;

        if(this.isDead) {
            this.animator.playClip("Dead");
        }
        else {
            this.aiManager.updateAi();
            this.moveHelper.update();

            // Kill the Worker if there heath or energy is too low.
            if(this.hunger.value <= this.hunger.minValue || this.energy.value <= this.energy.minValue || this.temperature.value >= this.temperature.maxValue) {
                this.kill();
            }

            if(this.happiness.value <= this.happiness.minValue) {
                // Work is leaving.
                this.world.entities.remove(this);
            }
        }
    }

    public override void onLateUpdate() {
        base.onLateUpdate();

        this.animator.rotation = this.rotation;
    }

    public void setType(WorkerType type) {
        if(this.type != null) {
            Debug.LogWarning("A Worker's type can only be set once!");
            return;
        }

        this.type = type;

        // Setup hat
        this.hatSpriteSwapper.sprites = this.type.hat;

        // Setup extra AI
        if(this.type.aiPrefab != null) {
            GameObject.Instantiate(this.type.aiPrefab, this.aiManager.transform);
        }
    }

    public void kill() {
        this.isDead = true;

        this.aiManager.stopAllTasks();

        this.onDeath();

        //this.world.entities.remove(this);
    }

    public void setSleeping(bool sleeping) {
        if(sleeping) {
            if(!this.isSleeping) {
                this.sleepParticle = this.world.particles.spawn(this.worldPos + Vector2.up, this.depth, this.sleepingParticlePrefab);
            }
            this.isSleeping = true;
        } else {
            if(this.sleepParticle != null) {
                this.world.particles.remove(this.sleepParticle);
                this.sleepParticle = null;
            }
            this.isSleeping = false;
        }
    }

    public virtual void writeWorkerInfo(StringBuilder sb) {
        sb.AppendLine("Energy: " + (int)this.energy.value);
        sb.AppendLine("Hunger: " + (int)this.hunger.value);
    }

    public override void getDebugText(List<string> s) {
        base.getDebugText(s);

        s.Add("Type: " + this.type.typeName);
        s.Add("Name: " + this.info.fullName);
        s.Add("Dead: " + this.isDead);
        s.Add("Sleeping: " + this.isSleeping);
        s.Add("Hunger: " + this.hunger.value);
        s.Add("Energy: " + this.energy.value);
        s.Add("Temperature: " + this.temperature.value);

        CookMetaData meta = this.GetComponentInChildren<CookMetaData>();
        if(meta != null) {
            s.Add("Plate: " + meta.plateState);
        }

        this.aiManager.generateDebugText(s);
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("energy", this.energy.value);
        tag.setTag("hunger", this.hunger.value);
        tag.setTag("temperature", this.temperature.value);
        tag.setTag("happiness", this.happiness.value);
        tag.setTag("isDead", this.isDead);
        tag.setTag("workerInfo", this.info.writeToNbt());
        tag.setTag("workerType", Main.instance.workerTypeRegistry.getIdOfElement(this.type));

        // Write Ai Meta objects to NBT
        foreach(IAiMeta meta in this.GetComponentsInChildren<IAiMeta>()) {
            meta.writeToNbt(tag);
        }
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.energy.value = tag.getFloat("energy");
        this.hunger.value = tag.getFloat("hunger");
        this.temperature.value = tag.getFloat("temperature");
        this.happiness.value = tag.getFloat("happiness");
        this.isDead = tag.getBool("isDead");
        this.info = new WorkerInfo(tag.getCompound("workerInfo"));
        this.setType(Main.instance.workerTypeRegistry.getElement(tag.getInt("workerType")));

        // Read Ai Meta objects from NBT
        foreach(IAiMeta meta in this.GetComponentsInChildren<IAiMeta>()) {
            meta.readFromNbt(tag);
        }
    }

    /// <summary>
    /// Called when the Worker dies
    /// </summary>
    public virtual void onDeath() { }

    public override void onRightClick() {
        PopupWorkerStats popup = Main.instance.findPopup<PopupWorkerStats>();
        if(popup != null) {
            popup.open();
            popup.setWorker(this);
        }
    }
}