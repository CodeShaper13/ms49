using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextFetchSliderValue : MonoBehaviour {

    [SerializeField]
    private Slider _slider = null;
    [SerializeField]
    private bool _floorValue = false;
    [SerializeField]
    private bool _ceilValue = false;
    [SerializeField]
    private bool _roundValue = false;

    private Text text;

    private void Start() {
        this.text = this.GetComponent<Text>();
    }

    private void Update() {
        if(this._slider != null) {
            float f = this._slider.value;

            if(this._floorValue) {
                f = Mathf.Floor(f);
            }

            if(this._ceilValue) {
                f = Mathf.Ceil(f);
            }

            if(this._roundValue) {
                f = Mathf.Round(f);
            }

            this.text.text = f.ToString();
        }
    }
}
