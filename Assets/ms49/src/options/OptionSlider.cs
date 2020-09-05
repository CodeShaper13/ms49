using UnityEngine;
using System;
using UnityEngine.UI;

public class OptionSlider : OptionBase<float> {

    public float minValue {
        get; private set;
    }
    public float maxValue {
        get; private set;
    }
    public bool wholeNumbersOnly {
        get; private set;
    }

    public OptionSlider(string name, string saveKey, float minValue, float maxValue, bool wholeNumbersOnly, Func<float> get, Action<float> set) :
        base(name, saveKey, get, set) {

        this.minValue = minValue;
        this.maxValue = maxValue;
        this.wholeNumbersOnly = wholeNumbersOnly;
    }

    public override void setupControlObj(GameObject obj) {
        base.setupControlObj(obj);

        Slider slider = obj.GetComponent<Slider>();
        slider.minValue = this.minValue;
        slider.maxValue = this.maxValue;
        slider.wholeNumbers = this.wholeNumbersOnly;
        slider.value = this.value;
        slider.onValueChanged.AddListener((float value) => { this.value = value; });
    }

    public override void read() {
        this.value = PlayerPrefs.GetFloat(this.saveKey);
    }

    public override void write() {
        PlayerPrefs.SetFloat(this.saveKey, this.value);
    }
}
