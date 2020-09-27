using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SkinColorer : MonoBehaviour {

    private void Start() {
        EntityWorker worker = this.GetComponentInParent<EntityWorker>();

        int skinTone = worker.info.skinTone;
        this.GetComponent<SpriteRenderer>().color =
            Main.instance.workerFactory.getSkinColorFromTone(skinTone);
    }
}
