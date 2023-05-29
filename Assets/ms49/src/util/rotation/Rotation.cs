using System;
using UnityEngine;

[Serializable]
public class Rotation {

    public static Rotation UP = new Rotation(
        0,
        "up",
        Vector2Int.up,
        EnumAxis.Y,
        EnumRotation.UP);
    public static Rotation RIGHT = new Rotation(
        1,
        "right",
        Vector2Int.right,
        EnumAxis.X,
        EnumRotation.RIGHT);
    public static Rotation DOWN = new Rotation(
        2,
        "down",
        Vector2Int.down,
        EnumAxis.Y,
        EnumRotation.DOWN);
    public static Rotation LEFT = new Rotation(
        3,
        "left",
        Vector2Int.left,
        EnumAxis.X,
        EnumRotation.LEFT);

    public static Rotation[] ALL = new Rotation[] { UP, RIGHT, DOWN, LEFT };
    public static Rotation Random => ALL[UnityEngine.Random.Range(0, 4)];

    public readonly int id;
    public readonly string name;
    public readonly Vector2Int vector;
    public readonly Vector2 vectorF;
    public readonly EnumAxis axis;
    public readonly EnumRotation enumRot;
    public readonly int xDir;
    public readonly int yDir;

    /// <summary>
    /// Converts an EnumRotation to a Rotation.  If the enum is None, null is returned.
    /// </summary>
    public static Rotation fromEnum(EnumRotation rotation) {
        if(rotation == EnumRotation.NONE) {
            return null;
        } else {
            return Rotation.ALL[(int)rotation];
        }
    }

    /// <summary>
    /// Converts a direction vector to a Rotation.
    /// </summary>
    public static Rotation directionToRotation(Vector2 dir) {
        //get the normalized direction
        Vector2 normDir = dir.normalized;

        //calculate how many degrees one slice is
        float step = 360f / 4; // 4 slices

        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        angle *= -1;

        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if(angle < 0) {
            angle += 360;
        }

        int index = Mathf.FloorToInt(angle / step);
        return Rotation.ALL[index];
    }

    private Rotation(int id, string name, Vector2Int dir, EnumAxis axis, EnumRotation enumRot) {
        this.id = id;
        this.name = name;
        this.vector = dir;
        this.vectorF = this.vector;
        this.axis = axis;
        this.enumRot = enumRot;

        this.xDir = dir.x;
        this.yDir = dir.y;
    }

    public override string ToString() {
        return "Rotation(" + this.name + ")";
    }

    public Rotation clockwise() {
        return this.func(1);
    }

    public Rotation counterClockwise() {
        return this.func(-1);
    }

    public Rotation opposite() {
        return this.func(2);
    }

    public Rotation rotate(Rotation cellRotation) {
        return this.func(cellRotation.id);
    }

    public static Position operator *(Rotation rot, int i) {
        return new Position(rot.vector * i, 0);
    }

    private Rotation func(int dir) {
        int i = this.id + dir;
        if(i < 0) {
            i += ALL.Length;
        } else if(i > 3) {
            i -= ALL.Length;
        }

        return ALL[i];
    }
}
