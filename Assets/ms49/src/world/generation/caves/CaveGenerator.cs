using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator {

    private int[,] map;
    private MapGenerationData.TileReferences tiles;

    public CaveGenerator(MapGenerationData.TileReferences tiles) {
        this.tiles = tiles;
    }

    public void generateCaves(System.Random rnd, LayerDataBase layerData, MapAccessor accessor) {
        this.map = new int[accessor.size, accessor.size];
        this.randomlyFillMap(rnd, layerData.caveFillPercent, accessor.size);

        for(int i = 0; i < layerData.caveSmoothPases; i++) {
            this.smoothMap(accessor.size);
        }

        if(layerData.lakeType != LakeType.NONE) {
            this.createLakes(rnd, layerData, accessor);
        }

        // Set tiles.
        for(int x = 0; x < accessor.size; x++) {
            for(int y  = 0; y < accessor.size; y++) {
                int id = this.map[x, y];
                CellData cell;

                if(id == 0) {
                    cell = null;
                } else if(id == 1) {
                    cell = layerData.getFillCell();
                } else if(id == 2) {
                    cell = this.tiles.waterTile;
                } else { // 3
                    cell = this.tiles.lavaTile;
                }

                accessor.setCell(x, y, cell);
            }
        }
    }

    private void createLakes(System.Random rnd, LayerDataBase layerData, MapAccessor accessor) {
        List<List<Vector2Int>> roomRegions = this.GetRegions(0, accessor.size);

        foreach(List<Vector2Int> roomRegion in roomRegions) {
            if(rnd.Next(0, 101) < layerData.lakeChance) {
                foreach(Vector2Int tile in roomRegion) {
                    map[tile.x, tile.y] = layerData.lakeType == LakeType.WATER ? 2 : 3;
                }
            }
        }
    }

    private List<List<Vector2Int>> GetRegions(int tileType, int mapSize) {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        int[,] mapFlags = new int[mapSize, mapSize];

        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                if(mapFlags[x, y] == 0 && map[x, y] == tileType) {
                    List<Vector2Int> newRegion = this.GetRegionTiles(x, y, mapSize);
                    regions.Add(newRegion);

                    foreach(Vector2Int tile in newRegion) {
                        mapFlags[tile.x, tile.y] = 1;
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
                    map[x, y] = 1; // Edges always filled.
                } else {
                    map[x, y] = (rnd.Next(0, 100) < rndFillPercent) ? 1 : 0;
                }
            }
        }
    }

    private void smoothMap(int size) {
        for(int x = 0; x < size; x++) {
            for(int y = 0; y < size; y++) {
                int neighbourWallTiles = this.getSurroundingWallCount(size, x, y);

                if(neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if(neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    private int getSurroundingWallCount(int size, int x, int y) {
        int wallCount = 0;
        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            for(int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                if(neighbourX >= 0 && neighbourX < size && neighbourY >= 0 && neighbourY < size) {
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