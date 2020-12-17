using UnityEngine;

[CreateAssetMenu(fileName = "Cell Rail", menuName = "MS49/Cell/Cell Rail", order = 1)]
public class CellDataRail : CellData {

    public EnumRailMoveType moveType;

    public enum EnumRailMoveType {
        STRAIGHT = 0,
        CROSSING = 1,
        CURVE = 2,
        STOPPER = 3,
        MERGER = 4,
    }
}
