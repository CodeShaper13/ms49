using UnityEngine;
using fNbt;
using System;

public class GameTime : MonoBehaviour, ISaveableState {

    [SerializeField, Tooltip("Length of the day in seconds.")]
    private int _lengthOfDay = 60;
    [SerializeField]
    private double _time;
    [SerializeField]
    private float[] _timeMultipliers = new float[] { 1 };
    [SerializeField, Min(0)]
    private int _startingYear = 2000;

    private int multiplyerIndex;

    public double time => this._time;
    public float timeScale => this._timeMultipliers[this.multiplyerIndex];

    public string saveableTagName => "time";

    private void OnValidate() {
        if(this._timeMultipliers == null || this._timeMultipliers.Length == 0) {
            this._timeMultipliers = new float[1];
        }

        this._timeMultipliers[0] = 1f;
    }

    private void Update() {
        if(!Pause.isPaused()) {
            this._time += Time.deltaTime;
        }
    }

    public string getFormattedTimeExact() {
        DateTime timeState = new DateTime(this._startingYear, 01, 01);
        timeState = timeState + this.getTimespan();
        return timeState.ToString("MM/dd/yyyy H:mm");
    }

    public string getFormattedTime() {
        if(Main.DEBUG) {
            return this.time.ToString();
        } else {
            TimeSpan timeSpan = this.getTimespan();

            return string.Format(
                "{0:D2}:{1:D2}",
                timeSpan.Hours,
                timeSpan.Minutes);
        }
    }

    public void increaseTimeSpeed() {
        this.func(1);
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.setTag("time", this.time);
        tag.setTag("multiplyerIndex", this.multiplyerIndex);
    }

    public void ReadFromNbt(NbtCompound tag) {
        this._time = tag.getDouble("time");
        this.multiplyerIndex = Mathf.Clamp(tag.getInt("multiplyerIndex"), 0, this._timeMultipliers.Length - 1);
    }

    private TimeSpan getTimespan() {
        return TimeSpan.FromSeconds(this.time * this._lengthOfDay);
    }

    private void func(int dir) {
        this.multiplyerIndex += dir;

        if(this.multiplyerIndex < 0) {
            this.multiplyerIndex = this._timeMultipliers.Length - 1;
        } else if(this.multiplyerIndex >= this._timeMultipliers.Length) {
            this.multiplyerIndex = 0;
        }
    }
}
