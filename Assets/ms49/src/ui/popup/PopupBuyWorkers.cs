using UnityEngine;

public class PopupBuyWorkers : PopupWindow {

    [Min(0)]
    public int workerCost = 1000;
    public IntVariable money = null;

    [SerializeField]
    private Position workerSpawnPoint = new Position(27, 25, 0);

    public void btnCallback(int entityId) {
        this.money.value -= this.workerCost;

        // Add worker(s)
        World world = GameObject.FindObjectOfType<World>();

        world.entities.spawn(this.workerSpawnPoint, entityId);
    }
}
