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

    public override void Update() {
        if(Pause.IsPaused) {
            return;
        }
        
        base.Update();

        // Terrible, make this better
        this._nameText.text = this.info.lastName;
//        this.moveHelper.speedMultiplyer = this.info.personality.moveSpeedMultiplyer;

        if(this.isDead) {
            this.animator.playClip("Dead");
        }
        else {
            this.aiManager.updateAi();

            // Kill the Worker if there heath or energy is too low.
            if(this.hunger.value <= this.hunger.minValue || this.energy.value <= this.energy.minValue || this.temperature.value >= this.temperature.maxValue) {
                this.kill();
            }

            if(this.happiness.value <= this.happiness.minValue) {
                // Work is leaving.
                this.world.entities.Remove(this);
            }
        }
    }

    public override void LateUpdate() {
        if(Pause.IsPaused) {
            return;
        }

        base.LateUpdate();

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
                this.sleepParticle = this.world.particles.Spawn(this.worldPos + Vector2.up, this.depth, this.sleepingParticlePrefab);
            }
            this.isSleeping = true;
        } else {
            if(this.sleepParticle != null) {
                this.world.particles.Remove(this.sleepParticle);
                this.sleepParticle = null;
            }
            this.isSleeping = false;
        }
    }

    public virtual void writeWorkerInfo(StringBuilder sb) {
        sb.AppendLine("Energy: " + (int)this.energy.value);
        sb.AppendLine("Hunger: " + (int)this.hunger.value);
    }

    public override void getDebugText(StringBuilder sb, string indent) {
        base.getDebugText(sb, indent);

        sb.AppendLine(indent + "Type: " + this.type.typeName);
        sb.AppendLine(indent + "Name: " + this.info.fullName);
        sb.AppendLine(indent + "Dead: " + this.isDead);
        sb.AppendLine(indent + "Sleeping: " + this.isSleeping);
        sb.AppendLine(indent + "Hunger: " + this.hunger.value);
        sb.AppendLine(indent + "Energy: " + this.energy.value);
        sb.AppendLine(indent + "Temperature: " + this.temperature.value);

        CookMetaData meta = this.GetComponentInChildren<CookMetaData>();
        if(meta != null) {
            sb.AppendLine(indent + "Plate: " + meta.plateState);
        }

        this.aiManager.generateDebugText(sb, indent);
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("energy", this.energy.value);
        tag.setTag("hunger", this.hunger.value);
        tag.setTag("temperature", this.temperature.value);
        tag.setTag("happiness", this.happiness.value);
        tag.setTag("isDead", this.isDead);
        tag.setTag("workerInfo", this.info.writeToNbt());
        tag.setTag("workerType", Main.instance.WorkerTypeRegistry.GetIdOfElement(this.type));

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
        this.setType(Main.instance.WorkerTypeRegistry[tag.getInt("workerType")]);

        // Read Ai Meta objects from NBT
        foreach(IAiMeta meta in this.GetComponentsInChildren<IAiMeta>()) {
            meta.readFromNbt(tag);
        }
    }

    /// <summary>
    /// Called when the Worker dies
    /// </summary>
    public virtual void onDeath() {
        this.moveHelper.enabled = false;
    }

    public override void OnRightClick() {
        PopupWorkerStats popup = Main.instance.FindPopup<PopupWorkerStats>();
        if(popup != null) {
            popup.open();
            popup.setWorker(this);
        }
    }
}