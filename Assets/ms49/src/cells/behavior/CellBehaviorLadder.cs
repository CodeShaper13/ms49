public class CellBehaviorLadder : CellBehavior {

    public override string getTooltipText() {
        return "[rmb] " + (this.goesUp() ? "go up" : "go down");
    }

    public override void onRightClick() {
        CameraController.instance.changeLayer(CameraController.instance.currentLayer + (this.goesUp() ? -1 : 1));
    }

    private bool goesUp() {
        return this.data.ZMoveDirections == EnumZMoveDirection.Up;
    }
}
