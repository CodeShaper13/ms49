using UnityEngine;

[CreateAssetMenu(fileName = "Personality", menuName = "MS49/Personality", order = 1)]
public class Personality : ScriptableObject {

    [SerializeField]
    private string _displayName = null;
    [SerializeField, Tooltip("The WorkerTypes that can't have this Personality.")]
    private WorkerType[] _notAllowedTypes = null;
    [SerializeField]
    private EnumPayModifier _payModifier = EnumPayModifier.NORMAL;

    [Space]

    [SerializeField]
    private PersonalityValueFloat _moveSpeedMultiplyer = new PersonalityValueFloat(1f);
    [SerializeField]
    private PersonalityValueFloat _workSpeedMultiplyer = new PersonalityValueFloat(1f);
    [SerializeField]
    private PersonalityValueInt _sleepStartOffset = new PersonalityValueInt(0);
    [SerializeField]
    private PersonalityValueInt _sleepStopOffset = new PersonalityValueInt(0);
    [SerializeField]
    private PersonalityValueFloat _energyLoseMultiplyer = new PersonalityValueFloat(1f); // Not used
    [SerializeField]
    private PersonalityValueFloat _energyGainMultiplyer = new PersonalityValueFloat(1f);
    [SerializeField]
    private PersonalityValueInt _eatStartOffset = new PersonalityValueInt(0);
    [SerializeField]
    private PersonalityValueInt _eatStopOffset = new PersonalityValueInt(0);
    [SerializeField]
    private PersonalityValueFloat _hungerLoseMultiplyer = new PersonalityValueFloat(1f); // Not used
    [SerializeField]
    private PersonalityValueBool _require2Meals = new PersonalityValueBool(false);
    [SerializeField]
    private PersonalityValueBool _isCareless = new PersonalityValueBool(false); // Cook implementation not done
    [SerializeField]
    private PersonalityValueFloat _combatDamageMultiplyer = new PersonalityValueFloat(1f); // Not used
    [SerializeField]
    private PersonalityValueFloat _stealPercent = new PersonalityValueFloat(0);

    // TODO buffs
    // TODO random stat increase/decrease
    // TODO doesn't run away from combat
    // TODO moody personality

    public string displayName => this._displayName;
    public EnumPayModifier payModifier => this._payModifier;

    public float moveSpeedMultiplyer => this._moveSpeedMultiplyer.value;
    public float workSpeedMultiplyer => this._workSpeedMultiplyer.value;
    public int sleepStartOffset => this._sleepStartOffset.value;
    public int sleepStopOffset => this._sleepStopOffset.value;
    public float energyLoseMultiplyer => this._energyLoseMultiplyer.value;
    public float energyGainMultiplyer => this._energyGainMultiplyer.value;
    public int eatStartOffset => this._eatStartOffset.value;
    public int eatStopOffset => this._eatStopOffset.value;
    public float hungerLoseMultiplyer => this._hungerLoseMultiplyer.value;
    public bool require2Meals => this._require2Meals.value;
    public bool isCareless => this._isCareless.value;
    public float combatDamageMultiplyer => this._combatDamageMultiplyer.value;
    public float stealPercent => this._stealPercent.value;

    /// <summary>
    /// Checks if the passed Worker type can have this personality.
    /// </summary>
    public bool canHave(WorkerType type) {
        foreach(WorkerType t in this._notAllowedTypes) {
            if(type == t) {
                return false;
            }
        }

        return true;
    }
}
