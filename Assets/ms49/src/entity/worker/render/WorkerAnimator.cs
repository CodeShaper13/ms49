using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class WorkerAnimator : MonoBehaviour {

    private static readonly string[] STATIC_DIRECTIONS = {
        "IdleDown",
        "IdleSideways",
        "IdleUp",
        "IdleSideways"
    };

    private static readonly string[] RUN_DIRECTIONS = {
        "WalkDown",
        "WalkSideways",
        "WalkUp",
        "WalkSideways"
    };

    private Rotation _rot = Rotation.UP;
    private SpriteRenderer sr;
    private Animator animator;
    private Vector2 posLastframe;
    private bool playingCustom;

    /// <summary>
    /// Controlls the direction the Worker is facing.  If set to
    /// null, the value is not changed.
    /// </summary>
    public Rotation rotation {
        get {
            return this._rot;
        }
        set {
            if(value != null) {
                this._rot = value;
            }
        }
    }

    private void Awake() {
        this.sr = this.GetComponent<SpriteRenderer>();
        this.animator = this.GetComponent<Animator>();
    }

    private void Start() {
        this.posLastframe = this.transform.position;
    }

    private void Update() {
        if(!Pause.isPaused()) {
            Vector2 currentPos = this.transform.position;
            Vector2 direction = currentPos - this.posLastframe;

            if(this.posLastframe != currentPos || !this.playingCustom) {
                // Moved
                this.playingCustom = false;

                string[] directionArray = direction.magnitude == 0 ?
                    STATIC_DIRECTIONS :
                    RUN_DIRECTIONS;

                // Flip the sprite if the Worker is facing right.
                this.sr.flipX = this.rotation == Rotation.RIGHT;

                // Tell the animator to play the requested state
                this.animator.Play(directionArray[this.rotation.id]);
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
}