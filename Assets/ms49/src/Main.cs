using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Main : MonoBehaviour {

    public const string SAVE_DIR = "saves/";
    public const string SAVE_EXTENSION = ".nbt";

    public static Main instance;

    public static bool DEBUG = false;

    [SerializeField]
    private GameObject _gamePrefab = null;
    [SerializeField]
    private GameObject _uiOverlay = null;
    [SerializeField]
    private TileRegistry _tileRegistry = null;
    [SerializeField]
    private EntityRegistry _entityRegistry = null;
    [SerializeField]
    private MinedItemRegistry _itemRegistry = null;
    [SerializeField]
    private PersonalityRegistry _personalityRegistry = null;
    [SerializeField]
    private WorkerTypeRegistry _workerTypes = null;
    [SerializeField]
    private Transform _popupParent = null;
    [SerializeField]
    private Options _options = null;
    [SerializeField]
    private WorkerFactory _workerFactory = null;
    [SerializeField]
    private DebugLoadSettings _debugLoadSettings = new DebugLoadSettings();

    [Space]

    [SerializeField]
    private PopupWindow titleScreenPopup = null;

    public TileRegistry tileRegistry => this._tileRegistry;
    public EntityRegistry entityRegistry => this._entityRegistry;
    public MinedItemRegistry itemRegistry => this._itemRegistry;
    public PersonalityRegistry personalityRegistry => this._personalityRegistry;
    public Options options => this._options;
    public WorkerFactory workerFactory => this._workerFactory;
    public WorkerTypeRegistry workerTypeRegistry => this._workerTypes;

    /// <summary>
    /// A reference to the World.  Null if the Player is not playing a save.
    /// </summary>
    private GameObject mainGameObj;

    private void Awake() {
        if(Main.instance == null) {
            Main.instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        } else {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    private void Start() {
        if(this._debugLoadSettings.instantLoad) {
            // Load world instantly.
            string name = this._debugLoadSettings.name;
            if(File.Exists(SAVE_DIR + name + SAVE_EXTENSION)) {
                NbtFile nbtFile = new NbtFile();
                nbtFile.LoadFromFile(SAVE_DIR + name + SAVE_EXTENSION);

                this.createWorld(name, nbtFile.RootTag);
            }
            else {
                this.createWorld(
                    this._debugLoadSettings.name,
                    this._debugLoadSettings.settings);
            }
        } else {
            // Normal game startup.
            this.titleScreenPopup.open();
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F3)) {
            Main.DEBUG = !Main.DEBUG;
        }

        if(!this.isPlaying() && PopupWindow.getPopupsOpen() == 0) {
            this.titleScreenPopup.open();
        }
    }

    /// <summary>
    /// Finds a Popup window of the passed type.  If it could not be
    /// found, null is returned and a warning is logged.
    /// </summary>
    public T findPopup<T>() where T : PopupWindow {
        T popup = this._popupParent.GetComponentInChildren<T>(true);
        if(popup == null) {
            Debug.LogWarning("Could not find Popup of type " + typeof(T));
        }
        return popup;
    }

    public List<SaveFile> getAllSaves(bool sortByLastPlayed = false) {
        List<SaveFile> saves = new List<SaveFile>();

        if(!Directory.Exists(SAVE_DIR)) {
            Directory.CreateDirectory(SAVE_DIR);
        }

        foreach(string s in Directory.GetFiles(SAVE_DIR, "*" + SAVE_EXTENSION)) {
            saves.Add(new SaveFile(s));
        }

        if(sortByLastPlayed) {
            saves.Sort((save2, save1) => DateTime.Compare(save1.lastPlayTime, save2.lastPlayTime));
        }

        return saves;
    }

    public void createWorld(string saveName, NewWorldSettings settings) {
        World world = this.instantiateGame();
        world.initialize(saveName, settings);
    }

    public void createWorld(string saveName, NbtCompound rootTag) {
        World world = this.instantiateGame();
        world.initialize(saveName, rootTag);
    }

    public void shutdownWorld() {
        // Destroy everything.
        GameObject.Destroy(this.mainGameObj);

        // Close the overlay
        this._uiOverlay.SetActive(false);

        // Open the title screen
        this.titleScreenPopup.open();
    }

    /// <summary>
    /// Returns true if there is a map loaded.
    /// </summary>
    public bool isPlaying() {
        return this.mainGameObj != null;
    }

    private World instantiateGame() {
        GameObject obj = GameObject.Instantiate(this._gamePrefab);
        this.mainGameObj = obj;

        this._uiOverlay.SetActive(true);

        return obj.GetComponentInChildren<World>();
    }

    [Serializable]
    private class DebugLoadSettings {
        public bool instantLoad = false;
        [Tooltip("If blank, the game is not saved.")]
        public string name = "temp";
        public NewWorldSettings settings = null;
    }
}
