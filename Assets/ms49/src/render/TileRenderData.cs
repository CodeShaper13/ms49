using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRenderData {

    public TileBase floorOverlayTile { get; set; }
    public TileBase objectTile { get; set; }
    public TileBase overlayTile { get; set; }
    public RotationEffect effect { get; set; }

    public TileRenderData(TileBase floorOverlay, TileBase tile, TileBase overlayTile, RotationEffect effect) {
        this.floorOverlayTile = floorOverlay;
        this.objectTile = tile;
        this.overlayTile = overlayTile;
        this.effect = effect;
    }

    /// <summary>
    /// Returns a matrix to Transform the tile with based on the DirectionalTile's settings.
    /// </summary>
    public Matrix4x4 getMatrix() {
        return Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(0f, 0f, getAngle(this.effect)),
            new Vector3(
                this.effect == RotationEffect.MirrorX ? -1 : 1,
                this.effect == RotationEffect.MirrorY ? -1 : 1,
                1));
    }

    /// <summary>
    /// Returns an angle from enum RotationEffect.
    /// </summary>
    private float getAngle(RotationEffect e) {
        switch(e) {
            case RotationEffect.Rotate90:
                return 90f;
            case RotationEffect.Rotate180:
                return 180f;
            case RotationEffect.Rotate270:
                return 270f;
            default:
                return 0f;
        }
    }
}
