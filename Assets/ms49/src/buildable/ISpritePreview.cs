using UnityEngine;

/// <summary>
/// Buildables should implement this if they want to provide a sprite
/// for their preview in the world instead of their cells.
/// </summary>
public interface ISpritePreview {

    Sprite getPreviewSprite(World world, Position pos);
}
