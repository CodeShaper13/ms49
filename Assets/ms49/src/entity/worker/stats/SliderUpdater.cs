using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderUpdater : MonoBehaviour {

    [SerializeField]
    private UnlockableStat stat = null;
    //[SerializeField]
    //private Color criticalColor;

    private Slider slider;

    private void Awake() {
        this.slider = this.GetComponent<Slider>();
    }

    private void Start() {
        if(this.valid()) {
            this.slider.minValue = this.stat.minValue;
            this.slider.maxValue = this.stat.maxValue;
        }
    }

    private void LateUpdate() {
        if(this.valid()) {
            this.slider.value = this.stat.value;
        }
    }

    private bool valid() {
        return this.stat != null;
    }
}