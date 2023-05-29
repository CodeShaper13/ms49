using TMPro;
using UnityEngine;

public class PopupTruck : PopupWindow {

    [SerializeField]
    private ProgressBar _progressBar;
    [SerializeField]
    private TMP_Text _textEstimatedValue = null;

    private EntityTruck truck;

    protected override void onUpdate() {
        base.onUpdate();

        if(this.truck != null) {
            if(this._progressBar != null) {
                this._progressBar.maxValue = this.truck.Inventory.Size;
                this._progressBar.Value = this.truck.Inventory.GetItemCount();
            }

            if(this._textEstimatedValue != null) {
                Economy economy = this.truck.world.economy;
                int value = 0;

                foreach(Item item in this.truck.Inventory) {
                    value += economy.getItemValue(item);
                }

                this._textEstimatedValue.text = string.Format("Estimate Load Value: ${0}", value);
            }
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