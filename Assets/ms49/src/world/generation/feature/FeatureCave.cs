using System.Collections.Generic;
using UnityEngine;

public class FeatureCave :FeatureBase {

    [SerializeField]
    public CellData waterTile = null;
    [SerializeField]
    public CellData lavaTile = null;
    [SerializeField]
    public float f1 = 4;
    [SerializeField]
    public float f2 = 4;
    [SerializeField]
    private float roomMinSize = 1f;
    [SerializeField]
    private float roomMaxSize = 100f;

    private int[,] map;

    public override void generate(System.Random rnd, LayerDataBase layerData, MapAccessor accessor) {
        if(!layerData.generateCaves) {
            return;
        }

        this.map = new int[accessor.size, accessor.size];
        this.randomlyFillMap(rnd, layerData.caveFillPercent, accessor.size);

        for(int i = 0; i < layerData.caveSmoothPases; i++) {
            this.smoothMap(accessor.size);
        }

        List<List<Vector2Int>> roomRegions = this.GetRegions(0, accessor.size);

        // Remove rooms that are too big or small
        foreach(List<Vector2Int> room in roomRegions) {
            if(room.Count <= this.roomMinSize || room.Count >= this.roomMaxSize) {
                foreach(Vector2Int tile in room) {
                    this.map[tile.x, tile.y] = 1;
                }
                continue;
            }
        }

        if(layerData.lakeType != LakeType.NONE) {
            this.createLakes(rnd, layerData, accessor, roomRegions);
        }

        // Set tiles.
        for(int x = 0; x < accessor.size; x++) {
            for(int y  = 0; y < accessor.size; y++) {
                int id = this.map[x, y];
                CellData cell = null;

                if(id == 0) {
                    cell = Main.instance.tileRegistry.getAir();
                } else if(id == 2) {
                    cell = this.waterTile;
                } else if(id == 3) {
                    cell = this.lavaTile;
                }

                if(cell != null) {
                    accessor.setCell(x, y, cell);
                }
            }
        }
    }

    private void createLakes(System.Random rnd, LayerDataBase layerData, MapAccessor accessor, List<List<Vector2Int>> roomRegions) {
        foreach(List<Vector2Int> room in roomRegions) {
            if(rnd.Next(0, 101) < layerData.lakeChance) {
                foreach(Vector2Int tile in room) {
                    map[tile.x, tile.y] = (layerData.lakeType == LakeType.WATER) ? 2 : 3;
                }
            }
        }
    }

    private List<List<Vector2Int>> GetRegions(int tileType, int mapSize) {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        bool[,] inRoom = new bool[mapSize, mapSize];

        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                if(!inRoom[x, y] && map[x, y] == tileType) {
                    List<Vector2Int> newRegion = this.GetRegionTiles(x, y, mapSize);
                    regions.Add(newRegion);

                    foreach(Vector2Int tile in newRegion) {
                        inRoom[tile.x, tile.y] = true;
                    }
                }
            }
        }

        return regions;
    }

    private List<Vector2Int> GetRegionTiles(int startX, int startY, int mapSize) {
        List<Vector2Int> tiles = new List<Vector2Int>();
        int[,] mapFlags = new int[mapSize, mapSize];
        int tileType = map[startX, startY];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        mapFlags[startX, startY] = 1;

        while(queue.Count > 0) {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            for(int x = tile.x - 1; x <= tile.x + 1; x++) {
                for(int y = tile.y - 1; y <= tile.y + 1; y++) {
                    if(this.IsInMapRange(x, y, mapSize) && (y == tile.y || x == tile.x)) {
                        if(mapFlags[x, y] == 0 && map[x, y] == tileType) {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    private bool IsInMapRange(int x, int y, int mapSize) {
        return x >= 0 && x < mapSize && y >= 0 && y < mapSize;
    }

    private void randomlyFillMap(System.Random rnd, float rndFillPercent, int size) {
        for(int x = 0; x < size; x++) {
            for(int y = 0; y < size; y++) {
                if(x == 0 || x == size - 1 || y == 0 || y == size - 1) {
                    this.map[x, y] = 1; // Edges always filled.
                } else {
                    this.map[x, y] = (rnd.Next(0, 100) < rndFillPercent) ? 1 : 0;
                }
            }
        }
    }

    private void smoothMap(int mapSize) {
        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                int neighbourWallTiles = this.getSurroundingWallCount(mapSize, x, y);

                if(neighbourWallTiles > this.f1) {
                    this.map[x, y] = 1; // Stone
                }
                else if(neighbourWallTiles < this.f2) {
                    this.map[x, y] = 0; // Air
                }

            }
        }
    }

    private int getSurroundingWallCount(int mapSize, int x, int y) {
        int wallCount = 0;
        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            for(int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                if(neighbourX >= 0 && neighbourX < mapSize && neighbourY >= 0 && neighbourY < mapSize) {
                    if(neighbourX != x || neighbourY != y) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                } else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }
}