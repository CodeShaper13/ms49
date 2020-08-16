using UnityEngine;

[CreateAssetMenu(fileName = "Sprites", menuName = "MS49/AnimationSprites", order = 1)]
public class SpriteFrames : ScriptableObject {

    public Sprite idleUp;
    public Sprite idleSideways;
    public Sprite idleDown;

    public Sprite walkUp1;
    public Sprite walkUp2;
    public Sprite walkSideways1;
    public Sprite walkSideways2;
    public Sprite walkDown1;
    public Sprite walkDown2;

    public Sprite getFrame(Frame frame) {
        switch (frame) {
            case Frame.IDLE_UP:
                return this.idleUp;
            case Frame.IDLE_SIDEWAYS:
                return this.idleSideways;
            case Frame.IDLE_DOWN:
                return this.idleDown;
            case Frame.WALK_UP_1:
                return this.walkUp1;
            case Frame.WALK_UP_2:
                return this.walkUp2;
            case Frame.WALK_SIDEWAYS_1:
                return this.walkSideways1;
            case Frame.WALK_SIDEWAYS_2:
                return this.walkSideways2;
            case Frame.WALK_DOWN_1:
                return this.walkDown1;
            case Frame.WALK_DOWN_2:
                return this.walkDown2;
        }

        return null;
    }

    public enum Frame {
        IDLE_UP,
        IDLE_SIDEWAYS,
        IDLE_DOWN,
        WALK_UP_1,
        WALK_UP_2,
        WALK_SIDEWAYS_1,
        WALK_SIDEWAYS_2,
        WALK_DOWN_1,
        WALK_DOWN_2,
    }
}
