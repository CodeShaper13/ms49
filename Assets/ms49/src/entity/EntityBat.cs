using UnityEngine;

public class EntityBat : EntityMonster {

    [SerializeField]
    private AiManager aiManager = null;
    [SerializeField]
    private PathfindingAgent agent = null;

    public override void onUpdate() {
        base.onUpdate();

        this.aiManager.updateAi();
        this.agent.update();
    }
}
