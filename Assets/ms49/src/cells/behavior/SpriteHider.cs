using UnityEngine;

/// <summary>
/// Hides Sprites if the specified behavior's depth is not currently being rendered.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteHider : MonoBehaviour {

    [SerializeField]
    private CellBehavior behavior = null;

    private SpriteRenderer sr;
    private WorldRenderer worldRenderer;

    private void Awake() {
        this.sr = this.GetComponent<SpriteRenderer>();
    }

    private void Start() {
        this.worldRenderer = GameObject.FindObjectOfType<WorldRenderer>();
    }

    private void LateUpdate() {
        if(this.worldRenderer != null) {
            this.sr.enabled = this.behavior.pos.depth == this.worldRenderer.targetDepth;
        }
    }
}
