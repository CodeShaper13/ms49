﻿using UnityEngine;

public abstract class BuildableBase : ScriptableObject {

    [SerializeField]
    private string structureName = string.Empty;
    [SerializeField, Min(0)]
    private int _cost = 0;
    [SerializeField, TextArea(3, 10)]
    private string _description = string.Empty;
    [SerializeField, Tooltip("What Milestone is required to unlock this Cell in the build screen.  If null, it is always unlocked.")]
    private MilestoneData _unlockedAt = null;

    public int cost { get { return this._cost; } }
    public string description { get { return this._description; } }
    public MilestoneData unlockedAt { get { return this._unlockedAt; } }

    public virtual string getName() {
        return this.structureName;
    }

    /// <summary>
    /// If true is returned, the buildable is considered "rotatable"
    /// and it's state can be changed with r and shift + r.
    /// </summary>
    public virtual bool isRotatable() {
        return false;
    }

    /// <summary>
    /// If this Buildable is rotatable (BuildableBase#isRotatable
    /// returns true) the rotate tip message uses the returned text.
    /// </summary>
    public virtual string getRotationMsg() {
        return "nul";
    }

    public virtual int getWidth() {
        return 1;
    }

    public virtual int getHeight() {
        return 1;
    }

    public abstract void getPreviewSprites(
        ref Sprite groundSprite,
        ref Sprite objectSprite,
        ref Sprite overlaySprite);

    /// <summary>
    /// Places the structure into the world.  highlight is null if a Structure is placing this Buildable.
    /// </summary>
    public abstract void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation);

    /// <summary>
    /// Returns true if the Structure can go at the passed position.
    /// </summary>
    public abstract bool isValidLocation(World world, Position pos);
}
