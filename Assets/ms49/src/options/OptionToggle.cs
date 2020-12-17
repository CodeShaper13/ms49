using UnityEngine;
using System;
using UnityEngine.UI;

public class OptionToggle : OptionBase<bool> {

    public OptionToggle(string name, string saveKey, string tooltip, Func<bool> get, Action<bool> set) :
        base(name, saveKey, tooltip, get, set) {
    }
    
    public override void setupControlObj(GameObject obj) {
        base.setupControlObj(obj);

        Toggle toggle = obj.GetComponentInChildren<Toggle>();
        toggle.isOn = this.value;
        toggle.onValueChanged.AddListener((bool value) => {
            this.value = value;
        });
    }

    public override void read() {
        this.value = PlayerPrefs.GetInt(this.saveKey) == 1;
    }

    public override void write() {
        PlayerPrefs.SetInt(this.saveKey, this.value ? 1 : 0);
    }
}
