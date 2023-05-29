using System;
using System.Collections;
using UnityEngine;

public class TaskEmote : TaskBase<EntityWorker> {

    [SerializeField]
    private float _emoteVisibleTime = 2f;
    [SerializeField, MinMaxSlider(0, 120), Tooltip("How long the Worker must be still to show an emote")]
    private Vector2 _idleTimeSpanToEmote = new Vector2(10, 30);

    private bool continueRunning;

    public override bool continueExecuting() {
        return continueRunning;
    }

    public override void preform() { }

    public override bool shouldExecute() {
        this.continueRunning = true;
        this.StartCoroutine("wait");

        return true;
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.StopCoroutine("showEmote");
        this.StopCoroutine("wait");
    }

    private IEnumerator showEmote() {
        this.owner.emote.startEmote(new Emote(
            this.pickEmote(),
            this._emoteVisibleTime));

        yield return new WaitForSeconds(this._emoteVisibleTime);

        this.continueRunning = false;
    }

    private IEnumerator wait() {
        float time = UnityEngine.Random.Range(
            this._idleTimeSpanToEmote.x,
            this._idleTimeSpanToEmote.y);

        yield return new WaitForSeconds(time);

        this.StartCoroutine("showEmote");
    }

    private string pickEmote() {
        /*
        if(this.owner.hunger.get() < this.taskFindFood.startFoodHuntAt + 5) {
            return this.icons.hungry;
        }

        if(this.owner.hunger.get() < this.taskSleep.seekBedAt + 5) {
            return this.icons.tired;
        }
        */

        foreach(EntityBase e in this.owner.world.entities.list) {
            if(e is EntityWorker) {
                EntityWorker worker = (EntityWorker)e;
                if(worker.depth == this.owner.depth && Vector2.Distance(worker.WorldPos, this.owner.WorldPos) < 3) {
                    return "happy";
                }
            }
        }

        // Pick a random emotion if there is nothing specific
        switch(UnityEngine.Random.Range(0, 5)) {
            case 0:
            case 1:
                return "happy";
            case 2:
            case 3:
                return "sad";
            case 4:
                return "angry";
            default:
                return "happy";
        }
    }

    [Serializable]
    private class EmoteIcons {
        public Sprite happy = null;
        public Sprite sad = null;
        public Sprite angry = null;

        public Sprite hungry = null;
        public Sprite tired = null;

        public Sprite friendly = null;
    }
}
