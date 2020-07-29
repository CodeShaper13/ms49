using System;
using UnityEngine;

public class Rotation {

    public static Rotation UP = new Rotation(0, "up", Vector2Int.up, EnumAxis.Y);
    public static Rotation RIGHT = new Rotation(1, "right", Vector2Int.right, EnumAxis.X);
    public static Rotation DOWN = new Rotation(2, "down", Vector2Int.down, EnumAxis.Y);
    public static Rotation LEFT = new Rotation(3, "left", Vector2Int.left, EnumAxis.X);

    public static Rotation[] ALL = new Rotation[] { UP, RIGHT, DOWN, LEFT };

    public readonly int id;
    public readonly string name;
    public readonly Vector2Int vector;
    public readonly Vector2 vectorF;
    public readonly EnumAxis axis;

    public static Rotation fromEnum(EnumRotation rotation) {
        return Rotation.ALL[(int)rotation];
    }

    private Rotation(int id, string name, Vector2Int dir, EnumAxis axis) {
        this.id = id;
        this.name = name;
        this.vector = dir;
        this.vectorF = this.vector;
        this.axis = axis;
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
