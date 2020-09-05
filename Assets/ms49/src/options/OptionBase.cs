using System;
using UnityEngine;

public abstract class OptionBase<T> : IOption {

    public string name { get; set; }
    public string saveKey { get; private set; }
    public GameObject controlObj { get; set; }
    public T value { get; protected set; }

    private Func<T> _fetchValue;
    private Action<T> _applyOption;


    public OptionBase(string name, string saveKey, Func<T> fetchValue, Action<T> applyOption) {
        this.name = name;
        this.saveKey = saveKey;
        this._fetchValue = fetchValue;
        this._applyOption = applyOption;
    }

    public void setValue(T value) {
        this.value = value;
    }

    public void applyValue() {
        this._applyOption(this.value);
    }

    public virtual void setupControlObj(GameObject obj) {
        this.controlObj = obj;
    }

    public abstract void write();

    public abstract void read();
}
