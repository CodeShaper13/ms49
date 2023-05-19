using UnityEngine;
using UnityEngine.UI;

public class PopupRailStop : PopupWindow {

    [SerializeField]
    private Toggle[] toggles = null;

    private CellBehaviorRailStop railStop;

    protected override void onUpdate() {
        base.onUpdate();

        if(this.railStop == null) {
            this.Close();
        }
    }

    public void SetRailStop(CellBehaviorRailStop railStop) {
        this.railStop = railStop;

        int index = Mathf.Clamp(
            this.railStop.mode.id,
            0,
            this.toggles.Length - 1);

        this.toggles[index].SetIsOnWithoutNotify(true);
    }

    public void Callback_ToggleClick(int id) {
        if(this.railStop != null) {
            this.railStop.mode = CellBehaviorRailStop.GetModeFromId(id);
        }
    }    
}