using UnityEngine;
using fNbt;

public class GameTime : MonoBehaviour {

    [SerializeField]
    private int dayLength = 0;
    [SerializeField]
    private float[] timeMultipliers = new float[] { 1 };

    public double time { get; private set; }

    private int multiplyerIndex;

    public float timeSpeed => this.timeMultipliers[this.multiplyerIndex];

    private void OnValidate() {
        if(this.timeMultipliers == null || this.timeMultipliers.Length == 0) {
            this.timeMultipliers = new float[1];
        }

        this.timeMultipliers[0] = 1f;
    }

    private void Update() {
        if(!Pause.isPaused()) {
            this.time += Time.deltaTime;
        }
    }

    public void increaseTimeSpeed() {
        this.func(1);
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("time", this.time);
        tag.setTag("multiplyerIndex", this.multiplyerIndex);

        return tag;
    }

    public void readFromNbt(NbtCompound tag) {
        this.time = tag.getDouble("time");
        this.multiplyerIndex = Mathf.Clamp(tag.getInt("multiplyerIndex"), 0, this.timeMultipliers.Length - 1);
    }

    private void func(int dir) {
        this.multiplyerIndex += dir;

        if(this.multiplyerIndex < 0) {
            this.multiplyerIndex = this.timeMultipliers.Length - 1;
        } else if(this.multiplyerIndex >= this.timeMultipliers.Length) {
            this.multiplyerIndex = 0;
        }
    }
}
