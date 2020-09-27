using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[DefaultExecutionOrder(-100)]
public class DirectionalSpriteSwapper : MonoBehaviour {

    [SerializeField]
    public DirectionalSprites sprites;
    [SerializeField, Tooltip("If set, the Worker's rotation will be ignored and this rotation will be used instead.")]
    private EnumRotation rotationOverride = EnumRotation.NONE;

    private SpriteRenderer sr;
    private WorkerAnimator animator;

    private void OnValidate() {
        if(this.sprites != null && this.sprites.front != null) {
            this.GetComponent<SpriteRenderer>().sprite = this.sprites.front;
        }
    }

    private void Awake() {
        this.sr = this.GetComponent<SpriteRenderer>();
        this.animator = this.GetComponentInParent<WorkerAnimator>();
    }

    private void LateUpdate() {
        if(!Pause.isPaused()) {
            Rotation r;

            if(this.rotationOverride != EnumRotation.NONE) {
                r = Rotation.fromEnum(this.rotationOverride);
            }
            else {
                r = this.animator.rotation;
            }

            this.sr.flipX = r == Rotation.RIGHT;

            if(this.sprites != null) {
                if(r == Rotation.DOWN) {
                    this.sr.sprite = this.sprites.front;
                }
                else if(r == Rotation.RIGHT || r == Rotation.LEFT) {
                    this.sr.sprite = this.sprites.side;
                }
                else if(r == Rotation.UP) {
                    this.sr.sprite = this.sprites.back;
                } else {
                    Debug.LogWarning("Worker rotation is null!");
                }
            } else {
                this.sr.sprite = null;
            }
        }
    }
}
