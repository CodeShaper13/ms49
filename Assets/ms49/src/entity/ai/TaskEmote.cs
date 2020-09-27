using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TaskEmote : TaskBase<EntityWorker> {

    [SerializeField]
    private float _emoteVisibleTime = 2f;
    [SerializeField, MinMaxSlider(0, 120), Tooltip("How long the Worker must be still to show an emote")]
    private Vector2 _idleTimeSpanToEmote = new Vector2(10, 30);
    [SerializeField]
    private Image emoteBubbleImg = null;
    [SerializeField]
    private Image emoteIconImg = null;

    [Space]

    [SerializeField]
    private EmoteIcons icons = null;

    [Space]

    [SerializeField]
    private TaskFindFood taskFindFood = null;
    [SerializeField]
    private TaskSleep taskSleep = null;

    private bool continueRunning;

    protected override void onStart() {
        base.onStart();

        this.emoteBubbleImg.gameObject.SetActive(false);
    }

    public override bool continueExecuting() {
        return continueRunning;
    }

    public override void preform() { }

    public override bool shouldExecute() {
        this.continueRunning = true;
        this.StartCoroutine(this.wait());

        return true;
    }

    public override void resetTask() {
        base.resetTask();

        this.emoteBubbleImg.gameObject.SetActive(false);

        this.StopAllCoroutines();
    }

    private IEnumerator showEmote() {
        this.emoteBubbleImg.gameObject.SetActive(true);
        this.emoteIconImg.sprite = this.pickEmote();

        yield return new WaitForSeconds(this._emoteVisibleTime);

        this.continueRunning = false;
    }

    private IEnumerator wait() {
        float time = UnityEngine.Random.Range(
            this._idleTimeSpanToEmote.x,
            this._idleTimeSpanToEmote.y);

        yield return new WaitForSeconds(time);

        this.StartCoroutine(this.showEmote());
    }

    private Sprite pickEmote() {
        if(this.owner.hunger.get() < this.taskFindFood.startFoodHuntAt + 5) {
            return this.icons.hungry;
        }

        if(this.owner.hunger.get() < this.taskSleep.seekBedAt + 5) {
            return this.icons.tired;
        }

        foreach(EntityBase e in this.owner.world.entities.list) {
            if(e is EntityWorker) {
                EntityWorker worker = (EntityWorker)e;
                if(worker.depth == this.owner.depth && Vector2.Distance(worker.worldPos, this.owner.worldPos) < 3) {
                    return this.icons.friendly;
                }
            }
        }

        // Pick a random emotion if there is nothing specific
        switch(UnityEngine.Random.Range(0, 5)) {
            case 0:
            case 1:
                return this.icons.happy;
            case 2:
            case 3:
                return this.icons.sad;
            case 4:
                return this.icons.angry;
            default:
                return this.icons.happy;
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
