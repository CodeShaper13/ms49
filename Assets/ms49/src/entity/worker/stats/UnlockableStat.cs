using UnityEngine;

public class UnlockableStat : MonoBehaviour {

    [SerializeField]
    private string statName = "?";
    [SerializeField]
    private float _minValue = 0;
    [SerializeField]
    private float _maxValue = 100;
    [SerializeField]
    private float startingValue = 50;
    [SerializeField]
    private MilestoneData enablingMilestone = null;

    [Space]

    [SerializeField]
    private float _value;

    public float value {
        get {
            return this._value;
        }
        set {
            this._value = this.clamp(value);
        }
    }
    public float minValue => this._minValue;
    public float maxValue => this._maxValue;

    private void OnValidate() {
        if(this._minValue > this._maxValue) {
            this._minValue = this._maxValue;
        }

        this.startingValue = this.clamp(this.startingValue);
    }

    private void Awake() {
        this.value = this.startingValue;
    }

    public void increase(float amount) {
        if(this.isStatEnabled()) {
            this.value = this.clamp(this.value + amount);
        }
    }

    public void decrease(float amount) {
        if(this.isStatEnabled()) {
            this.value = this.clamp(this.value - amount);
        }
    }

    public void set(float value) {
        this.value = this.clamp(value);
    }

    public float get() {
        return this.value;
    }

    public bool isStatEnabled() {
        return this.enablingMilestone == null || this.enablingMilestone.isUnlocked;
    }

    private float clamp(float value) {
        return Mathf.Clamp(
            value,
            this._minValue,
            this._maxValue);
    }
}
