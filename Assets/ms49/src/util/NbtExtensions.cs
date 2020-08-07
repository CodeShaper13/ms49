using fNbt;
using System;
using UnityEngine;

public static class NbtExtension {

    #region setters:

    public static void setTag(this NbtCompound tag, string name, bool value) {
        tag.setTag(name, value ? (byte)1 : (byte)0);
    }

    public static void setTag(this NbtCompound tag, string name, byte value) {
        tag.Add(new NbtByte(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, byte[] value) {
        tag.Add(new NbtByteArray(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, NbtCompound value) {
        value.Name = name;
        tag.Add(value);
    }

    public static void setTag(this NbtCompound tag, string name, double value) {
        tag.Add(new NbtDouble(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, float value) {
        tag.Add(new NbtFloat(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, int value) {
        tag.Add(new NbtInt(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, int[] value) {
        tag.Add(new NbtIntArray(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, NbtList value) {
        value.Name = name;
        tag.Add(value);
    }

    public static void setTag(this NbtCompound tag, string name, long value) {
        tag.Add(new NbtLong(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, short value) {
        tag.Add(new NbtShort(name, value));
    }

    public static void setTag(this NbtCompound tag, string name, string value) {
        tag.Add(new NbtString(name, value == null ? string.Empty : value));
    }

    public static void setTag(this NbtCompound tag, string name, Guid value) {
        tag.Add(new NbtString(name, value.ToString()));
    }

    /// <summary>
    /// Writes the passed Vector2 to the passed tag.
    /// </summary>
    public static void setTag(this NbtCompound tag, string name, Vector2 vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.setTag("x", vector.x);
        compound.setTag("y", vector.y);
        tag.Add(compound);
    }

    /// <summary>
    /// Writes the passed Vector2Int to the passed tag.
    /// </summary>
    public static void setTag(this NbtCompound tag, string name, Vector2Int vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.setTag("x", vector.x);
        compound.setTag("y", vector.y);
        tag.Add(compound);
    }

    /// <summary>
    /// Writes the passed Vector3 to the passed tag.
    /// </summary>
    public static void setTag(this NbtCompound tag, string name, Vector3 vector) {
        NbtCompound compound = new NbtCompound(name);
        compound.setTag("x", vector.x);
        compound.setTag("y", vector.y);
        compound.setTag("z", vector.z);
        tag.Add(compound);
    }

    /// <summary>
    /// Writes the passed Quaternion to the passed tag.
    /// </summary>
    public static void setTag(this NbtCompound tag, string name, Quaternion quaternion) {
        NbtCompound compound = new NbtCompound(name);
        compound.setTag("w", quaternion.w);
        compound.setTag("x", quaternion.x);
        compound.setTag("y", quaternion.y);
        compound.setTag("z", quaternion.z);
        tag.Add(compound);
    }

    #endregion

    #region getters:

    public static bool getBool(this NbtCompound tag, string name, bool defaultValue = false) {
        return tag.getByte(name, defaultValue ? (byte)1 : (byte)0) == 1;
    }

    public static int getByte(this NbtCompound tag, string name, byte defaultValue = 0) {
        NbtByte tag1 = tag.Get<NbtByte>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static byte[] getByteArray(this NbtCompound tag, string name) {
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

    public static double getDouble(this NbtCompound tag, string name, double defaultValue = 0) {
        NbtDouble tag1 = tag.Get<NbtDouble>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static float getFloat(this NbtCompound tag, string name, float defaultValue = 0) {
        NbtFloat tag1 = tag.Get<NbtFloat>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static int getInt(this NbtCompound tag, string name, int defaultValue = 0) {
        NbtInt tag1 = tag.Get<NbtInt>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static int[] getIntArray(this NbtCompound tag, string name) {
        NbtIntArray tag1 = tag.Get<NbtIntArray>(name);
        if(tag1 == null) {
            return new int[0];
        }
        else {
            return tag1.Value;
        }
    }

    public static NbtList getList(this NbtCompound tag, string name) {
        NbtList tag1 = tag.Get<NbtList>(name);
        if(tag1 == null) {
            return new NbtList();
        }
        else {
            return tag1;
        }
    }

    public static long getLong(this NbtCompound tag, string name, long defaultValue = 0) {
        NbtLong tag1 = tag.Get<NbtLong>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static int getShort(this NbtCompound tag, string name, short defaultValue = 0) {
        NbtShort tag1 = tag.Get<NbtShort>(name);
        if(tag1 == null) {
            return defaultValue;
        }
        else {
            return tag1.Value;
        }
    }

    public static string getString(this NbtCompound tag, string name, string defaultValue = "") {
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
    public static Guid getGuid(this NbtCompound tag, string name, Guid? defaultValue = null) {
        NbtString tag1 = tag.Get<NbtString>(name);
        if(tag1 == null) {
            return defaultValue == null ? Guid.Empty : (Guid)defaultValue;
        }
        else {
            return new Guid(tag1.Value);
        }
    }

    public static Vector2 getVector2(this NbtCompound tag, string name, Vector2? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector2.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector2(
            compound.getFloat("x", ((Vector2)defaultValue).x),
            compound.getFloat("y", ((Vector2)defaultValue).y));
    }

    public static Vector2Int getVector2Int(this NbtCompound tag, string name, Vector2Int? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector2Int.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector2Int(
            compound.getInt("x", ((Vector2Int)defaultValue).x),
            compound.getInt("y", ((Vector2Int)defaultValue).y));
    }

    public static Vector3 getVector3(this NbtCompound tag, string name, Vector3? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Vector3.zero;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Vector3(
            compound.getFloat("x", ((Vector3)defaultValue).x),
            compound.getFloat("y", ((Vector3)defaultValue).y),
            compound.getFloat("z", ((Vector3)defaultValue).z));
    }

    public static Quaternion getQuaternion(this NbtCompound tag, string name, Quaternion? defaultValue = null) {
        if(defaultValue == null) {
            defaultValue = Quaternion.identity;
        }

        NbtCompound compound = tag.getCompound(name);
        return new Quaternion(
            compound.getFloat("w", ((Quaternion)defaultValue).w),
            compound.getFloat("x", ((Quaternion)defaultValue).x),
            compound.getFloat("y", ((Quaternion)defaultValue).y),
            compound.getFloat("z", ((Quaternion)defaultValue).z));
    }

    #endregion

    /*
    /// <summary>
    /// Writes the passed Vector3 as a compound with passed name.
    /// </summary>
    public static NbtCompound writeVector3(string tagName, Vector3 vec) {
        NbtCompound tag = new NbtCompound(tagName);
        tag.Add(new NbtFloat("x", vec.x));
        tag.Add(new NbtFloat("y", vec.y));
        tag.Add(new NbtFloat("z", vec.z));
        return tag;
    }

    /// <summary>
    /// Reads a Vector3 compound from the passed tag.
    /// </summary>
    public static Vector3 readVector3(NbtCompound tag) {
        return new Vector3(
            tag.Get<NbtFloat>("x").FloatValue,
            tag.Get<NbtFloat>("y").FloatValue,
            tag.Get<NbtFloat>("z").FloatValue);
    }

    /// <summary>
    /// Writes the passed Vector3 to the passed tag, appending "x", "y" and "z" to the prefix to get the tag name.
    /// </summary>
    public static NbtCompound writeDirectVector3(NbtCompound tag, Vector3 vec, string prefix) {
        tag.Add(new NbtFloat(prefix + "x", vec.x));
        tag.Add(new NbtFloat(prefix + "y", vec.y));
        tag.Add(new NbtFloat(prefix + "z", vec.z));
        return tag;
    }

    public static Vector3 readDirectVector3(NbtCompound tag, string prefix) {
        return new Vector3(
            tag.Get<NbtFloat>(prefix + "x").FloatValue,
            tag.Get<NbtFloat>(prefix + "y").FloatValue,
            tag.Get<NbtFloat>(prefix + "z").FloatValue);
    }
    */
}