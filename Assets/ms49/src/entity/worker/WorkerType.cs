using UnityEngine;

[CreateAssetMenu(fileName = "Worker Type", menuName = "MS49/Worker Type", order = 1)]
public class WorkerType : ScriptableObject, ISerializationCallbackReceiver {

    private const string PATH = "characterTextures/";

    [SerializeField]
    public string _typeName = null;
    [SerializeField, Tooltip("Optional")]
    public DirectionalSprites _hat = null;
    [SerializeField, Tooltip("Optional")]
    private string _spriteSheetName = null;
    [SerializeField, Tooltip("Optional")]
    private GameObject _aiPrefab = null;

    public string typeName => this._typeName;
    public DirectionalSprites hat => this._hat;
    public GameObject aiPrefab => this._aiPrefab;

    private Sprite[] replacementSprites;

    public void OnAfterDeserialize() {
        this.replacementSprites = null; // Clear the chached value
    }

    public void OnBeforeSerialize() { }

    public Sprite[] getReplacementSprites() {
        if(this.replacementSprites == null && !string.IsNullOrEmpty(this._spriteSheetName)) {
            // Try and lookup the sprites
            this.replacementSprites = Resources.LoadAll<Sprite>(PATH + this._spriteSheetName);
            if(this.replacementSprites == null) {
                Debug.LogWarning("Could not find sprite sheet at " + PATH + this._spriteSheetName);
            }
        }

        return this.replacementSprites;
    }
}
