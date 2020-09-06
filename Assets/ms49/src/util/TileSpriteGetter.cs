using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileSpriteGetter {

    /// <summary>
    /// Returns the Sprite the passed Tile uses.
    /// May reutrn null.
    /// </summary>
    public static Sprite retrieveSprite(TileBase tile) {
        if(tile is WeightedRandomTile) {
            return ((WeightedRandomTile)tile).Sprites[0].Sprite;
        }
        else if(tile is AnimatedTile) {
            return ((AnimatedTile)tile).m_AnimatedSprites[0];
        }
        else if(tile is IsometricRuleTile) {
            return ((IsometricRuleTile)tile).m_DefaultSprite;
        }
        else if(tile is RuleTile) {
            return ((RuleTile)tile).m_DefaultSprite;
        }
        else if(tile is TerrainTile) {
            return ((TerrainTile)tile).m_Sprites[14];
        }
        else if(tile is Tile) {
            return ((Tile)tile).sprite;
        }

        return null;
    }
}
