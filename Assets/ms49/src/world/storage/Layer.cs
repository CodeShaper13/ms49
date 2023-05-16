using fNbt;
using UnityEngine;

public class Layer {

    public bool navGridDirty;

    private CellState[] tiles;
    public float[] temperatures;
    private float[] heatSources;
    private int[] hardness;
    private readonly World world;
    private float cachedLayerHeat = 0;

    public readonly int depth;
    public readonly Fog fog;
    public readonly int size;

    public int Area => this.size * this.size;
    public bool HasFog => this.fog != null;

    public Layer(World world, int depth) {
        this.world = world;
        this.depth = depth;
        this.size = this.world.MapSize;

        this.tiles = new CellState[this.Area];
        CellData air = Main.instance.CellRegistry.GetAir();
        for(int i = 0; i < this.tiles.Length; i++) {
            this.tiles[i] = new CellState(air, null, Rotation.UP);
        }

        this.temperatures = new float[this.Area];
        this.heatSources = new float[this.Area];
        this.hardness = new int[this.Area];

        LayerData layerData = this.world.MapGenerator.GetLayerFromDepth(this.depth);
        if(layerData != null) { // Null if a layer is added through an external editor or the layer count is reduced

            // Set heat sourcess
            this.cachedLayerHeat = layerData.DefaultTemperature;
            if(this.cachedLayerHeat != 0) {
                for(int i = 0; i < this.heatSources.Length; i++) {
                    this.heatSources[i] = this.cachedLayerHeat;
                }
            }

            // Setup Fog.
            if(layerData.HasFog) {
                this.fog = new Fog(this.size);
                this.fog.setAll(true);
            }
        }
    }

    public void SetCell(int x, int y, CellData data, int meta, bool alertNeighbors = true, bool callBehaviorCreateCallback = true) {
        CellState oldState = this.GetCellState(x, y);

        // Cleanup the old CellBehavior if it exists.
        if(oldState.behavior != null) {
            oldState.behavior.onDestroy();
            if(oldState.behavior.cache) {
                this.world.storage.cachedBehaviors.Remove(oldState.behavior);
            }
            GameObject.Destroy(oldState.behavior.gameObject);
        }

        CellBehavior behavior;
        if(data.HasBehaviorPrefab) {
            behavior = GameObject.Instantiate(data.BehaviorPrefab, this.world.storage.behaviorHolder).GetComponent<CellBehavior>();
            if(behavior == null) {
                Debug.LogWarning("Cell " + data.name + "had a behavior object assigned but it did not have a CellBehavior componenet on it's root.");
            } else {
                behavior.transform.position = this.world.CellToWorld(x, y);
                if(behavior.cache) {
                    this.world.storage.cachedBehaviors.Add(behavior);
                }
            }
        } else {
            behavior = null;
        }

        CellState state = new CellState(data, behavior, meta);
        this.tiles[this.world.MapSize * x + y] = state;

        if(callBehaviorCreateCallback && behavior != null) {
            behavior.OnCreate(this.world, state, new Position(x, y, this.depth));
        }

        // Update heat source map.
        this.SetTemperature(x, y, data.TemperatureOutput);
        this.heatSources[this.size * x + y] = data.TemperatureOutput;

        // Alert neighbors of the change.
        if(alertNeighbors) {
            int mapSize = this.world.MapSize;
            foreach(Rotation r in Rotation.ALL) {
                int x1 = x + r.vector.x;
                int y1 = y + r.vector.y;
                if(x1 >= 0 && y1 >= 0 && x1 < mapSize && y1 < mapSize) {
                    CellBehavior b = this.GetCellState(x1, y1).behavior;
                    if(b != null) {
                        b.onNeighborChange(state, new Position(x, y, this.depth));
                    }
                }
            }
        }

        this.navGridDirty = true;

        this.world.worldRenderer.dirtyTile(x, y);
    }

    public CellState GetCellState(int x, int y) {
        return this.tiles[this.size * x + y];
    }

    public void SetTemperature(int x, int y, float temperature) {
        if(!this.InBounds(x, y)) {
            return;
        }

        this.temperatures[this.size * x + y] = temperature;
    }

