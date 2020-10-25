using UnityEngine;
using UnityEngine.UI;

public class PayPeriodSlider : MonoBehaviour {

    [SerializeField]
    private Slider sliderPayPeriod = null;
    [SerializeField]
    private Tooltip _tooltip = null;

    private World world;

    private void Update() {
        if(!Pause.isPaused()) {

            if(this.world == null) {
                this.world = GameObject.FindObjectOfType<World>();
            }

            if(this.world != null) {
                double d = this.world.time.time - this.world.payroll.lastPayTime;

                this.sliderPayPeriod.maxValue =
                    this.world.payroll.payRate;
                this.sliderPayPeriod.value =
                    (float)d;

                this._tooltip.text = ((int)(this.world.payroll.payRate - d)).ToString() + " seconds";
            }
        }
    }
}
