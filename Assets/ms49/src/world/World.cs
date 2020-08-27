using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class World : MonoBehaviour {

    public Storage storage;
    public MapGenerationData mapGenData;
    public WorldRenderer worldRenderer;
    public IntVariable money;
    public ParticleList particles;
    public EntityList entities;
    public MilestoneManager milestones;

    private MapGenerator mapGenerator;
    private Grid grid;
    public int seed { get; private set; }
    public NavigationManager navManager { get; private set; }
    public int stoneExcavated { get; set; }

    private void Awake() {
        this.grid = this.GetComponent<Grid>();

        this.storage = new Storage(this);
        this.mapGenerator = new MapGenerator(this, this.mapGenData);
    }

    private void Start() {
        this.StartCoroutine(this.updateHeat());
    }

    /// <summary>
    /// Initializes the World.  rootTag should be null for new worlds.
    /// </summary>
    public void initialize(NbtCompound rootTag) {
        if(rootTag == null) {
            // New world:

            this.seed = (int)DateTime.Now.Ticks;

            // Generate the first layer.
            for(int i = 0; i < this.storage.layerCount; i++) {
                this.mapGenerator.generateLayer(i);
            }

            this.mapGenerator.generateStartRoom();

            CameraController.instance.initNewPlayer();
            CameraController.instance.changeLayer(this.mapGenData.playerStartLayer);

        }
        else {
            // Load world:
            this.readFromNbt(rootTag);

            // In the event of addition Layers being added in development,
            //there will be more layers in the MapGenerationData than
            //layers read.  Generate the new ones.
            for(int i = 0; i < this.storage.layerCount; i++) {
                if(this.storage.getLayer(i) == null) {
                    this.mapGenerator.generateLayer(i);
                }
            }
        }

        this.navManager = new NavigationManager(this.storage);
    }

    private void Update() {
        if(!Pause.isPaused()) {
            this.navManager.update();
        }
    }

    private IEnumerator updateHeat() {
        int size = this.storage.mapSize;
        float[] readBuffer = new float[size * size];

        while(true) {
            yield return new WaitForSeconds(0.01f);

            // Update heat
            for(int i = 0; i < this.storage.layerCount; i++) {
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

    public void setCell(Position pos, CellData tile, Rotation rotation, bool updateNeighbors = true) {
        if(this.isOutOfBounds(pos)) {
            return;
        }

        this.storage.setCell(pos, tile, rotation == null ? Rotation.UP : rotation, updateNeighbors);
    }

    public bool isTargeted(Position pos) {
        if(this.isOutOfBounds(pos)) {
            return false;
        }

        return this.storage.isTargeted(pos);
    }

    public void setTargeted(Position pos, bool targeted) {
        if(this.isOutOfBounds(pos)) {
            return;
        }

        this.storage.setTargeted(pos, targeted);

        this.worldRenderer.dirtyExcavationTarget(pos, targeted);
    }

    public void liftFog(Position pos, bool floodLiftFog = true) {
        if(this.isOutOfBounds(pos)) {
            return;
        }

        Layer layer = this.storage.getLayer(pos.depth);
        if(layer.fog != null) {
            if(floodLiftFog) {
                // Lift fog in the event of this cell being in a cave
                CellData air = Main.instance.tileRegistry.getAir();
                Stack<Position> pixels = new Stack<Position>();
                List<Position> changedCells = new List<Position>();
                pixels.Push(pos);

                while(pixels.Count > 0) {
                    Position a = pixels.Pop();
                    if(!this.isOutOfBounds(a)) {
                        if((this.getCellState(a).data == air || a == pos) && layer.fog.isFogPresent(a.x, a.y) && !changedCells.Contains(a)) {
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

    private IEnumerator floodLiftFog(List<Position> changedCells) {
        foreach(Position p in changedCells) {
            this.liftFog(p, false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// Returns the targeted square closest to the passed point.
    /// If no points can be found, null is returned.
    /// </summary>
    public Position? getClosestTargeted(Position pos) {
        float dis = float.PositiveInfinity;
        Position? closest = null;

        foreach(Position p in this.storage.targetedForRemovalSquares) {
            float f = p.distance(pos);
            if(f < dis) {
                dis = f;
                closest = p;
            }
        }
        return closest;
    }

    /// <summary>
    /// Returns true if the passed coordinates or depth are outside of the map.
    /// </summary>
    public bool isOutOfBounds(Position p) {
        int r = this.storage.mapSize;
        return p.x < 0 || p.y < 0 || p.x >= r || p.y >= r || p.depth < 0 || p.depth >= this.storage.layerCount;
    }

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

    public List<T> getAllBehaviors<T>() where T : CellBehavior {
        List<T> list = new List<T>();
        foreach(CellBehavior behavior in this.storage.cachedBehaviors) {
            if(behavior is T) {
                list.Add((T)behavior);
            }
        }
        return list;
    }

    public T getClosestBehavior<T>(Position pos, Predicate<T> predicate = null) where T : CellBehavior {
        float dis = float.PositiveInfinity;
        T closest = null;
        foreach(CellBehavior behavior in this.storage.cachedBehaviors) {
            if(behavior is T && (predicate == null || predicate((T)behavior))) {
                float f = pos.distance(behavior.pos);
                if(f < dis) {
                    dis = f;
                    closest = (T)behavior;
                }
            }
        }
        return closest;
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

        // Write Entities:
        if(this.entities != null) {
            this.entities.writeToNbt(tag);
        }

        // Write Player:
        if(CameraController.instance != null) {
            tag.setTag("player", CameraController.instance.writeToNbt());
        }

        // Write Milestones:
        if(this.milestones != null) {
            this.milestones.writeToNbt(tag);
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

        // Read level:
        NbtCompound tagStorage = tag.getCompound("storage");
        this.storage.readFromNbt(tagStorage);

        // Read Entities:
        if(this.entities != null) {
            this.entities.readFromNbt(tag);
        }

        // Read Player:
        if(CameraController.instance != null) {
            CameraController.instance.readFromNbt(tag.getCompound("player"));
        }

        // Read Milestones:
        if(this.milestones != null) {
            this.milestones.readFromNbt(tag);
        }
    }
}