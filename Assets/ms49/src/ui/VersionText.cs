using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionText : MonoBehaviour {

    private void Awake() {
        this.GetComponent<Text>().text = "version: " + Application.version;
    }
}