    /// <summary>
    /// Returns the temperature before it is modified by the Layer's base temperature.  If the position is out of range, 0 is returned.
    /// </summary>
    public float GetUnmodifiedTemperature(int x, int y) {
        if(!this.InBounds(x, y)) {
            return 0;
        }

        return this.temperatures[this.size * x + y];
    }

    /// <summary>
    /// Returns the temperature with the Layer's base temperature modification.  If the position is out of range, 0 is returned.
    /// </summary>
    public float GetTemperature(int x, int y) {
        if(!this.InBounds(x, y)) {
            return 0;
        }

        return this.GetUnmodifiedTemperature(x, y) + this.cachedLayerHeat;
    }

    public float GetHeatSource(int x, int y) {
        if(!this.InBounds(x, y)) {
            return 0;
        }

        return this.heatSources[this.size * x + y];
    }

    public void SetHardness(int x, int y, int hardness) {
        this.hardness[this.size * x + y] = hardness;
    }

    public int GetHardness(int x, int y) {
        return this.hardness[this.size * x + y];
    }

    public void WriteToNbt(NbtCompound tag) {
        // Write tiles:
        int len = this.tiles.Length;
        int[] idArray = new int[len];
        int[] metaArray = new int[len];
        int[] parents = new int[len * 2]; // TODO this is a mostly empty (filled of default values) array.  Optimize this.
        for(int i = 0; i < this.tiles.Length; i++) {
            CellState state = this.tiles[i];
            idArray[i] = Main.instance.CellRegistry.GetIdOfElement(state.data);
            metaArray[i] = state.meta;
            parents[i * 2] = state.parent.x;
            parents[(i * 2) + 1] = state.parent.y;
        }
        tag.setTag("tiles", idArray);
        tag.setTag("meta", metaArray);
        tag.setTag("parents", parents);

        // Write tile meta:
        NbtList listTileMeta = new NbtList(NbtTagType.Compound);
        for(int x = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                CellBehavior meta = this.GetCellState(x, y).behavior;
                if(meta != null && meta is IHasData) {
                    NbtCompound behaviorTag = new NbtCompound();
                    behaviorTag.setTag("xPos", x);
                    behaviorTag.setTag("yPos", y);
                    ((IHasData)meta).WriteToNbt(behaviorTag);
                    listTileMeta.Add(behaviorTag);
                }
            }
        }
        tag.setTag("meta", listTileMeta);

        // Write fog:
        if(this.HasFog) {
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

        // Write hardness:
        tag.setTag("hardness", this.hardness);
    }

    public void ReadFromNbt(NbtCompound tag) {
        // Read tiles:
        int[] tileIds = tag.getIntArray("tiles");
        int[] metas = tag.getIntArray("meta");
        int[] parents = tag.getIntArray("parents");

        int index = 0;
        for(int x  = 0; x < this.size; x++) {
            for(int y = 0; y < this.size; y++) {
                int i = this.size * x + y;
                CellData d = Main.instance.CellRegistry[tileIds[i]];
                if(d == null) {
                    Debug.LogWarningFormat("Unknown Cell with id \"{0}\" at ({1},{2}).", tileIds[i], x, y);
                }

                Vector2Int parentPos = new Vector2Int(
                    parents[i * 2],
                    parents[(i * 2) + 1]);

                this.SetCell(
                    x,
                    y,
                    d == null ? Main.instance.CellRegistry.GetAir() : d,
                    metas[i],
                    false,
                    false);
                index++;
            }
        }

        // Read fog:
        if(this.HasFog) {
            this.fog.readFromNbt(tag.getCompound("fog"));
        }

        // Read temperatures:
        int[] tempArray = tag.getIntArray("temperature");
        for(int i = 0; i < Mathf.Min(this.temperatures.Length, tempArray.Length); i++) {
            this.temperatures[i] = tempArray[i] / 1_000_000f;
        }

        // Read hardness:
        this.hardness = tag.getIntArray("hardness");
    }

    private bool InBounds(int x, int y) {
        return x >= 0 && x < this.size && y >= 0 && y < this.size;
    }
}
