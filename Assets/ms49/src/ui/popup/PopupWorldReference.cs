using UnityEngine;

public class PopupWorldReference : PopupWindow {

    public World world { get; private set; }

    protected override void initialize() {
        base.initialize();

        this.world = GameObject.FindObjectOfType<World>();
        if(this.world == null) {
            Debug.LogWarning("Popup " + this.name + " could not locate the World!");
        }
    }
}
