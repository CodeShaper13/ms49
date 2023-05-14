using UnityEngine;

public class DebugShortcuts : MonoBehaviour {

    [SerializeField]
    private World world = null;

    private void Update() {
        if(!Pause.isPaused() && !PopupWindow.blockingInput() && Main.DEBUG) {
            Position pos = CameraController.instance.getMousePos();

            if(Input.GetMouseButtonDown(0)) {

                if(Input.GetKey(KeyCode.LeftControl)) {
                    foreach(EntityBase e in this.world.entities.list) {
                        if(e is EntityWorker) {
                            PathfindingAgent agent = ((EntityWorker)e).moveHelper;
                            NavPath path = agent.calculatePath(pos);
                            if(path != null) {
                                agent.setPath(path);
                            }
                        }
                    }
                }

                if(Input.GetKey(KeyCode.Delete)) {
                    world.SetCell(pos, null);
                    world.LiftFog(pos);
                }

                if(Input.GetKey(KeyCode.F)) {
                    world.entities.Spawn(pos, 10);
                }
            }
        }
    }
}
