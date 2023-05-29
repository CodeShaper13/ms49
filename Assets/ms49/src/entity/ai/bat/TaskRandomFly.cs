using UnityEngine;

public class TaskRandomFly : TaskMovement<EntityBat> {

    [SerializeField]
    private int _minMoveDistance = 2;
    [SerializeField]
    private int _maxMoveDistance = 5;
    [SerializeField, MinMaxSlider(0.01f, 60)]
    private Vector2 _randomFlyTry = new Vector2(1, 1.5f);

    private float timer;

    public override void onTaskStart() {
        base.onTaskStart();

        this.timer = this.getRndTime();
    }

    public override bool shouldExecute() {
        this.timer -= Time.deltaTime;

        if(this.timer <= 0) {
            this.timer = this.getRndTime();

            int x = this.func();
            int y = this.func();
            Position dest = this.owner.Position.Add(x, y);
            if(!this.owner.world.IsOutOfBounds(dest) && this.owner.world.GetCellState(dest).data.IsWalkable) {
                if(this.calculateAndSetPath(dest)) {
                    return true;
                }
            }
        }

        return false;
    }

    public override void onDestinationArive() {
        base.onDestinationArive();

        this.navPath = null;
    }

    public override bool continueExecuting() {
        return this.navPath != null;
    }

    private int func() {
        return Random.Range(this._minMoveDistance, this._maxMoveDistance + 1) * (Random.value > 0.5 ? 1 : -1);
    }

    private float getRndTime() {
        return Random.Range(this._randomFlyTry.x, this._randomFlyTry.y);
    }
}
