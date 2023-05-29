public class CellBehaviorLadder : CellBehavior {

    public override string GetTooltipText() {
        return "[rmb] " + (this.goesUp() ? "go up" : "go down");
    }

    public override void OnRightClick() {
        CameraController.instance.changeLayer(CameraController.instance.currentLayer + (this.goesUp() ? -1 : 1));
    }

    private bool goesUp() {
        return this.data.ZMoveDirections == EnumZMoveDirection.Up;
    }
}
