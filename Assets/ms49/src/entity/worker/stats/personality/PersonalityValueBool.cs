using System;

[Serializable]
public class PersonalityValueBool : PersonalityValue<bool> {

    public PersonalityValueBool(bool defaultValue) : base(defaultValue) { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(PersonalityValueBool))]
    public class PersonalityValueBoolDrawer : PersonalityValueDrawer {
    }
#endif
}

