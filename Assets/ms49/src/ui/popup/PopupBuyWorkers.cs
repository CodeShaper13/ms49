using UnityEngine;

public class PopupBuyWorkers : PopupWindow {

    public void btnCallback(BuyWorkerBtn btn) {
        Money.remove(btn.cost);

        // Add worker(s)
        World world = GameObject.FindObjectOfType<World>();

        for(int i = 0; i < btn.workersGiven; i++) {
            world.spawnEntity(new Position(27, 25, 0), 1); // 1 = miner id
        }
    }
}
