using fNbt;
using System;
using UnityEngine;

public static class NbtExtension {

    #region setters:

    public static void SetTag(this NbtCompound tag, string name, bool value) {
        tag.SetTag(name, value ? (byte)1 : (byte)0);
    }

    public static void SetTag(this NbtCompound tag, string name, byte value) {
        tag.Add(new NbtByte(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, byte[] value) {
        tag.Add(new NbtByteArray(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, NbtCompound value) {
        value.Name = name;
        tag.Add(value);
    }

    public static void SetTag(this NbtCompound tag, string name, double value) {
        tag.Add(new NbtDouble(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, float value) {
        tag.Add(new NbtFloat(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, int value) {
        tag.Add(new NbtInt(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, int[] value) {
        tag.Add(new NbtIntArray(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, NbtList value) {
        value.Name = name;
        tag.Add(value);
    }

    public static void SetTag(this NbtCompound tag, string name, long value) {
        tag.Add(new NbtLong(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, short value) {
        tag.Add(new NbtShort(name, value));
    }

    public static void SetTag(this NbtCompound tag, string name, string value) {
        tag.Add(new NbtString(name, value == null ? string.Empty : value));
    }

    public static void SetTag(this NbtCompound tag, string name, Guid value) {
        tag.Add(new NbtString(name, value.ToString()));
    }

    public static void SetTag(this NbtCompound tag, string name, Vector2 vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.SetTag("x", vector.x);
        compound.SetTag("y", vector.y);
        tag.Add(compound);
    }

    public static void SetTag(this NbtCompound tag, string name, Vector2Int vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.SetTag("x", vector.x);
        compound.SetTag("y", vector.y);
        tag.Add(compound);
    }

    public static void SetTag(this NbtCompound tag, string name, Vector3 vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.SetTag("x", vector.x);
        compound.SetTag("y", vector.y);
        compound.SetTag("z", vector.z);
        tag.Add(compound);
    }

    public static void SetTag(this NbtCompound tag, string name, Vector3Int vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.SetTag("x", vector.x);
        compound.SetTag("y", vector.y);
        compound.SetTag("z", vector.z);
        tag.Add(compound);
    }

    public static void SetTag(this NbtCompound tag, string name, Quaternion quaternion) {
        NbtCompound compound = new NbtCompound(name);
        compound.SetTag("w", quaternion.w);
        compound.SetTag("x", quaternion.x);
        compound.SetTag("y", quaternion.y);
        compound.SetTag("z", quaternion.z);
        tag.Add(compound);
    }

    #endregion

    #region getters:

    public static bool GetBool(this NbtCompound tag, string name, bool defaultValue = false) {
        return tag.GetByte(name, defaultValue ? (byte)1 : (byte)0) == 1;
    }

    public static int GetByte(this NbtCompound tag, string name, byte defaultValue = 0) {
        NbtByte tag1 = tag.Get<NbtByte>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static byte[] GetByteArray(this NbtCompound tag, string name) {
        NbtByteArray tag1 = tag.Get<NbtByteArray>(name);
        if(tag1 == null) {
            return new byte[0];
        }
        else {
            return tag1.Value;
        }
    }

    public static NbtCompound getCompound(this NbtCompound tag, string name) {
        NbtCompound tag1 = tag.Get<NbtCompound>(name);
        if(tag1 == null) {
            return new NbtCompound();
        }
        else {
            return tag1;
        }
    }

    public static double GetDouble(this NbtCompound tag, string name, double defaultValue = 0) {
        NbtDouble tag1 = tag.Get<NbtDouble>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static float GetFloat(this NbtCompound tag, string name, float defaultValue = 0) {
        NbtFloat tag1 = tag.Get<NbtFloat>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static int GetInt(this NbtCompound tag, string name, int defaultValue = 0) {
        NbtInt tag1 = tag.Get<NbtInt>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static int[] GetIntArray(this NbtCompound tag, string name) {
        NbtIntArray tag1 = tag.Get<NbtIntArray>(name);
        if(tag1 == null) {
            return new int[0];
        }
        else {
            return tag1.Value;
        }
    }

    public static NbtList GetList(this NbtCompound tag, string name) {
        NbtList tag1 = tag.Get<NbtList>(name);
        if(tag1 == null) {
            return new NbtList();
        }
        else {
            return tag1;
        }
    }

    public static long GetLong(this NbtCompound tag, string name, long defaultValue = 0) {
        NbtLong tag1 = tag.Get<NbtLong>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static int GetShort(this NbtCompound tag, string name, short defaultValue = 0) {
        NbtShort tag1 = tag.Get<NbtShort>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static string GetString(this NbtCompound tag, string name, string defaultValue = "") {
        NbtString tag1 = tag.Get<NbtString>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    /// <summary>
    /// Returns Guid.Empty if no tag could be found.
    /// </summary>
    public static Guid GetGuid(this NbtCompound tag, string name, Guid? defaultValue = null) {
        NbtString tag1 = tag.Get<NbtString>(name);
        if(tag1 == null) {
            return defaultValue == null ? Guid.Empty : (Guid)defaultValue;
        }
        else {
            return new Guid(tag1.Value);
        }
    }

    public static Vector2 GetVector2(this NbtCompound tag, string name, Vector2? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector2.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector2(
            compound.GetFloat("x", ((Vector2)defaultValue).x),
            compound.GetFloat("y", ((Vector2)defaultValue).y));
    }

    public static Vector2Int GetVector2Int(this NbtCompound tag, string name, Vector2Int? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector2Int.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector2Int(
            compound.GetInt("x", ((Vector2Int)defaultValue).x),
            compound.GetInt("y", ((Vector2Int)defaultValue).y));
    }

    public static Vector3 GetVector3(this NbtCompound tag, string name, Vector3? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector3.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector3(
            compound.GetFloat("x", ((Vector3)defaultValue).x),
            compound.GetFloat("y", ((Vector3)defaultValue).y),
            compound.GetFloat("z", ((Vector3)defaultValue).z));
    }

    public static Vector3Int GetVector3Int(this NbtCompound tag, string name, Vector3Int? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector3Int.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector3Int(
            compound.GetInt("x", ((Vector3Int)defaultValue).x),
            compound.GetInt("y", ((Vector3Int)defaultValue).y),
            compound.GetInt("z", ((Vector3Int)defaultValue).z));
    }

    public static Quaternion GetQuaternion(this NbtCompound tag, string name, Quaternion? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Quaternion.identity;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Quaternion(
            compound.GetFloat("w", ((Quaternion)defaultValue).w),
            compound.GetFloat("x", ((Quaternion)defaultValue).x),
            compound.GetFloat("y", ((Quaternion)defaultValue).y),
            compound.GetFloat("z", ((Quaternion)defaultValue).z));
    }

    #endregion

    public static bool HasKey(this NbtCompound tag, string key) {
        return tag.TryGet(key, out _);
    }
}