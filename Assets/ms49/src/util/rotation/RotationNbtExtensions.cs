using fNbt;
using UnityEngine;

public static class RotationNbtExtensions {

    public static void SetTag(this NbtCompound tag, string name, Rotation value) {
        tag.SetTag(name, (byte)value.id);
    }

    public static Rotation GetRotation(this NbtCompound tag, string name, Rotation defaultValue = null) {
        byte rotationId = defaultValue == null ? (byte)0 : (byte)defaultValue.id;

        return Rotation.ALL[Mathf.Clamp(tag.GetByte(name, rotationId), 0, 3)];
    }
}