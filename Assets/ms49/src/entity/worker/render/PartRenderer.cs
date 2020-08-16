using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PartRenderer : MonoBehaviour {

    [SerializeField]
    private Sprite partUp = null;
    [SerializeField]
    private Sprite partSideways = null;
    [SerializeField]
    private Sprite partDown = null;
    [SerializeField]
    private WorkerAnimator animator = null;

    private SpriteRenderer sr;

    private void Awake() {
        this.sr = this.GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() {
        if(!Pause.isPaused()) {
            Rotation r = this.animator.getFacing();

            this.sr.flipX = r == Rotation.RIGHT;

            if(r == Rotation.UP) {
                this.sr.sprite = this.partUp;
            }
            else if(r == Rotation.RIGHT || r == Rotation.LEFT) {
                this.sr.sprite = this.partSideways;
            }
            else if(r == Rotation.DOWN) {
                this.sr.sprite = this.partDown;
            }
        }
    }
}
