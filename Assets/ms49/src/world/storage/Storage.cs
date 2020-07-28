using System.Collections.Generic;
using fNbt;
using UnityEngine;

// No safety checks are done for the passed position in all of the getters and setters.

public class Storage {

    private World world;
    private Layer[] layers;
    public HashSet<CellBehavior> cachedBehaviors { get; private set; }
    public HashSet<Position> targetedForRemovalSquares { get; private set; }

    public int layerCount { get; private set; }
    public int mapSize { get;  private set; }

    public Storage(World world) {
        this.world = world;
        this.mapSize = this.world.mapGenData.mapSize;
        this.layerCount = this.world.mapGenData.layerCount;
        this.layers = new Layer[layerCount];
        this.cachedBehaviors = new HashSet<CellBehavior>();
        this.targetedForRemovalSquares = new HashSet<Position>();
    }

    public bool isLayerGenerated(int depth) {
        return this.isValidLayer(depth) && this.layers[depth] != null;
    }

    public bool isValidLayer(int depth) {
        return depth >= 0 && depth < this.layerCount;
    }

    public void setCell(Position pos, CellData tile, Rotation rotation) {
        this.getLayer(pos.depth).setCell(pos.x, pos.y, tile, rotation);
    }

    public CellState getCellState(Position pos) {
        return this.getLayer(pos.depth).getCellState(pos.x, pos.y);
    }

    public bool isTargeted(Position pos) {
        return this.targetedForRemovalSquares.Contains(pos);
    }

    public void setTargeted(Position pos, bool targeted) {
        WorldRenderer renderer = GameObject.FindObjectOfType<WorldRenderer>(); // TODO remove find call
        if(targeted) {
            if(!this.targetedForRemovalSquares.Contains(pos)) {
                //if(this.targetedForRemovalSquares.Count < 100) { // Limit 100 targeted squares
                    this.targetedForRemovalSquares.Add(pos);
                    renderer.setExcavationTargetVisability(pos.x, pos.y, true);
                //}
            }
        }
        else {
            this.targetedForRemovalSquares.Remove(pos);
            renderer.setExcavationTargetVisability(pos.x, pos.y, false);
        }
    }

    public Layer getLayer(int depth) {
        if(this.isValidLayer(depth)) {
            return this.layers[depth];
        }
        return null;
    }

    public void setLayer(Layer layer, int depth) {
        this.layers[depth] = layer;
    }

    public void writeToNbt(NbtCompound tag) {
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

        // Write targeted tiles to NBT.
        NbtList targetedTilesList = new NbtList(NbtTagType.Compound);
        foreach(Position pos in this.targetedForRemovalSquares) {
            NbtCompound compound = new NbtCompound();
            compound.setTag("x", pos.x);
            compound.setTag("y", pos.y);
            compound.setTag("depth", pos.depth);
            targetedTilesList.Add(compound);
        }
        tag.setTag("targetedTiles", targetedTilesList);
    }

    public void readFromNbt(NbtCompound tag) {
        // Read Layers:
        NbtList layers = tag.getList("layers");
        for(int i = 0; i < layers.Count; i++) {
            Layer layer = new Layer(this.world, i);
            layer.readFromNbt(layers.Get<NbtCompound>(i));
            this.layers[layer.depth] = layer;
        }

        // Read targeted sqaures.
        NbtList listTargetedTags = tag.getList("targetedTiles");
        foreach(NbtCompound targetedTag in listTargetedTags) {
            if(!targetedTag.Contains("x") || !targetedTag.Contains("y")) {
                Debug.LogWarning("Targeted Tile found with a position of -1.  Ignoring!");
                continue;
            }
            int x = targetedTag.getInt("x");
            int y = targetedTag.getInt("y");
            int depth = targetedTag.getInt("depth");
            this.setTargeted(new Position(x, y, depth), true);
        }
    }
}
