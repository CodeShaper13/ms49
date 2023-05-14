using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Fog", order = 1)]
public class BuildableFog : BuildableBase {

    public override bool IsValidLocation(World world, Position pos, Rotation rotation) {
        return !world.IsCoveredByFog(pos);
    }

    public override void PlaceIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        world.PlaceFog(pos);
    }
}
