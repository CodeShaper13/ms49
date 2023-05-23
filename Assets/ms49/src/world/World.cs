using fNbt;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour {

    [SerializeField, Min(16)]
    private int _mapSize = 96;
    [SerializeField]
    private IntVariable _money = null;
    [SerializeField]
    private IntVariable _maxDepth = null;

    [Space]

    public WorldRenderer worldRenderer = null;
    public ParticleList particles = null;
    public EntityList entities = null;
    public TargetedSquares targetedSquares = null;
    public Grid grid = null;
    public HireCandidates hireCandidates = null;
    public GameTime time = null;
    public PlotManager plotManager = null;
    public Payroll payroll = null;
    public Economy economy = null;
    public StatisticManager statManager = null;
    public TechnologyTree technologyTree = null;
    [SerializeField, Required]
    private MapGenerator _mapGenerator = null;

    [Space]

    public CellData rubbleCell;

    public Storage storage { get; private set; }
    public string saveName { get; private set; }
    public int seed { get; private set; }

    public IntVariable Money => this._money;
    public IntVariable MaxDepth => this._maxDepth;
    /// <summary>
    /// The size of the map in cells.
    /// </summary>
    public int MapSize => this._mapSize;
    public MapGenerator MapGenerator => this._mapGenerator;

    private void Awake() {
        this.storage = new Storage(this);
    }

    private void Start() {
        Pathfinder.Initialize(this.MapSize * this.MapSize * this.storage.layerCount);

        this.StartCoroutine(this.UpdateHeat());
    }

    /// <summary>
    /// Initializes the World and reads it from NBT.
    /// </summary>
    public void Initialize(string saveName, NbtCompound rootTag) {
        this.saveName = saveName;

        this.ReadFromNbt(rootTag);

        // In the event of addition Layers being added in development,
        // there will be more layers in the MapGenerationData than
        // layers read.  Generate the new ones.
        for(int i = 0; i < this.storage.layerCount; i++) {
            if(this.storage.GetLayer(i) == null) {
                this._mapGenerator.generateLayer(this, i);
            }
        }

        // Read Player:
        if(CameraController.instance != null) {
            CameraController.instance.readFromNbt(rootTag.getCompound("world").getCompound("player"));
        }
    }

    /// <summary>
    /// Initialized a new World.
    /// </summary>
    public void Initialize(string saveName, NewWorldSettings settings) {
        this.saveName = saveName;

        this.seed = settings.getSeed();

        foreach(IFirstTimeInitializer initializer in this.GetComponentsInChildren<IFirstTimeInitializer>()) {
            initializer.InitializeFirstTime(this.seed);
        }

        // Generate the map.
        for(int depth = 0; depth < this.storage.layerCount; depth++) {
            this._mapGenerator.generateLayer(this, depth);
        }

        // Unlock the plot that contains the Dumptruck and set the
        // Worker spawn point.
        bool setPoint = false;
        foreach(EntityBase e in this.entities.list) {
            if(e is EntityTruck) {
                this.plotManager.GetPlot(e.position.x, e.position.y).isOwned = true;
                this.storage.workerSpawnPoint = e.position.Add(-1, -1);

                setPoint = true;

                break;
            }
        }
        if(!setPoint) {
            Debug.LogWarning("No Truck could be found when generating a new map, there must always be at least one!");
        }


        // Set starting money.
        this._money.value = this._mapGenerator.StartingMoney;


        // Spawn the starting Workers.
        WorkerFactory factory = Main.instance.workerFactory;
        foreach(WorkerType workerType in this._mapGenerator.StartingWorkers) {
            if(workerType == null) {
                continue;
            }

            int xShift = UnityEngine.Random.Range(-1, 2);
            int yShift = UnityEngine.Random.Range(0, 2);
            EntityWorker worker = factory.spawnWorker(
                this,
                this.storage.workerSpawnPoint.Add(xShift, -yShift),
                factory.generateWorkerInfo(workerType, Main.instance.PersonalityRegistry.GetDefaultPersonality()),
                workerType);
        }

        // Setup the new Player.
        CameraController player = Main.instance.player;
        player.inCreativeMode = settings.creativeEnabled;
        player.changeLayer(this._mapGenerator.PlayerStartLayer);
    }

    public CellState GetCellState(int x, int y, int depth) {
        return this.GetCellState(new Position(x, y, depth));
    }

    public CellState GetCellState(Position pos) {
        if(this.IsOutOfBounds(pos)) {
            Debug.Log("Something requested a CellState that is out of bounds.  It is recomended to use World#isOutOfBounds() first.");
            return null;
        }

        return this.storage.getCellState(pos);
    }

    public void SetCell(int x, int y, int depth, CellData cell, bool updateNeighbors = true) {
        this.SetCell(x, y, depth, cell, Rotation.UP, updateNeighbors);
    }

    public void SetCell(Position pos, CellData cell, bool updateNeighbors = true) {
        this.SetCell(pos, cell, Rotation.UP, updateNeighbors);
    }

    public void SetCell(int x, int y, int depth, CellData tile, Rotation rotation, bool updateNeighbors = true) {
        this.SetCell(new Position(x, y, depth), tile, rotation, updateNeighbors);
    }

    /// <summary>
    /// Sets a Cell in the World.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="data">If null, air will be placed.</param>
    /// <param name="rotation">If null, Rotation#UP will be used.</param>
    /// <param name="updateNeighbors"></param>
    public void SetCell(Position pos, CellData data, Rotation rotation, bool updateNeighbors = true) {
        if(this.IsOutOfBounds(pos)) {
            return;
        }

        this.storage.GetLayer(pos.depth).SetCell(
            pos.x,
            pos.y,
            data == null ? Main.instance.CellRegistry.GetAir() : data,
            rotation == null ? Rotation.UP.id : rotation.id,
            updateNeighbors);
    }

    public void PlaceFog(Position pos) {
        if(this.IsOutOfBounds(pos)) {
            return;
        }

        Layer layer = this.storage.GetLayer(pos.depth);
        if(layer.HasFog) {
            layer.fog.setFog(pos.x, pos.y, true);
            this.worldRenderer.dirtyFogmap(pos, true);
        }
    }

    /// <summary>
    /// Checks if the passed position has fog over it.  If the position
    /// is out of bounds, true is returned.
    /// </summary>
    public bool IsCoveredByFog(Position pos) {
        if(this.IsOutOfBounds(pos)) {
            return true;
        }

        return this.storage.GetLayer(pos.depth).fog.isFogPresent(pos.x, pos.y);
    }

    public void LiftFog(Position pos, bool floodLiftFog = true) {
        if(this.IsOutOfBounds(pos)) {
            return;
        }

        if(!this.IsCoveredByFog(pos)) {
            return; // There is no fog here already, don't do anything
        }

        Layer layer = this.storage.GetLayer(pos.depth);
        if(layer.fog != null) {
            if(floodLiftFog) {
                // Lift fog in the event of this cell being in a cave
                Stack<Position> pixels = new Stack<Position>();
                List<Position> changedCells = new List<Position>();
                pixels.Push(pos);

                while(pixels.Count > 0) {
                    Position a = pixels.Pop();
                    if(!this.IsOutOfBounds(a)) {
                        if((this.GetCellState(a).data.IncludeInFogFloodLift || a == pos) && layer.fog.isFogPresent(a.x, a.y) && !changedCells.Contains(a)) {
                            changedCells.Add(a);
                            pixels.Push(new Position(a.x - 1, a.y, pos.depth));
                            pixels.Push(new Position(a.x + 1, a.y, pos.depth));
                            pixels.Push(new Position(a.x, a.y - 1, pos.depth));
                            pixels.Push(new Position(a.x, a.y + 1, pos.depth));
                        }
                    }
                }

                changedCells = changedCells.OrderBy(
                    x => Vector2.Distance(pos.AsVec2, x.AsVec2)).ToList();

                this.StartCoroutine(this.FloodLiftFog(changedCells));                
            } else {
                layer.fog.setFog(pos.x, pos.y, false);
                this.targetedSquares.stopTargeting(pos);
            }

            this.worldRenderer.dirtyFogmap(pos, false);
        }
    }

    /// <summary>
    /// Checks if the passed position is "outside".
    /// </summary>
    public bool IsOutside(Position pos) {
        if(this.IsOutOfBounds(pos)) {
            return false;
        }

        return this.storage.IsOutside(pos.x, pos.y);
    }

    /// <summary>
    /// Returns the hardness of the rock at the passed position.  0
    /// is returned if the pos is out of the World.
    public int GetHardness(Position pos) {
        if(this.IsOutOfBounds(pos)) {
            return 0;
        }

        return this.storage.GetLayer(pos.depth).GetHardness(pos.x, pos.y);
    }

    public void tryCollapse(Position pos) {
        return; // TODO make better

        const int radius = 2;

        for(int potentialCollapseX = -radius; potentialCollapseX <= radius; potentialCollapseX++) {
            for(int potentialCollapseY = -radius; potentialCollapseY <= radius; potentialCollapseY++) {
                // For each cell, check if it is supported
                bool supported = false;
                Position potential = pos.Add(potentialCollapseX, potentialCollapseY);

                for(int x1 = -radius; x1 <= radius; x1++) {
                    for(int y1 = -radius; y1 <= radius; y1++) {
                        Position p1 = potential.Add(x1, y1);
                        if(!this.IsOutOfBounds(p1)) {
                            CellState state = this.GetCellState(p1);
                            if(state.data.SupportsCeiling) {
                                supported = true;
                                break;
                            }
                        }
                    }
                }

                if(!supported) {
                    for(int x = -1; x <= 1; x++) {
                        for(int y = -1; y <= 1; y++) {
                            Position p = potential.Add(x, y);
                            if(!this.IsOutOfBounds(p) && ((x == 0 && y == 0) || UnityEngine.Random.Range(0f, 1f) < 0.6f) && this.GetCellState(p).data.IsDestroyable) {
                                this.SetCell(p, this.rubbleCell);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns true if the passed coordinates or depth are outside of the map.
    /// </summary>
    public bool IsOutOfBounds(Position p) {
        int r = this.MapSize;
        return p.x < 0 || p.y < 0 || p.x >= r || p.y >= r || p.depth < 0 || p.depth >= this.storage.layerCount;
    }

    /// <summary>
    /// Returns the CellBehavior at the passed postion.  If there is
    /// no CellBehavior, null is returned.
    /// </summary>
    public CellBehavior GetCellBehavior(Position pos, bool getFromParent) {
        if(this.IsOutOfBounds(pos)) {
            return null;
        }

        CellState state = this.GetCellState(pos);

        if(getFromParent) {
            if(state.HasBehavior) {
                return state.behavior;
            } else if(state.HasParent) {
                Position parentPos = pos + state.parent;
                if(!this.IsOutOfBounds(parentPos)) {
                    return this.GetCellState(parentPos).behavior; // Could be null (shouldn't be), but that's ok.  The caller should be checking for null values.
                } else {
                    return null;
                }
            }
            return null;
        } else {
            return state.behavior;
        }
    }

    public T GetCellBehavior<T>(Position pos, bool getFromParent) where T : CellBehavior {
        CellBehavior behavior = this.GetCellBehavior(pos, getFromParent);
        if(behavior is T tBehavior) {
            return tBehavior;
        } else {
            return null;
        }
    }

    /// <summary>
    /// Returns all CellBehaviors of the specified type.  If a
    /// predicate is passed, only CellBehaviors that match are
    /// included.
    /// </summary>
    public List<T> GetAllBehaviors<T>(Predicate<T> predicate = null) where T : CellBehavior {
        List<T> list = new List<T>();
        this.GetAllBehaviorsNonAlloc(list, predicate);
        return list;
    }

    public void GetAllBehaviorsNonAlloc<T>(List<T> list, Predicate<T> predicate = null) where T : CellBehavior {
        foreach(CellBehavior behavior in this.storage.cachedBehaviors) {
            if(behavior is T tBehavior && (predicate == null || predicate(tBehavior))) {
                list.Add(tBehavior);
            }
        }
    }

    /// <summary>
    /// Checks if the passed Layer is unlocked.
    /// </summary>
    public bool IsDepthUnlocked(int depth) {
        return depth <= this._maxDepth.value || CameraController.instance.inCreativeMode;
    }

    /// <summary>
    /// Converts a position in world-space to cell units.
    /// </summary>
    public Vector2Int WorldToCell(Vector3 worldPos) {
        Vector3Int vec3 = this.grid.LocalToCell(worldPos);
        return new Vector2Int(vec3.x, vec3.y);
    }

    /// <summary>
    /// Converts a Cell position to world-space units.
    /// </summary>
    public Vector3 CellToWorld(int cellX, int cellY) {
        return this.grid.CellToWorld(new Vector3Int(cellX, cellY, 0));
    }

    public Transform CreateWrapper(string name) {
        GameObject obj = new GameObject(name);
        obj.transform.parent = this.transform;
        obj.transform.position = Vector3.zero;
        return obj.transform;
    }

    /// <summary>
    /// Saves the World.  If the World has no save name (in the event
    /// of this being a debug-launched world, nothing happens.
    /// </summary>
    public void SaveGame() {
        if(!string.IsNullOrEmpty(this.saveName)) {
            NbtCompound rootTag = new NbtCompound("root");
            rootTag.Add(this.WriteToNbt());
            NbtFile nbtFile = new NbtFile(rootTag);

            Directory.CreateDirectory("saves/");
            nbtFile.SaveToFile("saves/" + this.saveName + ".nbt", NbtCompression.None);
        }
    }

    public NbtCompound WriteToNbt() {
        NbtCompound tag = new NbtCompound("world");

        // Write general world info:
        tag.setTag("seed", this.seed);

        if(this._money != null) {
            tag.setTag("money", this._money.value);
        }

        // Write level:
        NbtCompound tagStorage = new NbtCompound("storage");
        this.storage.WriteToNbt(tagStorage);
        tag.Add(tagStorage);

        // Write Player:
        if(CameraController.instance != null) {
            tag.setTag("player", CameraController.instance.writeToNbt());
        }

        // Write all child componenets that implement ISaveableState to NBT
        foreach(ISaveableState saveable in this.GetComponentsInChildren<ISaveableState>()) {
            NbtCompound compound = new NbtCompound();
            saveable.WriteToNbt(compound);
            tag.setTag(saveable.saveableTagName, compound);
        }

        return tag;
    }

    public void ReadFromNbt(NbtCompound rootTag) {
        NbtCompound tag = rootTag.getCompound("world");

        // Read general world info:
        this.seed = tag.getInt("seed");

        if(this._money != null) {
            this._money.value = tag.getInt("money");
        }

        foreach(ISaveableState saveable in this.GetComponentsInChildren<ISaveableState>()) {
            saveable.ReadFromNbt(tag.getCompound(saveable.saveableTagName));
        }

        // Read level:
        NbtCompound tagStorage = tag.getCompound("storage");
        this.storage.ReadFromNbt(tagStorage);
    }

    private IEnumerator FloodLiftFog(List<Position> changedCells, float wait = 0.05f) {
        foreach(Position p in changedCells) {
            this.LiftFog(p, false);
            yield return new WaitForSeconds(wait);
        }
    }


    private IEnumerator UpdateHeat() {
        int size = this.MapSize;
        float[] readBuffer = new float[size * size];

        while(true) {
            // Update heat
            for(int i = 0; i < this.storage.layerCount; i++) {
                yield return new WaitForSeconds(0.1f);

                Layer layer = this.storage.GetLayer(i);

                if(this.MapGenerator.GetLayerFromDepth(i).DefaultTemperature == 0) {
                    continue;
                }

                Array.Copy(layer.temperatures, readBuffer, size * size);

                for(int x = 0; x < size; x++) {
                    for(int y = 0; y < size; y++) {
                        float heatSource = layer.GetHeatSource(x, y);
                        float f;
                        if(heatSource == 0) {
                            f = 0.25f * (
                                layer.GetUnmodifiedTemperature(x - 1, y) +
                                layer.GetUnmodifiedTemperature(x + 1, y) +
                                layer.GetUnmodifiedTemperature(x, y - 1) +
                                layer.GetUnmodifiedTemperature(x, y + 1)
                                );
                            f = Mathf.MoveTowards(f, 0, 0.01f);
                        }
                        else {
                            f = heatSource;
                        }

                        readBuffer[size * x + y] = f;
                    }
                }

                Array.Copy(readBuffer, layer.temperatures, size * size);
            }
        }
    }

}