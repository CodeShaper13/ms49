/// <summary>
/// Cell Behaviors should implement this is they wish to provide
/// different tiles to render with depending on the CellBehavior's
/// state.
/// 
/// CellBehavior#dirty() should be called when a state that affects
/// the Cell look is changed.  This will force the Cell to be
/// redrawn on the Tilemap.
/// </summary>
public interface IRenderTileOverride {

    void replaceTiles(ref TileRenderData renderData);
}
