﻿using System.Collections.Generic;
using UnityEngine;

public class FeatureCave : FeatureBase {

    [SerializeField]
    public CellData waterTile = null;
    [SerializeField]
    public CellData lavaTile = null;

    [Header("Cave Generation")]
    [SerializeField, Range(0, 100)]
    private int _randomFillPercent = 64;
    [SerializeField, Range(0, 10)]
    private int _smoothPasses = 3;
    [SerializeField, Range(0, 10)]
    private int _i = 5;
    [SerializeField, Range(0, 10)]
    private int _j = 4;

    [Header("Cave Prune Settings")]
    [SerializeField, Range(0, 100)]
    private float _roomFailChance = 10f;
    [SerializeField]
    private bool _pruneRooms = false;
    [SerializeField]
    private int _roomMinSize = 1;
    [SerializeField]
    private int _roomMaxSize = 100;

    [Header("Lake Settings")]
    [SerializeField, Range(0, 100)]
    private int _lakeFailPercent = 50;
    [SerializeField, Range(0, 100)]
    private int _lakeFillPercent = 80;
    [SerializeField]
    private int _lakeSmoothPasses = 3;

    public override void generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        if(layerData.generateCaves) {
            int[,] map = this.makeCaves(rnd, layerData.lakeType, accessor.size);

            // Set tiles.
            for(int x = 0; x < accessor.size; x++) {
                for(int y = 0; y < accessor.size; y++) {
                    int id = map[x, y];
                    CellData cell = null;

                    if(id == 0) {
                        cell = Main.instance.tileRegistry.getAir();
                    }
                    else if(id == 2) {
                        cell = layerData.lakeType == EnumLakeType.WATER ? this.waterTile : this.lavaTile;
                    }

                    if(cell != null) {
                        accessor.setCell(x, y, cell);
                    }
                }
            }
        }
    }

    public int[,] makeCaves(System.Random rnd, EnumLakeType lakeType, int size) {
        int[,] map = new int[size, size];
        this.RandomFillMap(rnd, map, size, this._randomFillPercent);

        for(int i = 0; i < this._smoothPasses; i++) {
            this.smoothMap(map, size);
        }

        List<List<Vector2Int>> roomRegions = this.GetRegions(map, 0, size);

        // Remove rooms that are too big or small.
        if(this._pruneRooms) {
            for(int i = roomRegions.Count - 1; i >= 0; i--) {
                List<Vector2Int> room = roomRegions[i];

                int roomSize = room.Count;
                if(rnd.Next(0, 100) < this._roomFailChance || (roomSize <= this._roomMinSize || roomSize >= this._roomMaxSize)) {
                    // Fill the room with dirt, we don't want it
                    foreach(Vector2Int tile in room) {
                        map[tile.x, tile.y] = 1;
                    }
                    roomRegions.RemoveAt(i);
                }
            }
        }

        // Fill rooms with water
        if(lakeType != EnumLakeType.NONE) {
            int[,] m = new int[size, size];
            foreach(List<Vector2Int> room in roomRegions) {
                if(rnd.Next(0, 100) < this._lakeFailPercent) {
                    continue; // Failed
                }

                // Fill array with -1
                for(int x = 0; x < size; x++) {
                    for(int y = 0; y < size; y++) {
                        m[x, y] = -1;
                    }
                }

                foreach(Vector2Int v in room) {
                    m[v.x, v.y] = 10; // Overwriten soon
                }

                this.RandomFillMap(rnd, m, size, this._lakeFillPercent);

                for(int i = 0; i < this._lakeSmoothPasses; i++) {
                    this.smoothMap(m, size);
                }

                // Apply the water to the map
                for(int x = 0; x < size; x++) {
                    for(int y = 0; y < size; y++) {
                        if(m[x, y] != -1) {
                            int i = m[x, y];

                            map[x, y] = (i == 1 ? 2 : 0);
                        }
                    }
                }
            }
        }

        return map;
    }

    private List<List<Vector2Int>> GetRegions(int[,] tiles, int tileType, int mapSize) {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        bool[,] inRoom = new bool[mapSize, mapSize];

        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                if(!inRoom[x, y] && tiles[x, y] == tileType) {
                    List<Vector2Int> newRegion = this.GetRegionTiles(tiles, x, y, mapSize);
                    regions.Add(newRegion);

                    foreach(Vector2Int tile in newRegion) {
                        inRoom[tile.x, tile.y] = true;
                    }
                }
            }
        }

        return regions;
    }

    private List<Vector2Int> GetRegionTiles(int[,] tiles, int startX, int startY, int mapSize) {
        List<Vector2Int> roomTiles = new List<Vector2Int>();
        int[,] mapFlags = new int[mapSize, mapSize];
        int tileType = tiles[startX, startY];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        mapFlags[startX, startY] = 1;

        while(queue.Count > 0) {
            Vector2Int tile = queue.Dequeue();
            roomTiles.Add(tile);

            for(int x = tile.x - 1; x <= tile.x + 1; x++) {
                for(int y = tile.y - 1; y <= tile.y + 1; y++) {
                    if(this.withinBounds(tiles, x, y) && (y == tile.y || x == tile.x)) {
                        if(mapFlags[x, y] == 0 && tiles[x, y] == tileType) {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }

        return roomTiles;
    }

    private bool withinBounds(int[,] tiles, int x, int y) {
        return x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1);
    }

    /// <summary>
    /// Randomly sets non -1 cells to 0 or 1.
    /// </summary>
	private void RandomFillMap(System.Random rnd, int[,] tiles, int mapSize, int rndFillPercent) {
        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                if(tiles[x, y] != -1) {

                    if(x == 0 || x == mapSize - 1 || y == 0 || y == mapSize - 1) {
                        tiles[x, y] = 1;
                    }
                    else {
                        tiles[x, y] = (rnd.Next(0, 100) < rndFillPercent) ? 1 : 0;
                    }
                }
            }
        }
    }

    private void smoothMap(int[,] tiles, int mapSize) {
        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                if(tiles[x, y] != -1) {
                    int neighbourWallTiles = this.getSurroundingWallCount(tiles, mapSize, x, y);

                    if(neighbourWallTiles > this._i) {
                        tiles[x, y] = 1; // Stone
                    }
                    else if(neighbourWallTiles < this._j) {
                        tiles[x, y] = 0; // Air
                    }
                }
            }
        }
    }

    private int getSurroundingWallCount(int[,] tiles, int mapSize, int x, int y) {
        int wallCount = 0;

        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            for(int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {

                if(this.withinBounds(tiles, neighbourX, neighbourY)) {
                    if(neighbourX == x && neighbourY == y) {
                        continue; // don't count the middle.
                    }

                    if(tiles[neighbourX, neighbourY] == 1) {
                        wallCount++;
                    }
                }
                else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }
}