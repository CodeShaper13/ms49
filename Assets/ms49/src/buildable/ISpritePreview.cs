using UnityEngine;

/// <summary>
/// Buildables should implement this if they want to provide a sprite
/// for their preview instead of a Cell.
/// </summary>
public interface ISpritePreview {

    Sprite GetPreviewSprite(World world, Position pos);
}
