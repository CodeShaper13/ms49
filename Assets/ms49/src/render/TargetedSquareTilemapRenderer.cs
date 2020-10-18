using UnityEngine;
using UnityEngine.Tilemaps;

public class TargetedSquareTilemapRenderer : MonoBehaviour {

    [SerializeField]
    private Tilemap _tilemap = null;
    [SerializeField]
    private TileBase _tileNormalPriority = null;
    [SerializeField]
    private TileBase _tileHighPriority = null;

    public void clear() {
        this._tilemap.ClearAllTiles();
    }

    public void setTile(int x, int y, TargetedSquare ts) {
        TileBase tile;
        if(ts == null) {
            tile = null;
        } else {
            if(ts.isPriority) {
                tile = this._tileHighPriority;
            }
            else {
                tile = this._tileNormalPriority;
            }
        }

        this._tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }
}
