using UnityEngine;

public class EntityBat : EntityMonster {

    [SerializeField]
    private AiManager aiManager = null;
    [SerializeField]
    private PathfindingAgent agent = null;

    public override void Update() {
        if(Pause.IsPaused) {
            return;
        }

        base.Update();

        this.aiManager.updateAi();
    }
}
