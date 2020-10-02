using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class RotationOverride {

    [SerializeField]
    private bool _enableOverride = true;

    [SerializeField]
    private TileBase _floorOverlayTile = null;
    [SerializeField]
    private TileBase _objectTile = null;
    [SerializeField]
    private TileBase _overlayTile = null;
    [SerializeField]
    private RotationEffect _effect = RotationEffect.NOTHING;

    public TileBase floorOverlay => this._floorOverlayTile;
    public TileBase objectTile => this._objectTile;
    public TileBase overlayTile => this._overlayTile;
    public RotationEffect effect => this._effect;
    public bool isOverrideEnabled => this._enableOverride;
}
