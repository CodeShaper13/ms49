using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupTruck : PopupWindow {

    [SerializeField]
    private Image _imgFillProgress;
    [SerializeField]
    private TMP_Text _textContents = null;

    private EntityTruck truck;

    protected override void onUpdate() {
        base.onUpdate();

        if(this.truck != null) {
            int itemCount = this.truck.Inventory.GetItemCount();
            int capacity = this.truck.Inventory.Size;

            if(this._imgFillProgress != null) {
                this._imgFillProgress.fillAmount = itemCount / capacity;
            }

            if(this._textContents != null) {
                this._textContents.text = string.Format("{0}/{1}", itemCount, capacity);
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