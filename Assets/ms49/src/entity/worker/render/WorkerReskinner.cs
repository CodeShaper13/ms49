using UnityEngine;
using System;

public class WorkerReskinner : MonoBehaviour {

    private WorkerType workerType = null;

    private void Start() {
        this.workerType = this.GetComponentInParent<EntityWorker>().type;
    }

    private void LateUpdate() {
        if(this.workerType != null) {
            Sprite[] replacements = this.workerType.getReplacementSprites();
            if(replacements != null) {
                foreach(SpriteRenderer sr in this.GetComponentsInChildren<SpriteRenderer>()) {
                    if(sr.sprite != null) {
                        string spriteName = sr.sprite.name;

                        Sprite newSprite = Array.Find(replacements, Item => Item.name == spriteName);

                        if(newSprite != null) {
                            sr.sprite = newSprite;
                        }
                    }
                }
            }
        }
    }
}
