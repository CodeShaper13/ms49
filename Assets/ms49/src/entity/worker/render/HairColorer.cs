using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HairColorer : MonoBehaviour {

    [SerializeField]
    private Color[] hairColors = new Color[0];

    private void Start() {
        EntityWorker worker = this.GetComponentInParent<EntityWorker>();

        int hairColor = worker.info.hairColor;
        this.GetComponent<SpriteRenderer>().color =
            this.hairColors[Mathf.Clamp(hairColor, 0, this.hairColors.Length)];
    }
}
