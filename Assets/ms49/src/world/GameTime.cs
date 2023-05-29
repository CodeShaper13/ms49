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

    public double Time => this._time;
    public float TimeScale => this._timeMultipliers[this.multiplyerIndex];

    public string saveableTagName => "time";

    private void OnValidate() {
        if(this._timeMultipliers == null || this._timeMultipliers.Length == 0) {
            this._timeMultipliers = new float[1];
        }

        this._timeMultipliers[0] = 1f;
    }

    private void Update() {
        if(!Pause.IsPaused) {
            this._time += UnityEngine.Time.deltaTime;
        }
    }

    public string GetFormattedTimeExact() {
        DateTime timeState = this.GetWorldStartDateTime();
        timeState += this.GetPlayTimeTimespan();
        return timeState.ToString("MM/dd/yyyy H:mm");
    }

    public string GetFormattedTime() {
        TimeSpan timeSpan = this.GetPlayTimeTimespan();

        return string.Format(
            "{0:D2}:{1:D2}",
            timeSpan.Hours,
            timeSpan.Minutes);
    }

    public void increaseTimeSpeed() {
        this.func(1);
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.SetTag("time", this.Time);
        tag.SetTag("multiplyerIndex", this.multiplyerIndex);
    }

    public void ReadFromNbt(NbtCompound tag) {
        this._time = tag.GetDouble("time");
        this.multiplyerIndex = Mathf.Clamp(tag.GetInt("multiplyerIndex"), 0, this._timeMultipliers.Length - 1);
    }

    /// <summary>
    /// Returns the timespan from the start of the world, to the current time.
    /// </summary>
    public TimeSpan GetPlayTimeTimespan() {
        return TimeSpan.FromSeconds(this.Time * this._lengthOfDay);
    }

    /// <summary>
    /// Returns the DateTime of the start of the world.
    /// </summary>
    public DateTime GetWorldStartDateTime() {
        return new DateTime(this._startingYear, 01, 01);
    }

    /// <summary>
    /// Returns the current world date and time.
    /// </summary>
    public DateTime GetWorldTime() {
        return this.GetWorldStartDateTime() + this.GetPlayTimeTimespan();
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
