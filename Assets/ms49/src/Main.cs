using UnityEngine;

public class Main : MonoBehaviour {

    public static Main instance;

    public static bool DEBUG = false;

    public TileRegistry tileRegistry;
    public EntityRegistry entityRegistry;
    public MinedItemRegistry itemRegistry;

    private void Awake() {
        if(Main.instance == null) {
            Main.instance = this;
            //GameObject.DontDestroyOnLoad(this.gameObject);
        } else {
            //GameObject.Destroy(this.gameObject);
            return;
        }

        References.bootstrap();
        Names.bootstrap();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F3)) {
            Main.DEBUG = !Main.DEBUG;
        }
    }

    private void OnGUI() {
        if(Main.DEBUG) {
            GUI.contentColor = Color.white;

            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            GUI.Label(new Rect(10, 10, 100, 40), "DEBUG", style);

            World world = GameObject.FindObjectOfType<World>();
            if(world != null) {
                Vector2Int pos = CameraController.instance.getMousePos();
                CellState state = world.getCellState(pos.x, pos.y, CameraController.instance.currentLayer);
                if(state != null) {
                    GUI.Label(new Rect(10, 50, 400, 40), "Tile: " + state.data.name);
                    GUI.Label(new Rect(10, 70, 400, 40), "Pos: " + pos.ToString());
                    GUI.Label(new Rect(10, 90, 400, 40), "Rotation: " + state.rotation.name);
                }
            }
        }
    }
}
