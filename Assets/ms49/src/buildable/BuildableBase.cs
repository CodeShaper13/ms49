using UnityEngine;

public abstract class BuildableBase : ScriptableObject {

    [SerializeField]
    private string structureName = string.Empty;
    [SerializeField, Min(0)]
    private int cost = 0;
    [SerializeField, TextArea(3, 10)]
    private string description = string.Empty;

    public virtual string getName() {
        return this.structureName;
    }

    public virtual string getDescription() {
        return this.description;
    }

    public virtual int getCost() {
        return this.cost;
    }

    public virtual bool isRotatable() {
        return false;
    }

    public virtual int getWidth() {
        return 1;
    }

    public virtual int getHeight() {
        return 1;
    }

    public abstract void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite);

    /// <summary>
    /// Places the structure into the world.  highlight is null if a structure is placing this buildable.
    /// </summary>
    public abstract void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation);

    /// <summary>
    /// Returns true if the structure can go at the passed position.
    /// </summary>
    public abstract bool isValidLocation(World world, Position pos);
}
