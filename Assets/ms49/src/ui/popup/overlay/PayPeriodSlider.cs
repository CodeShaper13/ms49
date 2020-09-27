using UnityEngine;
using UnityEngine.UI;

public class PayPeriodSlider : MonoBehaviour {

    [SerializeField]
    private Slider sliderPayPeriod = null;

    private World world;

    private void Update() {
        if(!Pause.isPaused()) {

            if(this.world == null) {
                this.world = GameObject.FindObjectOfType<World>();
            }

            if(this.world != null) {
                this.sliderPayPeriod.maxValue =
                    this.world.payroll.payRate;
                this.sliderPayPeriod.value =
                    (float)(this.world.time.time - this.world.payroll.lastPayTime);
            }
        }
    }
}
