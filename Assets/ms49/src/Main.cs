using UnityEngine;

public class Main : MonoBehaviour {

    public static Main instance;

    public static bool DEBUG = false;

    [SerializeField]
    private TileRegistry _tileRegistry;
    [SerializeField]
    public EntityRegistry _entityRegistry;
    [SerializeField]
    public MinedItemRegistry _itemRegistry;
    [SerializeField]
    public Names _names;

    public TileRegistry tileRegistry { get { return this._tileRegistry; } }
    public EntityRegistry entityRegistry { get { return this._entityRegistry; } }
    public MinedItemRegistry itemRegistry { get { return this._itemRegistry; } }
    public Names names { get { return this._names; } }


    private void Awake() {
        if(Main.instance == null) {
            Main.instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        } else {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F3)) {
            Main.DEBUG = !Main.DEBUG;
        }
    }
}
