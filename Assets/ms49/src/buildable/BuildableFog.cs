using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Fog", order = 1)]
public class BuildableFog : BuildableBase {

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        return !world.isCoveredByFog(pos);
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        world.placeFog(pos);
    }
}
