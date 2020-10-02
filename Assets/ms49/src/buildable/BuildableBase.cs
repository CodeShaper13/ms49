using UnityEngine;

public abstract class BuildableBase : ScriptableObject {

    [SerializeField]
    private string structureName = string.Empty;
    [SerializeField, Min(0)]
    private int _cost = 0;
    [SerializeField, TextArea(3, 10)]
    private string _description = string.Empty;

    public int cost { get { return this._cost; } }
    public string description { get { return this._description; } }

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

    public virtual bool isRotationValid(Rotation rotation) {
        return true;
    }

    /// <summary>
    /// If this Buildable is rotatable (BuildableBase#isRotatable
    /// returns true) the rotate tip message uses the returned text.
    /// </summary>
    public virtual string getRotationMsg() {
        return "nul";
    }

    public virtual int getHighlightWidth() {
        return 1;
    }

    public virtual int getHighlightHeight() {
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
    public abstract bool isValidLocation(World world, Position pos, Rotation rotation);
}
