using System;

[Serializable]
public class PersonalityValueInt : PersonalityValue<int> {

    public PersonalityValueInt(int defaultValue) : base(defaultValue) { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(PersonalityValueInt))]
    public class PersonalityValueIntDrawer : PersonalityValueDrawer { }
#endif
}
