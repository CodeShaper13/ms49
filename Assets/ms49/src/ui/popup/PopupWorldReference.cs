using UnityEngine;

public class PopupWorldReference : PopupWindow {

    public World world { get; private set; }

    private void Awake() {
        this.world = GameObject.FindObjectOfType<World>();
    }

    protected virtual void Start() { }

    protected override void onOpen() {
        base.onOpen();

        this.world = GameObject.FindObjectOfType<World>();
        if(this.world == null) {
            Debug.LogWarning("Popup " + this.name + " could not locate the World!");
        }
    }
}
