using UnityEngine;

public class PopupTruck : PopupWindow {

    [SerializeField]
    private ProgressBar _progressBar;

    private EntityTruck truck;

    protected override void onUpdate() {
        base.onUpdate();

        if(this.truck != null) {
            this._progressBar.maxValue = this.truck.Inventory.Size;
            this._progressBar.Value = this.truck.Inventory.GetItemCount();
        }
    }

    public void SetTruck(EntityTruck truck) {
        this.truck = truck;
    }

    public void Callback_SendOff() {
        if(this.truck != null && !this.truck.isDriving) {
            this.truck.StartDriving();
        }
    }
}