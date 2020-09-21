using UnityEngine.Tilemaps;

/// <summary>
/// Behaviors should implement this is they wish to provide different
/// tiles to render with depending on the behavior's state.
/// </summary>
public interface IRenderTileOverride {

    void replaceTiles(ref TileBase floorOverlay, ref TileBase tile, ref TileBase tileOverlay);
}
