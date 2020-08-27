using UnityEngine;

public class DebugText : MonoBehaviour {

    [SerializeField]
    private int lineSpacing = 20;
    [SerializeField]
    private int xSpacing = 10;

    private int textY;

    private void OnGUI() {
        this.textY = 0;

        if(Main.DEBUG) {
            GUI.contentColor = Color.white;

            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            GUI.Label(new Rect(10, 10, 100, 40), "DEBUG", style);

            World world = GameObject.FindObjectOfType<World>();
            if(world != null) {
                Position pos = CameraController.instance.getMousePos();
                if(!world.isOutOfBounds(pos)) {
                    CellState state = world.getCellState(pos);
                    this.addLine("- - - - - DEBUG: - - - - -");
                    this.addLine("Tile: " + state.data.name);
                    this.addLine("Pos: " + pos.ToString());
                    this.addLine("Rotation: " + state.rotation.name);
                    Layer layer = world.storage.getLayer(pos.depth);
                    this.addLine("Temperature: " + layer.getTemperature(pos.x, pos.y));
                    this.addLine("Unmodified temperature: " + layer.getUnmodifiedTemperature(pos.x, pos.y));
                    this.addLine("Heat Source: " + layer.getHeatSource(pos.x, pos.y));
                }
            }
        }
    }

    private void addLine(string text) {
        GUI.Label(new Rect(this.xSpacing, this.textY, 400, 40), text);
        this.textY += this.lineSpacing;
    }
}
