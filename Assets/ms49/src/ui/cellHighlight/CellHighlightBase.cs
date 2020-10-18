using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CellHighlightBase : MonoBehaviour {

    [SerializeField]
    protected Color validColor = Color.white;
    [SerializeField]
    protected Color invalidColor = Color.white;
    [SerializeField]
    protected Vector2 cellOffset = new Vector3(-0.5f, 0, 0);
    [SerializeField]
    protected SpriteRenderer sr;

    protected World world {
        private set; get;
    }

    protected virtual void Awake() {
        this.world = GameObject.FindObjectOfType<World>();
    }

    private void Update() {
        if(EventSystem.current.IsPointerOverGameObject()) {
            this.setInvisible();
        }
        else {
            CameraController cc = CameraController.instance;
            Position pos = cc.getMousePos();

            // Move the highlight
            this.transform.position = this.world.cellToWorld(pos.x, pos.y) + (Vector3)this.cellOffset;

            bool valid;
            if(world.isOutOfBounds(pos)) {
                valid = false;
            } else {
                valid = this.onUpdate(pos);
            }

            if(valid && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                this.onClick(pos, valid);
            }
        }
    }

    protected abstract bool onUpdate(Position pos);

    protected abstract void onClick(Position pos, bool isValid);

    public virtual void show() {
        this.gameObject.SetActive(true);
    }

    public virtual void hide() {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called to change the highlight to a valid visual.
    /// </summary>
    public virtual void setValidColor() {
        this.sr.color = this.validColor;
    }

    /// <summary>
    /// Called to change the highlight to a invalid visual.
    /// </summary>
    public virtual void setInvalidColor() {
        this.sr.color = this.invalidColor;
    }

    public virtual void setInvisible() {
        this.sr.color = Color.clear;
    }
}
