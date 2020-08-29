using UnityEngine;

public class PopupBuyWorkers : PopupWorldReference {

    [Min(0)]
    public int workerCost = 1000;
    public IntVariable money = null;

    [SerializeField]
    private Position workerSpawnPoint = new Position(27, 25, 0);

    public void btnCallback(int entityId) {
        if(!CameraController.instance.inCreativeMode) {
            this.money.value -= this.workerCost;
        }

        // Add worker(s)
        this.world.entities.spawn(this.workerSpawnPoint, entityId);
    }
}
