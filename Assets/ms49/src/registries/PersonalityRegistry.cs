using UnityEngine;

public class PersonalityRegistry : RegistryBase<Personality> {

    [SerializeField]
    private int _defaultPersonalityId = 0;

    public int defaultPersonalityId => this._defaultPersonalityId;

    public Personality getDefaultPersonality() {
        return this.getElement(this._defaultPersonalityId);
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(PersonalityRegistry))]
    public class PersonalityRegistryEditor : RegistryBaseEditor { }
#endif
}
