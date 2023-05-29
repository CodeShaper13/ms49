﻿using UnityEngine;
using System.Collections.Generic;
using fNbt;

public class TargetedSquares : MonoBehaviour, ISaveableState {

    [SerializeField]
    private WorldRenderer worldRenderer = null;

    public HashSet<TargetedSquare> list { get; private set; }

    public string saveableTagName => "targetedSquares";

    private void Awake() {
        this.list = new HashSet<TargetedSquare>();
    }

    private TargetedSquare get(Position pos) {
        foreach(TargetedSquare ts in this.list) {
            if(ts.pos == pos) {
                return ts;
            }
        }

        return null;
    }

    public bool isTargeted(Position pos) {
        TargetedSquare ts = this.get(pos);
        return ts != null;
    }

    public bool isPriority(Position pos) {
        TargetedSquare ts = this.get(pos);
        if(ts == null) {
            return false;
        } else {
            return ts.isPriority;
        }
    }

    public void stopTargeting(Position pos) {
        this.list.RemoveWhere((ts) => ts.pos == pos);
        this.worldRenderer.dirtyExcavationTarget(pos, null);
    }

    public void startTargeting(Position pos, bool isPriority) {
        if(!this.isTargeted(pos)) {
            TargetedSquare ts = new TargetedSquare(pos, isPriority);
            this.list.Add(ts);
            this.worldRenderer.dirtyExcavationTarget(pos, ts);
        }
    }

    public void WriteToNbt(NbtCompound tag) {
        NbtList targetedTilesList = new NbtList(NbtTagType.Compound);
        foreach(TargetedSquare square in this.list) {
            NbtCompound compound = new NbtCompound();
            compound.SetTag("x", square.pos.x);
            compound.SetTag("y", square.pos.y);
            compound.SetTag("depth", square.pos.depth);
            compound.SetTag("isPriority", square.isPriority);
            targetedTilesList.Add(compound);
        }
        tag.SetTag("targetedTiles", targetedTilesList);
    }

    public void ReadFromNbt(NbtCompound tag) {
        NbtList listTargetedTags = tag.GetList("targetedTiles");
        foreach(NbtCompound targetedTag in listTargetedTags) {
            if(!targetedTag.Contains("x") || !targetedTag.Contains("y")) {
                Debug.LogWarning("Targeted Tile found with a position of -1.  Ignoring!");
                continue;
            }
            int x = targetedTag.GetInt("x");
            int y = targetedTag.GetInt("y");
            int depth = targetedTag.GetInt("depth");
            bool priority = targetedTag.GetBool("isPriority");
            this.startTargeting(new Position(x, y, depth), priority);
        }
    }
}
