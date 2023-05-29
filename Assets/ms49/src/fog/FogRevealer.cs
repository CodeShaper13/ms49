using UnityEngine;

[RequireComponent(typeof(EntityBase))]
public class FogRevealer : MonoBehaviour {

    private Vector2Int cellPosLastFrame;
    private EntityBase entity;

    private void Awake() {
        this.entity = this.GetComponent<EntityBase>();
    }

    private void Start() {
        this.liftFog();
    }

    private void Update() {
        if(Pause.isPaused()) {
            return;
        }

        Vector2Int currentPos = this.entity.GetCellPos();
        if(currentPos != this.cellPosLastFrame) {
            this.liftFog();
        }

        this.cellPosLastFrame = currentPos;
    }

    private void liftFog() {
        this.entity.world.LiftFog(new Position(this.entity));
    }
}
