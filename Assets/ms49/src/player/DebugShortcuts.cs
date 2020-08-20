using UnityEngine;

public class DebugShortcuts : MonoBehaviour {

    [SerializeField]
    private World world = null;

    private void Update() {
        if(!Pause.isPaused() && !PopupWindow.blockingInput() && Main.DEBUG) {
            Position pos = CameraController.instance.getMousePos();

            if(Input.GetMouseButtonDown(0)) {

                if(Input.GetKey(KeyCode.LeftControl)) {
                    foreach(EntityBase e in this.world.entities.entityList) {
                        if(e is EntityWorker) {
                            ((EntityWorker)e).moveHelper.setDestination(pos);
                        }
                    }
                }

                if(Input.GetKey(KeyCode.Delete)) {
                    world.setCell(pos, Main.instance.tileRegistry.getAir());
                    world.liftFog(pos);
                }

                if(Input.GetKey(KeyCode.F)) {
                    world.entities.spawn(pos, 10);
                }
            }
        }
    }
}
