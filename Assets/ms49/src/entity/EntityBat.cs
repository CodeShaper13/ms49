using UnityEngine;
using System.Collections;

public class EntityBat : EntityMonster {

    [SerializeField]
    private AiManager aiManager = null;
    [SerializeField]
    private PathfindingAgent agent;

    public override void onUpdate() {
        base.onUpdate();

        this.aiManager.updateAi();
        this.agent.update();
    }
}
