using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PartRenderer : MonoBehaviour {

    [SerializeField]
    private Sprite partFront = null;
    [SerializeField]
    private Sprite partSide = null;
    [SerializeField]
    private Sprite partBack = null;
    [SerializeField, Tooltip("If set, the Worker's rotation will be ignored and this rotation will be used instead.")]
    private EnumRotation rotationOverride = EnumRotation.NONE;

    private EntityWorker worker;
    private SpriteRenderer sr;

    private void OnValidate() {
        if(this.partFront != null) {
            this.GetComponent<SpriteRenderer>().sprite = this.partFront;
        }
    }

    private void Awake() {
        this.sr = this.GetComponent<SpriteRenderer>();
        this.worker = this.GetComponentInParent<EntityWorker>();
    }

    private void LateUpdate() {
        if(!Pause.isPaused()) {
            Rotation r;

            if(this.rotationOverride != EnumRotation.NONE) {
                r = Rotation.fromEnum(this.rotationOverride);
            }
            else {
                r = this.worker.rotation;
            }

            this.sr.flipX = r == Rotation.RIGHT;

            if(r == Rotation.DOWN) {
                this.sr.sprite = this.partFront;
            }
            else if(r == Rotation.RIGHT || r == Rotation.LEFT) {
                this.sr.sprite = this.partSide;
            }
            else if(r == Rotation.UP) {
                this.sr.sprite = this.partBack;
            } else {
                Debug.LogWarning("Worker rotation is null!");
            }

        }
    }
}
