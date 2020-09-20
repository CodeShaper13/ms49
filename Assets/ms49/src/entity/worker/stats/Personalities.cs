using UnityEngine;

public class Personalities : MonoBehaviour {

    [SerializeField]
    private TextAsset personalitiesTextAsset = null;

    private string[] personalities;

    private void Awake() {
        this.personalities = Names.readTextAsset(this.personalitiesTextAsset, true).ToArray();
    }

    public int getPersonalityCount() {
        return this.personalities.Length;
    }

    public string getDescription(int id) {
        return this.personalities[Mathf.Clamp(id, 0, this.personalities.Length - 1)];
    }
}
