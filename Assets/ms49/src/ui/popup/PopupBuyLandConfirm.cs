using UnityEngine;
using UnityEngine.UI;

public class PopupBuyLandConfirm : PopupWindow {

    [SerializeField]
    private Text text = null;
    [SerializeField]
    private Button buyButton = null;
    [SerializeField]
    private IntVariable money = null;

    private Plot plot;

    private void Update() {
        this.buyButton.interactable = this.enoughMoney();
    }

    public void setPlot(Plot p) {
        this.plot = p;
        this.text.text = "Buy land for $" + this.plot.cost;
    }

    public void callback_confirm() {
        if(this.enoughMoney()) {
            if(!CameraController.instance.inCreativeMode) {
                this.money.value -= this.plot.cost;
            }
            this.plot.isOwned = true;
        }

        this.close();
    }

    private bool enoughMoney() {
        if(this.plot != null) {
            return CameraController.instance.inCreativeMode || this.money.value >= this.plot.cost;
        } else {
            return false;
        }
    }
}
