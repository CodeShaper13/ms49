using UnityEngine;

[CreateAssetMenu(fileName = "Registry", menuName = "MS49/Registry/Personality", order = 1)]
public class PersonalityRegistry : Registry<Personality> {

    [Space]

    [SerializeField, Tooltip("If null, the first personality in the registry is used.")]
    private Personality _defaultPersonality = null;

    public Personality GetDefaultPersonality() {
        if(this._defaultPersonality != null) {
            return this._defaultPersonality;
        } else {
            for(int i = 0; i < this.RegistrySize; i++) {
                Personality personality = this.GetElement(i);
                if(personality != null) {
                    return personality;
                }
            }

            Debug.LogError("Returning null, brace for errors.");
            return null;
        }
    }
}
