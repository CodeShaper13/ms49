using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Cell Rail", menuName = "MS49/Cell/Cell Rail", order = 1)]
public class CellDataRail : CellData {

    [SerializeField]
    private RailConnection[] connections;

    [Serializable]
    public class RailConnection {

        public EnumRotation to;
        public EnumRotation from;
        [Tooltip("If true, minecarts can travel both ways.")]
        public bool bothWays = true;
    }
}
