using UnityEngine;
using UnityEngine.UI;

public class TextWorkers : MonoBehaviour {

    private string prefix;

    [SerializeField]
    private Text text = null;
    private World world;

    private void Awake() {
        this.prefix = this.text.text;
        this.world = GameObject.FindObjectOfType<World>();
    }

    private void Update() {
        this.text.text = "TODO"; // this.prefix + " " + this.world.getMinerCount();
    }
}
