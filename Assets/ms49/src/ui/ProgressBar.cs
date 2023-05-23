using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ProgressBar : MonoBehaviour {

    public float minValue = 0;
    public float maxValue = 1;
    public bool wholeNumbers = true;

    [Space]

    [SerializeField]
    public float _value = 0.5f;

    [Space]

    public TMP_Text text = null;
    [ShowIf(nameof(HasTextComponent))]
    public string format = "{0}/{2}";

    private Image image;

    public float Value {
        get => this._value;
        set {
            if(this.wholeNumbers) {
                this._value = Mathf.Round(value);
            } else {
                this._value = value;
            }
        }
    }

    private void OnValidate() {
        if(this.minValue > this.maxValue) {
            this.minValue = this.maxValue;
        }

        if(this.maxValue < this.minValue) {
            this.maxValue = this.minValue;
        }

        this.Value = Mathf.Clamp(this.Value, this.minValue, this.maxValue);
    }

    private void LateUpdate() {
       // if(this.image == null) {
            this.image = this.GetComponentInChildren<Mask>().GetComponent<Image>();
       // }

        this.image.type = Image.Type.Filled;
        this.image.fillAmount = (this.Value - this.minValue) / (this.maxValue - this.minValue);

        if(this.text != null) {
            this.text.text = string.Format(
                this.format,
                this.Value,
                this.minValue,
                this.maxValue);
        }
    }

    private bool HasTextComponent => this.text != null;
}