using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TileBridge : RuleTile {

    [SerializeField]
    public List<TileBase> liquidTiles = new List<TileBase>();

    public override bool RuleMatch(int neighbor, TileBase other) {
        switch(neighbor) {
            case TilingRule.Neighbor.This:
                return !liquidTiles.Contains(other) || base.RuleMatch(neighbor, other);
            case TilingRule.Neighbor.NotThis:
                return liquidTiles.Contains(other) && base.RuleMatch(neighbor, other);
        }
        return base.RuleMatch(neighbor, other);
    }
}
