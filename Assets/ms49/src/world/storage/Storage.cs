using System.Collections.Generic;
using fNbt;
using UnityEngine;

public class Storage {

    private World world;
    private Layer[] layers;
    private byte[] aboveGroundMap;
    /// <summary>
    /// A lsit of all behaviors.  This exists so ALL behaviors on a
    /// layer can be iterated though faster.  Without this, you would
    /// have to check every tile for a behavior.
    /// </summary>
    public HashSet<CellBehavior> cachedBehaviors { get; private set; }

    public int layerCount { get; private set; }
    public Transform behaviorHolder { get; private set; }
    public Position workerSpawnPoint { get; set; }

    public Storage(World world) {
        this.world = world;
        this.layerCount = this.world.MapGenerator.LayerCount;
        this.layers = new Layer[this.layerCount];
        this.aboveGroundMap = new byte[this.world.MapSize * this.world.MapSize];
        this.cachedBehaviors = new HashSet<CellBehavior>();

        this.behaviorHolder = this.world.CreateWrapper("BEHAVIOR_HOLDER");
    }

    public CellState getCellState(Position pos) {
        return this.GetLayer(pos.depth).getCellState(pos.x, pos.y);
    }

    public bool IsOutside(int x, int y) {
        return this.aboveGroundMap[this.world.MapSize * x + y] == 1;
    }

    // This is only used during world generation.
    public void SetOutside(int x, int y, bool aboveGround) {
        this.aboveGroundMap[this.world.MapSize * x + y] = aboveGround ? (byte)1 : (byte)0;
    }

    public float GetTemperature(Position pos) {
        Layer layer = this.GetLayer(pos.depth);
        if(layer != null) {
            return layer.GetTemperature(pos.x, pos.y);
        } else {
            return 0;
        }
    }

    public Layer GetLayer(int depth) {
        if(depth >= 0 && depth < this.layerCount) {
            return this.layers[depth];
        }
        return null;
    }

    public void SetLayer(Layer layer, int depth) {
        this.layers[depth] = layer;
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.setTag("aboveGroundMap", this.aboveGroundMap);

        tag.setTag("workerSpawnPoint", this.workerSpawnPoint);

        // Write Layers:
        NbtList listLayers = new NbtList(NbtTagType.Compound);
        for(int i = 0; i < this.layers.Length; i++) {
            if(this.layers[i] != null) {
                NbtCompound tagLayer = new NbtCompound();
                this.layers[i].WriteToNbt(tagLayer);
                listLayers.Add(tagLayer);
            }
        }

        tag.setTag("layers", listLayers);
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.aboveGroundMap = tag.getByteArray("aboveGroundMap");

        this.workerSpawnPoint = tag.getPosition("workerSpawnPoint");

        // Read Layers:
        NbtList layers = tag.getList("layers");
        for(int i = 0; i < layers.Count; i++) {
            Layer layer = new Layer(this.world, i);
            NbtCompound layerCompound = layers.Get<NbtCompound>(i);
            layer.ReadFromNbt(layerCompound);
            this.layers[layer.depth] = layer;

            // Call onCreate method for all the behaviors, now that all Cell's
            // have been loaded.
            for(int x = 0; x < this.world.MapSize; x++) {
                for(int y = 0; y < this.world.MapSize; y++) {
                    CellState state = layer.getCellState(x, y);
                    if(state.behavior != null) {
                        state.behavior.onCreate(this.world, state, new Position(x, y, layer.depth));
                    }
                }
            }

            // After onCreate is called for all of the behaviors, let them read their
            // state from NBT.
            NbtList listBehaviorTags = layerCompound.getList("meta");
            foreach(NbtCompound behaviorTag in listBehaviorTags) {
                CellBehavior meta = layer.getCellState(behaviorTag.getInt("xPos"), behaviorTag.getInt("yPos")).behavior;
                if(meta != null && meta is IHasData) {
                    ((IHasData)meta).ReadFromNbt(behaviorTag);
                }
            }
        }
    }
}
