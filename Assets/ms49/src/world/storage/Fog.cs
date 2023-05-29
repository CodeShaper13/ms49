using fNbt;
using UnityEngine;

public class Fog {

    private bool[] fogMap; // True = fog
    private int size;

    public Fog(int s) {
        this.size = s;
        this.fogMap = new bool[this.size * this.size];
    }

    /// <summary>
    /// Returns true if there is fog at the passed tile.
    /// </summary>
    public bool isFogPresent(int x, int y) {
        return this.fogMap[size * x + y];
    }

    public void setFog(int x, int y, bool fog) {
        int i = this.size * x + y;
        if(this.fogMap[i] != fog) {
            this.fogMap[i] = fog;
        }
    }

    public void writeToNbt(NbtCompound tag) {
        byte[] array = new byte[this.fogMap.Length];
        for(int i = 0; i < this.fogMap.Length; i++) {
            array[i] = this.fogMap[i] ? (byte)1 : (byte)0;
        }
        tag.SetTag("fog", array);
    }

    public void readFromNbt(NbtCompound tag) {
        byte[] array = tag.GetByteArray("fog");
        for(int i = 0; i < Mathf.Min(this.fogMap.Length, array.Length); i++) {
            this.fogMap[i] = array[i] == 1;
        }
    }

    public void setAll(bool fog) {
        for(int x = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                this.fogMap[this.size * x + y] = fog;
            }
        }
    }
}
