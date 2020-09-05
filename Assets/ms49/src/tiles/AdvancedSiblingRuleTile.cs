using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "Tile", menuName = "Tiles/Advanced Sibling Rule Tile (MS49)")]
public class AdvancedSiblingRuleTile : RuleTile<AdvancedSiblingRuleTile.Neighbor> {

    [Tooltip("Siblings that are included in all of the mappings siblings.")]
    public List<TileBase> sharedSiblings = new List<TileBase>();

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        // 0, 1, 2 is using in RuleTile.TilingRule.Neighbor
        public const int mapping3 = 3;
        public const int mapping4 = 4;
        public const int mapping5 = 5;
        public const int mapping6 = 6;
        public const int mapping7 = 7;
        public const int mapping8 = 8;
    }

    public List<SiblingMapping> mappings;

    public override bool RuleMatch(int neighbor, TileBase other) {
        if(neighbor >= 3) {
            // Rule states that is should try and match with a sibbling mapping

            if(this.sharedSiblings.Contains(other)) {
                return true;
            }

            // Find the mapping
            SiblingMapping mapping = null;
            if(this.mappings != null) {
                foreach(SiblingMapping m in this.mappings) {
                    if(m != null && m.rule == neighbor) {
                        mapping = m;
                        break;
                    }
                }
            }

            if(mapping != null) {
                return mapping.siblings.Contains(other);
            }
        }

        return base.RuleMatch(neighbor, other);
    }

    [Serializable]
    public class SiblingMapping {
        [Tooltip("The rule that the following siblings should be used for,")]
        public int rule;
        public List<TileBase> siblings;
    }
}
