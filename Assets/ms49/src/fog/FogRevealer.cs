using UnityEngine;

[RequireComponent(typeof(EntityBase))]
public class FogRevealer : MonoBehaviour {
    
    [SerializeField]
    [Min(0)]
    private int revealDistance = 3;

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

        Vector2Int currentPos = this.entity.getCellPos();
        if(currentPos != this.cellPosLastFrame) {
            this.liftFog();
        }

        this.cellPosLastFrame = currentPos;
    }

    private void liftFog() {
        this.entity.world.liftFog(new Position(this.entity), this.revealDistance);
    }
}
