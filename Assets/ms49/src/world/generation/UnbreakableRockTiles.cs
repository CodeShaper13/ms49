using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tiles", menuName = "MS49/Generation/Unbreakable Tiles", order = 1)]
public class UnbreakableRockTiles : ScriptableObject {

    public CellData cTopLeft;
    public CellData cTopRight;
    public CellData cBottomLeft;
    public CellData cBottomRight;
    public CellData left;
    public CellData right;
    public CellData top;
    public CellData bottom;
    public CellData middle;
}
