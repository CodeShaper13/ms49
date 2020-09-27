using System;
using UnityEngine;
using UnityEngine.UI;

public class FaceUiPreview : MonoBehaviour {

    [SerializeField]
    private Image bodyImg = null;
    [SerializeField]
    private Image skinCutoutImg = null;
    [SerializeField]
    private Image hairImg = null;
    [SerializeField]
    private Image hatImg = null;

    private Sprite defaultBodySprite;

    private void Awake() {
        this.defaultBodySprite = this.bodyImg.sprite;
    }

    public void setTarget(EntityWorker worker) {
        this.setTarget(worker.info, worker.type);
    }

    public void setTarget(WorkerType type) {
        this.setTarget(null, type);
    }

    /// <summary>
    /// Safe to pass null for info parameter.
    /// </summary>
    public void setTarget(WorkerInfo info, WorkerType type) {
        // Body/Clothes
        Sprite s = this.tryGetSprite(type);
        if(s != null) {
            this.bodyImg.sprite = s;
        } else {
            this.bodyImg.sprite = this.defaultBodySprite;
        }

        // Skin
        Color c = Main.instance.workerFactory.getSkinColorFromTone(info == null ? 0 : info.skinTone);
        this.skinCutoutImg.color = c;

        // Hair
        this.hairImg.sprite = null; // TODO
        this.hairImg.color = Color.white; // TODO

        // Hat
        DirectionalSprites hat = type.hat;
        if(hat != null) {
            this.hatImg.enabled = true;
            this.hatImg.sprite = hat.front;
        } else {
            this.hatImg.enabled = false;
        }
    }

    private Sprite tryGetSprite(WorkerType type) {
        Sprite[] replacements = type.getReplacementSprites();
        if(replacements != null) {
            Sprite newSprite = Array.Find(
                replacements, Item => Item.name == this.defaultBodySprite.name);
            if(newSprite != null) {
                return newSprite;
            }
        }

        return null;
    }
}
