using System;
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

    [SerializeField]
    private EntityWorker worker = null;

    private SpriteRenderer sr;
    private Animator animator;

    private Vector2 posLastframe;
    private int lastDirection;
    private bool playingCustom;

    private void Awake() {
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

                string[] directionArray = null;

                //measure the magnitude of the input.
                if(direction.magnitude == 0) {
                    directionArray = STATIC_DIRECTIONS;
                }
                else {
                    directionArray = RUN_DIRECTIONS;
                    this.lastDirection = this.DirectionToIndex(direction, 4);
                }

                this.sr.flipX = this.lastDirection == 3;

                // Tell the animator to play the requested state
                this.animator.Play(directionArray[this.lastDirection]);
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

    public Rotation getFacing() {
        if(this.lastDirection == 2) {
            return Rotation.UP;
        }
        else if(this.lastDirection == 1) {
            return Rotation.RIGHT;
        }
        else if(this.lastDirection == 0) {
            return Rotation.DOWN;
        }
        else { // Left
            return Rotation.LEFT;
        }
    }

    /// <summary>
    /// Sets what direction the Worker is facing.
    /// </summary>
    public void setFacing(Rotation rot) {
        if(rot == Rotation.UP) {
            this.lastDirection = 2;
        }
        else if(rot == Rotation.RIGHT) {
            this.lastDirection = 1;
        }
        else if(rot == Rotation.DOWN) {
            this.lastDirection = 0;
        }
        else { // Left
            this.lastDirection = 3;
        }
    }

    /*
    public void setSprite(int id) {
        this.sr.sprite = this.func(id);
        print(this.sr.sprite);
    }

    private Sprite func(int i) {
        switch(i) {
            case 0:
                return this.sprites.idleUp;
            case 1:
                return this.sprites.idleSideways;
            case 2:
                return this.sprites.idleDown;
            case 3:
                return this.sprites.walkUp1;
            case 4:
                return this.sprites.walkUp2;
            case 5:
                return this.sprites.walkSideways1;
            case 6:
                return this.sprites.walkSideways2;
            case 7:
                return this.sprites.walkDown1;
            case 8:
                return this.sprites.walkDown2;
        }
        return this.sprites.idleUp;
    }
    */

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    private int DirectionToIndex(Vector2 dir, int sliceCount) {
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        //add the halfslice offset
        angle += halfstep;
        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if(angle < 0) {
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }

    public enum Direction {
        UP,
        SIDEWAYS,
        DOWN,
    }
}
