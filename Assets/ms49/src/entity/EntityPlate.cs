using UnityEngine;
using static CellBehaviorTable;

public class EntityPlate : EntityBase {

    [SerializeField]
    private SpriteRenderer plateSpriteRenderer = null;
    [SerializeField]
    private Sprite spriteFullPlate = null;
    [SerializeField]
    private Sprite spriteDirtyPlate = null;

    public void updateSprite(EnumPlateState state) {
        Sprite s = null;
        switch(state) {
            case EnumPlateState.NONE:
                s = null;
                break;
            case EnumPlateState.DIRTY:
                s = this.spriteDirtyPlate;
                break;
            case EnumPlateState.FULL:
                s = this.spriteFullPlate;
                break;
        }

        this.plateSpriteRenderer.sprite = s;
    }
}