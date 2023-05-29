using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mineshaft", menuName = "MS49/Structure/Mineshaft", order = 1)]
public class StructureMineshaft : StructureData {

    [SerializeField]
    private CellData _support = null;
    [SerializeField]
    private CellData _chest = null;
    [SerializeField]
    private CellData _web = null;
    [SerializeField]
    private LootTable _chestLootTable = null;

    [Space]

    [SerializeField, MinMaxSlider(0, 10)]
    private Vector2Int _mineshaftsPerLayer = new Vector2Int(3, 4);
    [SerializeField, Tooltip("The number of pieces extending from the center in small mineshafts")]
    private int _smallShaftSize = 2;
    [SerializeField, Tooltip("The number of pieces extending from the center in large mineshafts")]
    private int _largeShaftSize = 4;
    [SerializeField, Min(0)]
    private int minHallwayLength = 4;
    [SerializeField, Min(1)]
    private int maxHallwayLength = 6;
    [SerializeField, Range(0, 1)]
    private float _childHallwayChance = 0.75f;
    [SerializeField, Range(0, 1)]
    private float _roomChance = 0.25f;
    [SerializeField, MinMaxSlider(0, 16)]
    private Vector2Int _plotCenterOffset = new Vector2Int(0, 1);

    [Space]

    [SerializeField]
    private int _entityBatId = 4;
    [SerializeField, Range(0, 1)]
    private float _batsPerSquares = 0.1f;

    [Space]

    [SerializeField, Range(0, 1)]
    private float _supportChance = 0.25f;
    [SerializeField, Range(0, 1)]
    private float _webChance = 0.1f;
    [SerializeField, Range(0, 1)]
    private float _chestChance = 0.6f;

    public override void Generate(World world, int depth) {
        List<Plot> list = new List<Plot>(world.plotManager.plots);

        // Small shaft.
        Position smallShaftPos = new Position(world.MapSize / 2, 20, depth);
        this.makeShaft(
            world,
            smallShaftPos,
            this._smallShaftSize);
        list.RemoveAll(plot => plot.Contains(smallShaftPos.x, smallShaftPos.y));

        // Large shaft.
        int j = Random.Range(this._mineshaftsPerLayer.x, this._mineshaftsPerLayer.y);
        for(int i = 0; i < j; i++) {
            int index = Random.Range(0, list.Count);
            Plot plot = list[index];

            int x = ((int)plot.rect.center.x) + Random.Range(this._plotCenterOffset.x, this._plotCenterOffset.y);
            int y = ((int)plot.rect.center.y) + Random.Range(this._plotCenterOffset.x, this._plotCenterOffset.y);
            this.makeShaft(
                world,
                new Position(x, y, depth),
                this._largeShaftSize);

            list.RemoveAt(index);
        }
    }

    private void makeShaft(World world, Position start, int childCounter) {
        List<Position> airPositions = new List<Position>();

        this.makeRoom(airPositions, world, start, null, childCounter);

        this.spawnBats(airPositions, world);
    }

    private void makeRoom(List<Position> airPositions, World world, Position pos, Rotation entryHallwayDir, int childCounter) {
        // Move the position to the center of the room
        if(entryHallwayDir != null) {
            pos += entryHallwayDir;
        }

        // Carve out the middle
        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                Position p1 = pos.Add(x, y);
                if(this.safeSetCell(world, p1, null)) {
                    airPositions.Add(p1);
                }
            }
        }

        // Place a chest
        if(Random.value < this._chestChance) {
            if(this.safeSetContainer(world, pos, this._chest, Rotation.UP, this._chestLootTable)) {
                airPositions.Remove(pos);
            }
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
                this.makeHallway(airPositions, world, start, r, childCounter - 1); // Only make children if this is the starting room
            }
        }
    }

    private void makeHallway(List<Position> airPositions, World world, Position start, Rotation direction, int childCounter) {
        int hallwayLength = Random.Range(this.minHallwayLength, this.maxHallwayLength + 1) + childCounter;

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

            if(this.safeSetCell(world, p, cell, r)) {
                if(cell == null) {
                    airPositions.Add(p);
                }
            }

            p += direction;
        }

        bool flag = childCounter > 2;

        childCounter -= 1;

        if(Random.value < this._roomChance) {
            this.makeRoom(airPositions, world, p, direction, childCounter);
        } else {
            if(childCounter > 0) {
                if(flag || func()) {
                    this.makeHallway(airPositions, world, p, direction.clockwise(), childCounter);
                }

                if(flag || func()) {
                    this.makeHallway(airPositions, world, p, direction.counterClockwise(), childCounter);
                }
            }
        }
    }

    private void spawnBats(List<Position> airPositions, World world) {
        int batCount = Mathf.RoundToInt(airPositions.Count * this._batsPerSquares);

        for(int i = 0; i < batCount; i++) {
            Position batPos = airPositions[Random.Range(0, airPositions.Count)];
            airPositions.Remove(batPos);

            world.entities.Spawn(batPos, this._entityBatId);
        }
    }

    private bool func() {
        return Random.value < this._childHallwayChance;
    }
}
