using UnityEngine;
using UnityEngine.UI;

public class PopupFurnace : PopupContainer {

    [SerializeField]
    private Image _imgSmeltProgressBar = null;
    [SerializeField]
    private ProgressBar _progressBarFuel = null;

    public CellBehaviorFurnace furnace { get; set; }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.furnace != null) {
            if(this._imgSmeltProgressBar != null) {
                this._imgSmeltProgressBar.fillAmount = this.furnace.NormalizedSmeltProgress;
            }

            if(this._progressBarFuel != null) {
                this._progressBarFuel.Value = this.furnace.NormalizedSmeltProgress;
            }
        }
    }
}