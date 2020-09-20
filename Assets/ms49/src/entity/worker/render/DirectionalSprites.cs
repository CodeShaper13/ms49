using UnityEngine;

[CreateAssetMenu(fileName = "Directional Sprites", menuName = "MS49/Directional Sprites", order = 1)]
public class DirectionalSprites : ScriptableObject {

    [SerializeField]
    private Sprite _front = null;
    [SerializeField]
    private Sprite _side = null;
    [SerializeField]
    private Sprite _back = null;

    public Sprite front => this._front;
    public Sprite side => this._side;
    public Sprite back => this._back;
}
