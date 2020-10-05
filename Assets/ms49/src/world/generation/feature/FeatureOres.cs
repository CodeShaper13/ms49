using System.Collections.Generic;
using UnityEngine;

public class FeatureOres : FeatureBase {

    private int _chunkCount = 6;

    private Vector2Int[] dirs = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };


    List<Vector2> samples;

    /*
    private void OnDrawGizmos() {
        //if(samples == null) {
            int SIZE = 96;

            samples = QuasiRandom.poisson_disk_sampling(10, 30, SIZE);
        print(samples.Count);
        //}

        Gizmos.color = Color.red;
        foreach(Vector2 v in samples) {
            Gizmos.DrawSphere(v, 1f);
        }

        for(int chunkPosX = 0; chunkPosX < this._chunkCount; chunkPosX++) {
            for(int chunkPosY = 0; chunkPosY < this._chunkCount; chunkPosY++) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(new Vector3(chunkPosX * 16 + 8, chunkPosY * 16 + 8), Vector3.one * 16);
            }
        }
    }
    */

    public override void generate(System.Random rnd, LayerDataBase layerData, MapAccessor accessor) {
        for(int chunkPosX = 0; chunkPosX < this._chunkCount; chunkPosX++) {
            for(int chunkPosY = 0; chunkPosY < this._chunkCount; chunkPosY++) {

                foreach(OreSettings setting in layerData.oreSpawnSettings) {
                    if(setting != null && setting.cell != null) {
                        for(int i = 0; i < setting.veinCount; i++) {
                            this.makeVein(rnd, setting.cell, setting, accessor, chunkPosX, chunkPosY);
                        }
                    }
                }
            }
        }
    }

    private void makeVein(System.Random rnd, CellData oreCell, OreSettings setting, MapAccessor accessor, int i1, int j1) {
        int chunkX = rnd.Next(0, 16); // Don't start on edge cells.
        int chunkY = rnd.Next(0, 16);

        int size = rnd.Next(setting.veinSize.x, setting.veinSize.y + 1);

        Rotation lastDir = null;
        for(int i = 0; i < size; i++) {
            int x = (i1 * 16) + chunkX;
            int y = (j1 * 16) + chunkY;

            CellData c = accessor.getCell(x, y);
            if(c is CellDataMineable) {
                accessor.setCell(x, y, oreCell);
            }

            Rotation r = Rotation.ALL[rnd.Next(0, Rotation.ALL.Length)];

            if(lastDir != null && lastDir == r) {
                r = r.opposite();
            }

            chunkX += r.vector.x;
            chunkY += r.vector.y;
        }
    }
}
