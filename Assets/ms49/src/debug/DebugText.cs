using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour {

    [SerializeField]
    private int lineSpacing = 20;
    [SerializeField]
    private int xSpacing = 10;

    private int textY;
    private Indent indent;
    private List<string> strings;

    private void Awake() {
        this.indent = new Indent();
        this.strings = new List<string>();
    }

    private void OnGUI() {
        if(Main.DEBUG) {
            World world = GameObject.FindObjectOfType<World>();
            if(world != null) {
                this.textY = 0;
                this.indent.reset();


                this.addLine("- - - - - DEBUG: - - - - -");


                // World Info:
                this.addLine("World Info:");
                this.indent.increase();
                this.addLine("Seed: " + world.seed);
                this.addLine("Save Name: " + world.saveName);
                this.addLine("Time: " + world.time.time);


                this.indent.reset();
                this.addLine();


                // Cell under mouse:
                Position pos = CameraController.instance.getMousePos();
                if(!world.isOutOfBounds(pos)) {
                    this.addLine("Selected Cell:");
                    this.indent.increase();
                    CellState state = world.getCellState(pos);
                    this.addLine("Tile: " + state.data.name);
                    this.addLine("Pos: " + pos.ToString());
                    this.addLine("Rotation: " + state.rotation.name);
                    Layer layer = world.storage.getLayer(pos.depth);
                    this.addLine("Temperature: " + layer.getTemperature(pos.x, pos.y));
                    this.addLine("Unmodified temperature: " + layer.getUnmodifiedTemperature(pos.x, pos.y));
                    this.addLine("Heat Source: " + layer.getHeatSource(pos.x, pos.y));
                    this.addLine("Hardness: " + layer.getHardness(pos.x, pos.y));

                    // Cell's Behavior:
                    CellBehavior behavior = world.getBehavior(pos);
                    if(behavior != null) {
                        this.addLine("Behavior:");
                        this.indent.increase();
                        this.strings.Clear();
                        behavior.getDebugText(this.strings);
                        foreach(string s in this.strings) {
                            this.addLine(s);
                        }
                    }
                }


                this.indent.reset();
                this.addLine();


                // Entity under mouse:
                EntityBase e = CameraController.instance.getMouseOver();
                if(e != null) {
                    this.addLine("Entity:");
                    this.indent.increase();
                    this.addLine("Entity ID: " + e.id);
                    this.addLine("Position: " + e.position);
                    this.addLine("Rotation: " + e.rotation.name);
                    this.addLine("GUID: " + e.guid.ToString());
                    this.strings.Clear();
                    e.getDebugText(this.strings);
                    foreach(string s in this.strings) {
                        this.addLine(s);
                    }
                }
            }
        }
    }

    private void addLine(string text = "") {
        GUI.Label(
            new Rect(this.xSpacing, this.textY, 800, 40),
            this.indent.asString() + text);
        this.textY += this.lineSpacing;
    }

    private class Indent {

        private int level;

        public Indent() {
            this.level = 0;
        }

        public void reset() {
            this.level = 0;
        }

        public void increase() {
            this.level++;
        }

        public void decrease() {
            this.level--;
            if(this.level < 0) {
                this.level = 0;
            }
        }

        public string asString() {
            return new string(' ', this.level * 2);
        }
    }
}
