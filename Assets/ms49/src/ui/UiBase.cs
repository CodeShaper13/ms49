using UnityEngine;

public class UIBase : MonoBehaviour {

    protected UiManager manager;

    private void Awake() {
        this.manager = this.GetComponentInParent<UiManager>();

        this.onAwake();
    }

    private void Update() {
        this.onUpdate();
    }

    public virtual void onAwake() {
    }

    public virtual void onUpdate() {
    }

    /// <summary>
    /// Called when the UI is shown.
    /// </summary>
    public virtual void onShow() {
    }

    /// <summary>
    /// Called when the UI is closed/hidden.
    /// </summary>
    public virtual void onClose() {
    }

    /// <summary>
    /// Called when the escape key is pressed or the back button is pressed (if it exists).
    /// </summary>
    public virtual void onEscapeOrBack() {
        this.manager.closeCurrent();
    }
}

