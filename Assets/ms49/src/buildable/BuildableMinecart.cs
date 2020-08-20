﻿using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Minecart", order = 1)]
public class BuildableMinecart : BuildableBase {

    [SerializeField]
    private Sprite railSprite = null;
    [SerializeField]
    private Sprite minecartSprite = null;
    [SerializeField]
    private Sprite fillSprite = null;

    public override bool isRotatable() {
        return true;
    }

    public override string getRotationMsg() {
        return "change direction with r";
    }

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        groundSprite = this.railSprite;
        objectSprite = this.minecartSprite;
        overlaySprite = this.fillSprite;
    }

    public override bool isValidLocation(World world, Position pos) {
        return !world.isOutOfBounds(pos) && world.getCellState(pos).data is CellDataRail;
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        EntityMinecart minecart = (EntityMinecart)world.entities.spawn(pos, 2); // Minecart id.
        Rotation trackRot = world.getCellState(pos).rotation;
        minecart.facing = rotation.axis == EnumAxis.Y ? trackRot : trackRot.opposite();
    }
}
