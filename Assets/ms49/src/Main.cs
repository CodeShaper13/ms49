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
    private GameObject _uiOverlay = null;
    [SerializeField]
    private Transform _popupParent = null;
    [SerializeField]
    private WorkerFactory _workerFactory = null;
    [SerializeField]
    private PopupWindow titleScreenPopup = null;


    [Header("Registries")]
    [SerializeField]
    private CellDataRegistry _cellRegistry = null;
    [SerializeField]
    private EntityRegistry _entityRegistry = null;
    [SerializeField]
    private ItemRegistry _itemRegistry = null;
    [SerializeField]
    private PersonalityRegistry _personalityRegistry = null;
    [SerializeField]
    private WorkerTypeRegistry _workerTypes = null;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _worldPrefab = null;
    [SerializeField]
    private GameObject _playerPrefab = null;

    /// <summary>
    /// A reference to the World.  Null if the Player is not playing a save.
    /// </summary>
    public World activeWorld { get; private set; }
    public CameraController player { get; private set; }

    public WorkerFactory workerFactory => this._workerFactory;
    public CellDataRegistry CellRegistry => this._cellRegistry;
    public EntityRegistry EntityRegistry => this._entityRegistry;
    public ItemRegistry ItemRegistry => this._itemRegistry;
    public PersonalityRegistry PersonalityRegistry => this._personalityRegistry;
    public WorkerTypeRegistry WorkerTypeRegistry => this._workerTypes;
    /// <summary>
    /// Returns true if there is a map loaded.
    /// </summary>
    public bool IsPlayingGame() => this.activeWorld != null;

    private void Awake() {
        if(Main.instance == null) {
            Main.instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        } else {
            Debug.LogError("There can only be one Main script at a time!");
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    private void Start() {
        if(!this.IsPlayingGame()) {
            this.titleScreenPopup.open();
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F3)) {
            Main.DEBUG = !Main.DEBUG;
        }

        if(!this.IsPlayingGame() && PopupWindow.getPopupsOpen() == 0) {
            this.titleScreenPopup.open();
        }
    }

    /// <summary>
    /// Finds a Popup window of the passed type.  If it could not be
    /// found, null is returned and a warning is logged.
    /// </summary>
    public T FindPopup<T>() where T : PopupWindow {
        T popup = this._popupParent.GetComponentInChildren<T>(true);
        if(popup == null) {
            Debug.LogWarning("Could not find Popup of type " + typeof(T));
        }
        return popup;
    }

    /// <summary>
    /// Returns all saved games.
    /// </summary>
    /// <param name="sortByLastPlayed"></param>
    /// <returns></returns>
    public List<SaveFile> GetAllSaves(bool sortByLastPlayed = false) {
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

    public void StartWorld(string saveName, NewWorldSettings settings) {
        PopupWindow.closeAll();

        World world = this.InstantiateGameObjects();
        world.Initialize(saveName, settings);
    }

    public void StartWorld(string saveName, NbtCompound rootTag) {
        PopupWindow.closeAll();

        World world = this.InstantiateGameObjects();
        world.Initialize(saveName, rootTag);
    }

    /// <summary>
    /// Closes the world and returns to the title screen.  This will
    /// NOT save the game, it must be done elsewhere.
    /// </summary>
    public void ShutdownWorld() {
        // Destroy everything.
        GameObject.Destroy(this.activeWorld.gameObject);
        GameObject.Destroy(this.player.gameObject);

        // Close the overlay
        this._uiOverlay.SetActive(false);

        this.titleScreenPopup.open();
    }

    private World InstantiateGameObjects() {
        this.activeWorld = GameObject.Instantiate(this._worldPrefab).GetComponent<World>();
        this.player = GameObject.Instantiate(this._playerPrefab).GetComponent<CameraController>();

        this._uiOverlay.SetActive(true);

        return this.activeWorld;
    }
}
