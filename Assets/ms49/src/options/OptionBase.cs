using System;
using UnityEngine;

public abstract class OptionBase<T> : IOption {

    public string name { get; set; }
    public string saveKey { get; private set; }
    public GameObject controlObj { get; set; }
    public T value { get; protected set; }

    private Func<T> fetchValue;
    private Action<T> applyOption;
    private string tooltip;

    public OptionBase(string name, string saveKey, string tooltip, Func<T> fetchValue, Action<T> applyOption) {
        this.name = name;
        this.saveKey = saveKey;
        this.tooltip = tooltip;
        this.fetchValue = fetchValue;
        this.applyOption = applyOption;
    }

    public void setValue(T value) {
        this.value = value;
    }

    public void applyValue() {
        this.applyOption(this.value);
    }

    public virtual void setupControlObj(GameObject obj) {
        this.controlObj = obj;

        if(!string.IsNullOrEmpty(this.tooltip)) {
            Tooltip tooltip = obj.GetComponentInChildren<Tooltip>();
            if(tooltip != null) {
                tooltip.text = this.tooltip;
            } else {
                Debug.LogWarning("Could not find Tooltip component on UI Option Object \"" + obj.name + "\"");
            }
        }
    }

    public abstract void write();

    public abstract void read();
}
