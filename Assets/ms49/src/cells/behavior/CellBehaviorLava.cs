using UnityEngine;

public class CellBehaviorLava : CellBehavior {

    [SerializeField]
    private Vector2 _fireSpreadRate = new Vector2(3, 8);

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);

        this.StartCoroutine(EntityFire.fireSpread(this.world, this.pos, this._fireSpreadRate));
    }
}
