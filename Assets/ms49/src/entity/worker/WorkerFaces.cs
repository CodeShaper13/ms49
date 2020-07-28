using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WorkerFaces : MonoBehaviour {

    public Sprite normalFace;
    public Sprite deadFace;
    public Sprite sleepingFace;
    public Sprite backOfHead;
    public Sprite eating1;
    public Sprite eating2;

    private SpriteRenderer sr;
    private EnumFace currentFace;

    private void Awake() {
        this.sr = this.GetComponent<SpriteRenderer>();

        this.setFace(EnumFace.ALIVE);
    }

    public Sprite getFace() {
        return this.sr.sprite;
    }

    public void setFace(EnumFace face) {
        if(this.currentFace == face) {
            return;
        }

        switch(face) {
            case EnumFace.ALIVE:
                sr.sprite = this.normalFace;
                break;
            case EnumFace.DEAD:
                sr.sprite = this.deadFace;
                break;
            case EnumFace.SLEEPING:
                sr.sprite = this.sleepingFace;
                break;
            case EnumFace.HEAD_BACK:
                sr.sprite = this.backOfHead;
                break;
            case EnumFace.EATING_1:
                sr.sprite = this.eating1;
                break;
            case EnumFace.EATING_2:
                sr.sprite = this.eating2;
                break;
        }

        this.currentFace = face;
    }

    public enum EnumFace {
        ALIVE = 0,
        DEAD = 1,
        SLEEPING = 2,
        HEAD_BACK = 3,
        EATING_1 = 4,
        EATING_2 = 5,
    }
}
