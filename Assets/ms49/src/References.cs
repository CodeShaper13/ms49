using UnityEngine;

public class References : MonoBehaviour {

    public static References list;

    [Header("Text")]
    public TextAsset maleNames;
    public TextAsset femaleNames;
    public TextAsset lastNames;

    public static void bootstrap() {
        References.list = GameObject.FindObjectOfType<References>();
    }
}
