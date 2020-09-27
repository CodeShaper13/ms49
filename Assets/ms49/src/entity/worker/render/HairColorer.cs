using UnityEngine;

[RequireComponent(typeof(DirectionalSpriteSwapper))]
public class HairColorer : MonoBehaviour {

    private void Start() {
        EntityWorker worker = this.GetComponentInParent<EntityWorker>();

        int style = worker.info.hairStyle;
        this.GetComponent<DirectionalSpriteSwapper>().sprites =
            Main.instance.workerFactory.getHairSpritesFromId(style);
    }
}
