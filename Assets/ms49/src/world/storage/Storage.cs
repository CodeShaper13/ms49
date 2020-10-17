using System.Collections.Generic;
using fNbt;
using UnityEngine;

public class Storage {

    private World world;
    private Layer[] layers;
    private byte[] aboveGroundMap;
    public HashSet<CellBehavior> cachedBehaviors { get; private set; }

    public int layerCount { get; private set; }
    public Transform behaviorHolder { get; private set; }
    public Position workerSpawnPoint { get; set; }

    public Storage(World world) {
        this.world = world;
        this.layerCount = this.world.mapGenerator.layerCount;
        this.layers = new Layer[layerCount];
        this.aboveGroundMap = new byte[this.world.mapSize * this.world.mapSize];
        this.cachedBehaviors = new HashSet<CellBehavior>();
        this.behaviorHolder = this.world.createHolder("BEHAVIOR_HOLDER");
    }

    public void setCell(Position pos, CellData tile, Rotation rotation, bool updateNeighbors = false) {
        this.getLayer(pos.depth).setCell(pos.x, pos.y, tile, rotation, updateNeighbors);
    }

    public CellState getCellState(Position pos) {
        return this.getLayer(pos.depth).getCellState(pos.x, pos.y);
    }

    public bool isOutside(int x, int y) {
        return this.aboveGroundMap[this.world.mapSize * x + y] == 1;
    }

    public void setOutside(int x, int y, bool aboveGround) {
        this.aboveGroundMap[this.world.mapSize * x + y] = aboveGround ? (byte)1 : (byte)0;
    }

    public float getTemperature(Position pos) {
        Layer layer = this.getLayer(pos.depth);
        if(layer != null) {
            return layer.getTemperature(pos.x, pos.y);
        } else {
            return 0;
        }
    }

    public Layer getLayer(int depth) {
        if(depth >= 0 && depth < this.layerCount) {
            return this.layers[depth];
        }
        return null;
    }

    public void setLayer(Layer layer, int depth) {
        this.layers[depth] = layer;
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("aboveGroundMap", this.aboveGroundMap);

        tag.setTag("workerSpawnPoint", this.workerSpawnPoint);

        // Write Layers:
        NbtList listLayers = new NbtList(NbtTagType.Compound);
        for(int i = 0; i < this.layers.Length; i++) {
            if(this.layers[i] != null) {
                NbtCompound tagLayer = new NbtCompound();
                this.layers[i].writeToNbt(tagLayer);
                listLayers.Add(tagLayer);
            }
        }

        tag.setTag("layers", listLayers);
    }

    public void readFromNbt(NbtCompound tag) {
        this.aboveGroundMap = tag.getByteArray("aboveGroundMap");

        this.workerSpawnPoint = tag.getPosition("workerSpawnPoint");

        // Read Layers:
        NbtList layers = tag.getList("layers");
        for(int i = 0; i < layers.Count; i++) {
            Layer layer = new Layer(this.world, i);
            NbtCompound layerCompound = layers.Get<NbtCompound>(i);
            layer.readFromNbt(layerCompound);
            this.layers[layer.depth] = layer;

            // Call onCreate method for all the behaviors, now that all Cell's
            // have been loaded.
            for(int x = 0; x < this.world.mapSize; x++) {
                for(int y = 0; y < this.world.mapSize; y++) {
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
                    ((IHasData)meta).readFromNbt(behaviorTag);
                }
            }
        }
    }
}
