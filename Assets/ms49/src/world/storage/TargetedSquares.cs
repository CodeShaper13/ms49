using UnityEngine;
using System.Collections.Generic;
using fNbt;

public class TargetedSquares : MonoBehaviour, ISaveableState {

    [SerializeField]
    private WorldRenderer worldRenderer = null;

    public HashSet<Position> list { get; private set; }

    public string tagName => "targetedSquares";

    private void Awake() {
        this.list = new HashSet<Position>();
    }

    public bool isTargeted(Position pos) {
        return this.list.Contains(pos);
    }

    public void setTargeted(Position pos, bool targeted) {
        if(targeted) {
            if(!this.list.Contains(pos)) {
                this.list.Add(pos);
                this.worldRenderer.dirtyExcavationTarget(pos, targeted);
            }
        }
        else {
            this.list.Remove(pos);
            this.worldRenderer.dirtyExcavationTarget(pos, targeted);
        }
    }

    public void writeToNbt(NbtCompound tag) {
        NbtList targetedTilesList = new NbtList(NbtTagType.Compound);
        foreach(Position pos in this.list) {
            NbtCompound compound = new NbtCompound();
            compound.setTag("x", pos.x);
            compound.setTag("y", pos.y);
            compound.setTag("depth", pos.depth);
            targetedTilesList.Add(compound);
        }
        tag.setTag("targetedTiles", targetedTilesList);
    }

    public void readFromNbt(NbtCompound tag) {
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
