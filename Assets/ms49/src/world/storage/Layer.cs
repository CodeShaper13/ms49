using fNbt;
using UnityEngine;

public class Layer {

    public bool navGridDirty;

    private CellState[] tiles;
    public float[] temperatures;
    private float[] heatSources;
    private World world;
    private WorldRenderer worldRenderer;

    public int depth { get; private set; }
    public Fog fog { get; private set; }
    public int size; // { get { return this.world.storage.mapSize; } }

    public Layer(World world, int depth) {
        this.world = world;
        this.depth = depth;
        this.size = this.world.storage.mapSize;

        this.worldRenderer = GameObject.FindObjectOfType<WorldRenderer>();

        this.tiles = new CellState[this.size * this.size];
        for(int i = 0; i < this.tiles.Length; i++) {
            this.tiles[i] = new CellState(Main.instance.tileRegistry.getAir(), null, Rotation.UP);
        }

        this.temperatures = new float[this.size * this.size];
        this.heatSources = new float[this.size * this.size];

        // Setup Fog.
        if(this.world.mapGenData.getLayerFromDepth(this.depth).hasFog) {
            this.fog = new Fog(this.world.storage.mapSize);
            this.fog.setAll(true);
        }
    }

    public void setCell(int x, int y, CellData data, Rotation rotation, bool alertNeighbors = true, bool callBehaviorCreateCallback = true) {
        CellState oldState = this.getCellState(x, y);

        // Cleanup the old meta object if it exists.
        if(oldState.behavior != null) {
            oldState.behavior.onDestroy();
            if(oldState.behavior.cache) {
                this.world.storage.cachedBehaviors.Remove(oldState.behavior);
            }
            GameObject.Destroy(oldState.behavior.gameObject);
        }

        CellBehavior behavior;
        if(data.behaviorPrefab != null) {
            behavior = GameObject.Instantiate(data.behaviorPrefab).GetComponent<CellBehavior>();
            if(behavior == null) {
                Debug.LogWarning("Cell " + data.name + "had a meta object assigned but it did not have a CellMeta componenet on it's root.");
            } else {
                behavior.transform.position = this.world.cellToWorld(x, y);
                if(behavior.cache) {
                    this.world.storage.cachedBehaviors.Add(behavior);
                }
            }
        } else {
            behavior = null;
        }

        CellState state = new CellState(data, behavior, rotation);
        this.tiles[this.world.storage.mapSize * x + y] = state;

        if(callBehaviorCreateCallback && behavior != null) {
            behavior.onCreate(this.world, state, new Position(x, y, this.depth));
        }

        // Update heat soruce map.
        this.setTemperature(x, y, data.temperatureOutput);
        this.heatSources[this.size * x + y] = data.temperatureOutput;

        // Alert neighbors of the change.
        if(alertNeighbors) {
            int mapSize = this.world.storage.mapSize;
            foreach(Rotation r in Rotation.ALL) {
                int x1 = x + r.vector.x;
                int y1 = y + r.vector.y;
                if(x1 >= 0 && y1 >= 0 && x1 < mapSize && y1 < mapSize) {
                    CellBehavior b = this.getCellState(x1, y1).behavior;
                    if(b != null) {
                        b.onNeighborChange(state, new Position(x1, this.depth, y1));
                    }
                }
            }
        }

        this.navGridDirty = true;

        this.worldRenderer.dirtyTile(x, y);
    }

    public CellState getCellState(int x, int y) {
        return this.tiles[this.size * x + y];
    }

    public void setTemperature(int x, int y, float temperature) {
        if(!this.inBounds(x, y)) {
            return;
        }

        this.temperatures[this.size * x + y] = temperature;
    }

    public float getTemperature(int x, int y) {
        if(!this.inBounds(x, y)) {
            return 0;
        }

        return this.temperatures[this.size * x + y];
    }

    public float getHeatSource(int x, int y) {
        if(!this.inBounds(x, y)) {
            return 0;
        }

        return this.heatSources[this.size * x + y];
    }

    public bool hasFog() {
        return this.fog != null;
    }

    public void writeToNbt(NbtCompound tag) {
        // Write tiles:
        int[] idArray = new int[this.tiles.Length];
        int[] rotationArray = new int[this.tiles.Length];
        for(int i = 0; i < this.tiles.Length; i++) {
            CellState state = this.tiles[i];
            idArray[i] = Main.instance.tileRegistry.getIdOfElement(state.data);
            if(state.rotation == null) {
                Debug.Log(state.data.name);
            }
            rotationArray[i] = state.rotation.id;
        }
        tag.setTag("tiles", idArray);
        tag.setTag("rotations", rotationArray);


        // Write tile meta:
        NbtList listTileMeta = new NbtList(NbtTagType.Compound);
        for(int x = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                CellBehavior meta = this.getCellState(x, y).behavior;
                if(meta != null && meta is IHasData) {
                    NbtCompound behaviorTag = new NbtCompound();
                    behaviorTag.setTag("xPos", x);
                    behaviorTag.setTag("yPos", y);
                    ((IHasData)meta).writeToNbt(behaviorTag);
                    listTileMeta.Add(behaviorTag);
                }
            }
        }
        tag.setTag("meta", listTileMeta);

        // Write fog:
        if(this.hasFog()) {
            NbtCompound fogTag = new NbtCompound();
            this.fog.writeToNbt(fogTag);
            tag.setTag("fog", fogTag);
        }

        // Write temperature:
        int[] tempArray = new int[this.temperatures.Length];
        for(int i = 0; i < this.temperatures.Length; i++) {
            tempArray[i] = (int)(this.temperatures[i] * 1_000_000f);
        }
        tag.setTag("temperature", tempArray);
    }

    public void readFromNbt(NbtCompound tag) {
        // Read tiles:
        CellData[] tileArray = new CellData[this.size * this.size];

        int[] tileIds = tag.getIntArray("tiles");
        int[] rotations = tag.getIntArray("rotations");

        int index = 0;
        for(int x  = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                int i = this.size * x + y;
                CellData d = Main.instance.tileRegistry.getElement(tileIds[i]);
                this.setCell(
                    x,
                    y,
                    d == null ? Main.instance.tileRegistry.getAir() : d,
                    Rotation.ALL[rotations[i]],
                    false,
                    false);
                index++;
            }
        }

        // Read fog:
        if(this.hasFog()) {
            this.fog.readFromNbt(tag.getCompound("fog"));
        }

        // Read temperatures:
        int[] tempArray = tag.getIntArray("temperature");
        for(int i = 0; i < Mathf.Min(this.temperatures.Length, tempArray.Length); i++) {
            this.temperatures[i] = tempArray[i] / 1_000_000f;
        }
    }

    private bool inBounds(int x, int y) {
        return x >= 0 && x < this.size && y >= 0 && y < this.size;
    }
}
