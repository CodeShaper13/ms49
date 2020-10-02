using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour {

    // References to Scriptable Objects
    public MapGenerationData mapGenData = null;
    public IntVariable money = null;

    // References to other Components:
    public WorldRenderer worldRenderer = null;
    public ParticleList particles = null;
    public EntityList entities = null;
    public MilestoneManager milestones = null;
    public TargetedSquares targetedSquares = null;
    public Grid grid = null;
    public HireCandidates hireCandidates = null;
    public GameTime time = null;
    public PlotManager plotManager = null;
    public Payroll payroll = null;
    public Economy economy = null;

    // References to Cells
    public CellData rubbleCell;

    // State:
    public Storage storage { get; private set; }
    public string saveName { get; private set; }
    public int seed { get; private set; }
    public int mapSize => this.plotManager.mapSize;
    public int stoneExcavated { get; set; }
    public NavigationManager navManager { get; private set; }
    private MapGenerator mapGenerator;

    /// <summary>
    /// Initializes the World and reads it from NBT.
    /// </summary>
    public void initialize(string saveName, NbtCompound rootTag) {
        this.saveName = saveName;

        this.preInitialization();

        this.readFromNbt(rootTag);

        // In the event of addition Layers being added in development,
        // there will be more layers in the MapGenerationData than
        // layers read.  Generate the new ones.
        for(int i = 0; i < this.storage.layerCount; i++) {
            if(this.storage.getLayer(i) == null) {
                this.mapGenerator.generateLayer(this, i);
            }
        }

        this.postInitialization();

        // Read Player:
        if(CameraController.instance != null) {
            CameraController.instance.readFromNbt(rootTag.getCompound("world").getCompound("player"));
        }
    }

    /// <summary>
    /// Initialized a new World.
    /// </summary>
    public void initialize(string saveName, NewWorldSettings settings) {
        this.saveName = saveName;
        this.seed = settings.getSeed();

        this.preInitialization();

        // Create land plots
        this.plotManager.initializeFirstTime();

        // Generate the map.
        for(int i = 0; i < this.storage.layerCount; i++) {
            this.mapGenerator.generateLayer(this, i);
        }
        this.mapGenerator.generateStartingStructures(this);

        // Set starting money.
        this.money.value = this.mapGenData.startingMoney;

        // Setup the new Player.
        CameraController.instance.initNewPlayer(settings);

        // Spawn the starting Workers.
        WorkerFactory factory = Main.instance.workerFactory;
        foreach(WorkerType workerType in this.mapGenData.startingWorkers) {
            if(workerType != null) {
                int xShift = UnityEngine.Random.Range(-1, 2);
                int yShift = UnityEngine.Random.Range(0, 2);
                EntityWorker worker = factory.spawnWorker(
                    this,
                    this.mapGenData.workerSpawnPoint.add(xShift, -yShift),
                    factory.generateWorkerInfo(), workerType);

                // Modify the starting Worker's pay so new players can't be hosed.
                worker.info.pay = factory.payRange.x + UnityEngine.Random.Range(0, 6);
            }
        }

        this.postInitialization();

        CameraController.instance.changeLayer(this.mapGenData.playerStartLayer);
    }

    private void preInitialization() {
        this.mapGenerator = GameObject.FindObjectOfType<MapGenerator>();
        this.storage = new Storage(this);
    }

    private void postInitialization() {
        this.worldRenderer.startup(this);

        this.navManager = new NavigationManager(this, this.mapSize);

        this.StartCoroutine(this.updateHeat());
    }

    private void Update() {
        if(!Pause.isPaused()) {
            this.navManager.update();
        }
    }

    private IEnumerator updateHeat() {
        int size = this.mapSize;
        float[] readBuffer = new float[size * size];

        while(true) {
            // Update heat
            for(int i = 0; i < this.storage.layerCount; i++) {
                yield return new WaitForSeconds(0.1f);

                Layer layer = this.storage.getLayer(i);

                Array.Copy(layer.temperatures, readBuffer, size * size);

                for(int x = 0; x < size; x++) {
                    for(int y = 0; y < size; y++) {
                        float heatSource = layer.getHeatSource(x, y);
                        float f;
                        if(heatSource == 0) {
                            f = 0.25f * (
                                layer.getUnmodifiedTemperature(x - 1, y) +
                                layer.getUnmodifiedTemperature(x + 1, y) +
                                layer.getUnmodifiedTemperature(x, y - 1) +
                                layer.getUnmodifiedTemperature(x, y + 1)
                                );
                            f = Mathf.MoveTowards(f, 0, 0.01f);
                        } else {
                            f = heatSource;
                        }

                        readBuffer[size * x + y] = f;
                    }
                }

                Array.Copy(readBuffer, layer.temperatures, size * size);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if(this.navManager != null) {
            this.navManager.debugDraw();
        }
    }

    public CellState getCellState(int x, int y, int depth) {
        return this.getCellState(new Position(x, y, depth));
    }

    public CellState getCellState(Position pos) {
        if(this.isOutOfBounds(pos)) {
            return null;
        }

        return this.storage.getCellState(pos);
    }

    public void setCell(int x, int y, int depth, CellData cell, bool updateNeighbors = true) {
        this.setCell(x, y, depth, cell, Rotation.UP, updateNeighbors);
    }

    public void setCell(Position pos, CellData cell, bool updateNeighbors = true) {
        this.setCell(pos, cell, Rotation.UP, updateNeighbors);
    }

    public void setCell(int x, int y, int depth, CellData tile, Rotation rotation, bool updateNeighbors = true) {
        this.setCell(new Position(x, y, depth), tile, rotation, updateNeighbors);
    }

    public void setCell(Position pos, CellData data, Rotation rotation, bool updateNeighbors = true) {
        if(this.isOutOfBounds(pos)) {
            return;
        }

        this.storage.setCell(pos, data, rotation == null ? Rotation.UP : rotation, updateNeighbors);
    }

    public void placeFog(Position pos) {
        if(this.isOutOfBounds(pos)) {
            return;
        }

        Layer layer = this.storage.getLayer(pos.depth);
        if(layer.hasFog()) {
            layer.fog.setFog(pos.x, pos.y, true);
            this.worldRenderer.dirtyFogmap(pos, true);
        }
    }

    public void liftFog(Position pos, bool floodLiftFog = true) {
        if(this.isOutOfBounds(pos)) {
            return;
        }

        Layer layer = this.storage.getLayer(pos.depth);
        if(layer.fog != null) {
            if(floodLiftFog) {
                // Lift fog in the event of this cell being in a cave
                Stack<Position> pixels = new Stack<Position>();
                List<Position> changedCells = new List<Position>();
                pixels.Push(pos);

                while(pixels.Count > 0) {
                    Position a = pixels.Pop();
                    if(!this.isOutOfBounds(a)) {
                        if((this.getCellState(a).data.includeInFogFloodLift || a == pos) && layer.fog.isFogPresent(a.x, a.y) && !changedCells.Contains(a)) {
                            changedCells.Add(a);
                            pixels.Push(new Position(a.x - 1, a.y, pos.depth));
                            pixels.Push(new Position(a.x + 1, a.y, pos.depth));
                            pixels.Push(new Position(a.x, a.y - 1, pos.depth));
                            pixels.Push(new Position(a.x, a.y + 1, pos.depth));
                        }
                    }
                }

                changedCells = changedCells.OrderBy(
                    x => Vector2.Distance(pos.vec2, x.vec2)).ToList();

                this.StartCoroutine(this.floodLiftFog(changedCells));                
            } else {
                layer.fog.setFog(pos.x, pos.y, false);
            }

            this.worldRenderer.dirtyFogmap(pos, false);
        }
    }

    public void tryCollapse(Position pos) {
        return; // TODO make better

        const int radius = 2;

        for(int potentialCollapseX = -radius; potentialCollapseX <= radius; potentialCollapseX++) {
            for(int potentialCollapseY = -radius; potentialCollapseY <= radius; potentialCollapseY++) {
                // For each cell, check if it is supported
                bool supported = false;
                Position potential = pos.add(potentialCollapseX, potentialCollapseY);

                for(int x1 = -radius; x1 <= radius; x1++) {
                    for(int y1 = -radius; y1 <= radius; y1++) {
                        Position p1 = potential.add(x1, y1);
                        if(!this.isOutOfBounds(p1)) {
                            CellState state = this.getCellState(p1);
                            if(state.data.supportsCeiling) {
                                supported = true;
                                break;
                            }
                        }
                    }
                }

                if(!supported) {
                    for(int x = -1; x <= 1; x++) {
                        for(int y = -1; y <= 1; y++) {
                            Position p = potential.add(x, y);
                            if(!this.isOutOfBounds(p) && ((x == 0 && y == 0) || UnityEngine.Random.Range(0f, 1f) < 0.6f) && this.getCellState(p).data.isDestroyable) {
                                this.setCell(p, this.rubbleCell);
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator floodLiftFog(List<Position> changedCells) {
        foreach(Position p in changedCells) {
            this.liftFog(p, false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// Returns true if the passed coordinates or depth are outside of the map.
    /// </summary>
    public bool isOutOfBounds(Position p) {
        int r = this.mapSize;
        return p.x < 0 || p.y < 0 || p.x >= r || p.y >= r || p.depth < 0 || p.depth >= this.storage.layerCount;
    }

    /// <summary>
    /// Returns the CellBehavior at the passed postion.  If there is
    /// no CellBehavior, null is returned.
    /// </summary>
    public CellBehavior getBehavior(Position pos) {
        foreach(CellBehavior behavior in this.storage.cachedBehaviors) {
            if(behavior.pos.Equals(pos)) {
                return behavior;
            }
        }
        return null;
    }

    public T getBehavior<T>(Position pos) where T : CellBehavior {
        CellBehavior behavior = this.getBehavior(pos);
        if(behavior is T) {
            return (T)behavior;
        } else {
            return null;
        }
    }

    /// <summary>
    /// Returns all CellBehaviors of the specified type.  If a
    /// predicate is passed, only CellBehaviors that match are
    /// included.
    /// </summary>
    public List<T> getAllBehaviors<T>(Predicate<T> predicate = null) where T : CellBehavior {
        List<T> list = new List<T>();
        foreach(CellBehavior behavior in this.storage.cachedBehaviors) {
            if(behavior is T && (predicate == null || predicate((T)behavior))) {
                list.Add((T)behavior);
            }
        }
        return list;
    }

    /// <summary>
    /// Checks if the passed Layer is unlocked.
    /// </summary>
    public bool isDepthUnlocked(int depth) {
        if(depth == 0) {
            return true;
        }

        /*
        for(int i = 0; i < this.mapGenData.layerCount; i++) {
            LayerDataBase ld = this.mapGenData.getLayerFromDepth(i);
            if(ld != null && ld.unlockedAtStart) {
                firstLockedIndex++;
            }
        }
        */

        int maxUnlockedIndex = 0;
        foreach(var milestone in this.milestones.milestones) {
            if(milestone.isUnlocked && milestone.unlocksLayer) {
                maxUnlockedIndex++;
            }
        }

        return depth <= maxUnlockedIndex;
    }

    /// <summary>
    /// Converts a position in world units to cell units.
    /// </summary>
    public Vector2Int worldToCell(Vector3 worldPos) {
        Vector3Int vi = this.grid.LocalToCell(worldPos);
        return new Vector2Int(vi.x, vi.y);
    }

    public Vector3 cellToWorld(int cellX, int cellY) {
        return this.grid.CellToWorld(new Vector3Int(cellX, cellY, 0));
    }

    public Transform createHolder(string name) {
        GameObject obj = new GameObject(name);
        obj.transform.parent = this.transform;
        obj.transform.position = Vector3.zero;
        return obj.transform;
    }

    /// <summary>
    /// Saves the World.  If the world has no save name, nothing happens.
    /// </summary>
    public void saveGame() {
        if(!string.IsNullOrEmpty(this.saveName)) {
            NbtCompound rootTag = new NbtCompound("root");
            rootTag.Add(this.writeToNbt());
            NbtFile nbtFile = new NbtFile(rootTag);

            Directory.CreateDirectory("saves/");
            nbtFile.SaveToFile("saves/" + this.saveName + ".nbt", NbtCompression.None);
        }
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound("world");

        // Write general world info:
        tag.setTag("seed", this.seed);
        tag.setTag("stoneExcavated", this.stoneExcavated);

        if(this.money != null) {
            tag.setTag("money", this.money.value);
        }

        // Write level:
        NbtCompound tagStorage = new NbtCompound("storage");
        this.storage.writeToNbt(tagStorage);
        tag.Add(tagStorage);

        // Write Player:
        if(CameraController.instance != null) {
            tag.setTag("player", CameraController.instance.writeToNbt());
        }

        // Write all child componenets that implement ISaveableState to NBT
        foreach(ISaveableState saveable in this.GetComponentsInChildren<ISaveableState>()) {
            NbtCompound compound = new NbtCompound();
            saveable.writeToNbt(compound);
            tag.setTag(saveable.tagName, compound);
        }

        return tag;
    }

    public void readFromNbt(NbtCompound rootTag) {
        NbtCompound tag = rootTag.getCompound("world");

        // Read general world info:
        this.seed = tag.getInt("seed");
        this.stoneExcavated = tag.getInt("stoneExcavated");

        if(this.money != null) {
            this.money.value = tag.getInt("money");
        }

        foreach(ISaveableState saveable in this.GetComponentsInChildren<ISaveableState>()) {
            saveable.readFromNbt(tag.getCompound(saveable.tagName));
        }

        // Read level:
        NbtCompound tagStorage = tag.getCompound("storage");
        this.storage.readFromNbt(tagStorage);
    }
}