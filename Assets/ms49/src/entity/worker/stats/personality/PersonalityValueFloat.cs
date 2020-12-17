using System;

[Serializable]
public class PersonalityValueFloat : PersonalityValue<float> {

    public PersonalityValueFloat(float defaultValue) : base(defaultValue) { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(PersonalityValueFloat))]
    public class PersonalityValueFloatDrawer : PersonalityValueDrawer {
    }
#endif
}

