using fNbt;
using UnityEngine;

public class Fog {

    private bool[] fogMap; // True = fog
    private int size;
    private WorldRenderer renderer;

    public Fog(WorldRenderer renderer, int size) {
        this.size = size;
        this.fogMap = new bool[size * size];
        this.renderer = renderer;
    }

    public void liftFog(Vector2Int currentPos, int revealDistance) {
        if(revealDistance == 0) {
            this.setFog(currentPos.x, currentPos.y, false);
        } else {
            for(int x = -revealDistance; x <= revealDistance; x++) {
                for(int y = -revealDistance; y <= revealDistance; y++) {
                    Vector2Int v = new Vector2Int(currentPos.x + x, currentPos.y + y);
                    int aX = Mathf.Abs(x);
                    if(!(aX == revealDistance && Mathf.Abs(y) == revealDistance)) {
                        this.setFog(v.x, v.y, false);
                    }
                }
            }
        }
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
            this.renderer.redrawFog(x, y, fog);
        }
    }

    public void writeToNbt(NbtCompound tag) {
        byte[] array = new byte[this.fogMap.Length];
        for(int i = 0; i < this.fogMap.Length; i++) {
            array[i] = this.fogMap[i] ? (byte)1 : (byte)0;
        }
        tag.setTag("fog", array);
    }

    public void readFromNbt(NbtCompound tag) {
        byte[] array = tag.getByteArray("fog");
        for(int i = 0; i < Mathf.Min(this.fogMap.Length, array.Length); i++) {
            this.fogMap[i] = array[i] == 1;
        }
    }

    public void setAll(bool fog) {
        for(int x = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                this.setFog(x, y, fog);
            }
        }
    }
}
