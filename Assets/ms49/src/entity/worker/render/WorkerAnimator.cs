using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class WorkerAnimator : MonoBehaviour {

    public static readonly string[] STATIC_DIRECTIONS = {
        "IdleDown",
        "IdleSideways",
        "IdleUp",
        "IdleSideways"
    };

    public static readonly string[] RUN_DIRECTIONS = {
        "WalkDown",
        "WalkSideways",
        "WalkUp",
        "WalkSideways"
    };

    private EntityWorker worker;
    private SpriteRenderer sr;
    private Animator animator;

    private Vector2 posLastframe;
    private bool playingCustom;

    private void Awake() {
        this.worker = this.GetComponentInParent<EntityWorker>();
        this.sr = this.GetComponent<SpriteRenderer>();
        this.animator = this.GetComponent<Animator>();
    }

    private void Start() {
        this.posLastframe = this.worker.worldPos;
    }

    private void Update() {
        if(!Pause.isPaused()) {
            Vector2 currentPos = this.worker.worldPos;
            Vector2 direction = currentPos - this.posLastframe;

            if(this.posLastframe != currentPos || !this.playingCustom) {
                // Moved
                this.playingCustom = false;

                string[] directionArray = direction.magnitude == 0 ?
                    STATIC_DIRECTIONS :
                    RUN_DIRECTIONS;

                // Flip the sprite if the Worker is facing right.
                this.sr.flipX = this.worker.rotation == Rotation.RIGHT;

                // Tell the animator to play the requested state
                this.animator.Play(directionArray[this.worker.rotation.id]);
            }

            this.posLastframe = currentPos;
        }
    }

    public void playClip(string clipName) {
        this.playingCustom = true;
        this.sr.flipX = false;

        this.animator.Play(clipName);
    }

    public void stopClip() {
        this.playingCustom = false;
    }

    /// <summary>
    /// Returns the Sprite that the animator is applying to the sprite.
    /// </summary>
    public Sprite getSprite() {
        return this.sr.sprite;
    }
}