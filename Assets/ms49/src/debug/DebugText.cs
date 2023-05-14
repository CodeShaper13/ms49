using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DebugText : MonoBehaviour {

    private TMP_Text text;
    private StringBuilder sb;

    private void Awake() {
        this.text = this.GetComponent<TMP_Text>();
        this.sb = new StringBuilder();
    }

    private void LateUpdate() {
        if(Main.DEBUG) {
            this.text.enabled = true;

            this.sb.Clear();

            World world = GameObject.FindObjectOfType<World>();
            if(world != null) {
                this.sb.AppendLine("- - - - - Debug Info: - - - - -");

                // World Info:
                this.sb.AppendLine("World Info:");
                this.sb.AppendLine("  Seed: " + world.seed);
                this.sb.AppendLine("  Save Name: " + world.saveName);
                this.sb.AppendLine("  Time: " + world.time.time.ToString("0.000"));

                this.sb.AppendLine();

                // Cell under mouse:
                Position pos = CameraController.instance.getMousePos();
                if(!world.IsOutOfBounds(pos)) {
                    this.sb.AppendLine("Selected Cell:");
                    CellState state = world.GetCellState(pos);
                    this.sb.AppendLine("  Tile: " + state.data.name);
                    this.sb.AppendLine("  Pos: " + pos.ToString());
                    this.sb.AppendLine("  Meta: " + state.meta + " (Rotation=" + state.Rotation.name + ")");
                    Layer layer = world.storage.GetLayer(pos.depth);
                    this.sb.AppendLine("  Temperature: " + layer.GetTemperature(pos.x, pos.y));
                    this.sb.AppendLine("  Unmodified temperature: " + layer.GetUnmodifiedTemperature(pos.x, pos.y));
                    this.sb.AppendLine("  Heat Source: " + layer.GetHeatSource(pos.x, pos.y));
                    this.sb.AppendLine("  Hardness: " + layer.GetHardness(pos.x, pos.y));

                    // Cell's Behavior:
                    CellBehavior behavior = world.GetCellBehavior(pos, false);
                    if(behavior != null) {
                        this.sb.AppendLine("Behavior:");
                        behavior.getDebugText(this.sb, "  ");
                    }
                }

                this.sb.AppendLine();

                // Entity under mouse:
                EntityBase entity = CameraController.instance.getMouseOver();
                if(entity != null) {
                    this.sb.AppendLine("Entity:");
                    this.sb.AppendLine("  Entity ID: " + entity.id);
                    this.sb.AppendLine("  Position: " + entity.position);
                    this.sb.AppendLine("  Rotation: " + entity.rotation.name);
                    this.sb.AppendLine("  GUID: " + entity.guid.ToString());
                    entity.getDebugText(this.sb, "  ");
                }
            }

            this.text.text = this.sb.ToString();
        } else {
            this.text.enabled = false;
        }
    }
}
