using UnityEngine;
using UnityEngine.UI;

public class PopupRailStop : PopupWindow {

    [SerializeField]
    private Toggle[] toggles = null;

    private CellBehaviorRailStop railStop;
    private CellBehaviorRailStop.Mode stopMode;

    protected override void onOpen() {
        base.onOpen();

        int index = Mathf.Clamp(this.stopMode.id, 0, this.toggles.Length - 1);
        this.toggles[index].SetIsOnWithoutNotify(true);
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.railStop == null) {
            this.Close();
        }
    }

    protected override void onClose() {
        base.onClose();

        if(this.railStop != null) {
            this.railStop.mode = this.stopMode;
        }
    }

    public void SetRailStop(CellBehaviorRailStop railStop) {
        this.railStop = railStop;
        this.stopMode = railStop.mode;
    }

    public void Callback_ToggleClick(int id) {
        this.stopMode = CellBehaviorRailStop.GetModeFromId(id);
    }    
}