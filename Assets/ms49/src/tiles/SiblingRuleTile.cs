using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Sibling Rule Tile", menuName = "MS49 Tiles/Sibling Rule Tile")]
public class SiblingRuleTile : RuleTile {

    public List<TileBase> siblings;

    public override bool RuleMatch(int neighbor, TileBase other) {
        switch(neighbor) {
            case TilingRule.Neighbor.This:
                return siblings.Contains(other) || base.RuleMatch(neighbor, other);
            case TilingRule.Neighbor.NotThis:
                return !siblings.Contains(other) && base.RuleMatch(neighbor, other);
        }
        return base.RuleMatch(neighbor, other);
    }
}
