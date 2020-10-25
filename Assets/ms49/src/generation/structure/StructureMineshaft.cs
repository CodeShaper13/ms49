using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mineshaft", menuName = "MS49/Structure/Mineshaft", order = 1)]
public class StructureMineshaft : StructureBase {

    [SerializeField]
    private CellData _support = null;
    [SerializeField]
    private CellData _chest = null;
    [SerializeField]
    private CellData _web = null;
    [SerializeField]
    private int _entityBatId = 4;
    [SerializeField]
    private int minHallwayLength = 4;
    [SerializeField]
    private int maxHallwayLength = 6;
    [SerializeField, Range(0, 1)]
    private float _childHallwayChance = 0.75f;
    [SerializeField]
    private float _roomChance = 0.25f;
    [SerializeField]
    private float _supportChance = 0.25f;
    [SerializeField]
    private float _webChance = 0.1f;
    [SerializeField]
    private float _chestChance = 0.6f;
    [SerializeField]
    private float _batsPerSquares = 0.1f;

    public override void placeIntoWorld(World world, Position pos) {
        List<Position> airPositions = new List<Position>();

        this.makeRoom(airPositions, world, pos, null);

        // Spawn bats

        int batCount = Mathf.RoundToInt(airPositions.Count * this._batsPerSquares);
        
        for(int i = 0; i < batCount; i++) {
            Position batPos = airPositions[Random.Range(0, airPositions.Count)];
            airPositions.Remove(batPos);

            world.entities.spawn(batPos, this._entityBatId);
        }
    }

    public void makeRoom(List<Position> airPositions, World world, Position pos, Rotation entryHallwayDir) {
        // Move the position to the center of the room
        if(entryHallwayDir != null) {
            pos += entryHallwayDir;
        }

        // Carve out the middle
        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                Position p1 = pos.add(x, y);
                this.safeSetCell(world, p1, null);
                airPositions.Add(p1);
            }
        }

        // Place a chest
        if(Random.value < this._chestChance) {
            this.safeSetCell(world, pos, this._chest);
            airPositions.Remove(pos);
        }

        // Make the hallways
        foreach(Rotation r in Rotation.ALL) {
            if(r.opposite() == entryHallwayDir) {
                continue;
            }

            // On outside rooms, skip a hallway leading away
            if(entryHallwayDir != null && r.opposite() == entryHallwayDir) {
                continue;
            }

            float hallwayChance = this._childHallwayChance;

            if(entryHallwayDir == null) { // The starting room
                hallwayChance = 1f;
            }

            if(Random.value < hallwayChance) {
                Position start = pos + r + r; // Make a pos on the outside of the room
                this.makeHallway(airPositions, world, start, r, entryHallwayDir == null); // Only make children if this is the starting room
            }
        }
    }

    public void makeHallway(List<Position> airPositions, World world, Position start, Rotation direction, bool generateChildren) {
        int hallwayLength = Random.Range(this.minHallwayLength, this.maxHallwayLength + 1);

        Position p = start;

        for(int i = 0; i <= hallwayLength; i++) {
            CellData cell = null;
            Rotation r = null;

            if(Random.value < this._webChance) {
                cell = this._web;
            }

            if(i % 2 == 0 && i != hallwayLength) { // No supports at the end
                if(Random.value < this._supportChance) {
                    cell = this._support;
                    r = direction;
                }
            }

            this.safeSetCell(world, p, cell, r);

            if(cell == null) {
                airPositions.Add(p);
            }

            p += direction;
        }

        if(Random.value < this._roomChance) {
            this.makeRoom(airPositions, world, p, direction);
        } else {
            if(generateChildren) {
                if(func()) {
                    this.makeHallway(airPositions, world, p, direction.clockwise(), false);
                }

                if(func()) {
                    this.makeHallway(airPositions, world, p, direction.counterClockwise(), false);
                }
            }
        }
    }

    private bool func() {
        return Random.value < this._childHallwayChance;
    }
}
