using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SkinColorer : MonoBehaviour {

    [SerializeField]
    private Color[] skinTones = null;

    private void Start() {
        EntityWorker worker = this.GetComponentInParent<EntityWorker>();

        int skinTone = worker.info.skinTone;
        this.GetComponent<SpriteRenderer>().color = this.getColorFromTone(skinTone);
    }

    public Color getColorFromTone(int tone) {
        return this.skinTones[Mathf.Clamp(tone, 0, this.skinTones.Length -1)];
    }
}
