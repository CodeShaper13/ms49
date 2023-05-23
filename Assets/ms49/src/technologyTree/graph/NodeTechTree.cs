using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class NodeTechTree : Node {

    private const string DEFAULT = "default";

    [SerializeField, Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
    private Empty _parent;
    [SerializeField, Output(backingValue = ShowBackingValue.Never)]
    private Empty _children;

    [HorizontalLine]

    [SerializeField]
    private string _name = string.Empty;
    [SerializeField, Multiline]
    private string _description = string.Empty;
    [SerializeField]
    private Sprite _image = null;

    [SerializeField]
    private UnlockCost[] _costs;

    [Space]

    [SerializeField]
    private List<BuildableBase> _unlockedBuildable = null;
    [SerializeField, AllowNesting]
    private List<Field> _unlockedFields = null;

    /// <summary>
    /// The name to use when saving the technology to disk.
    /// </summary>
    public string SaveName => this.name;
    /// <summary>
    /// The pretty display name to use when showing the technology in interfaces.
    /// </summary>
    public string TechName => this._name;
    public string TechDescription => this._description;
    public Sprite Image => this._image;
    public UnlockCost[] Costs => this._costs; // TODO protect modifying at runtime.
    public List<BuildableBase> UnlockedBuildables => this._unlockedBuildable;
    public List<Field> UnlockedFields => this._unlockedFields;

    //////////////////
    // Runtime Data //
    //////////////////
    private bool isUnlocked;

    public bool IsUnlocked {
        get => this.name == DEFAULT ? true : this.isUnlocked;
        set {
            if(this.name != DEFAULT) {
                this.isUnlocked = value;
            }
        }
    }
    //////////////////
    

    public override object GetValue(NodePort port) {
        if(port.fieldName == "_parent") {
            return GetInputValue("_children", this._children);
        } else {
            return null;
        }
    }

    [Serializable]
    public class UnlockCost {

        public Item item;
        [Min(1)]
        public int cost;
    }

    [Serializable]
    public class Field {

        public Varaible variable;

        [Label("Value"), AllowNesting, ShowIf(nameof(__isBool))]
        public bool boolValue;
        [Label("Value"), AllowNesting, ShowIf(nameof(__isInt))]
        public int intValue;
        [Label("Value"), AllowNesting, ShowIf(nameof(__isFloat))]
        public float floatValue;
        [Label("Value"), AllowNesting, ShowIf(nameof(__isString))]
        public string stringValue;

        private bool __isBool => this.variable is BoolVariable;
        private bool __isInt => this.variable is IntVariable;
        private bool __isFloat => this.variable is FloatVariable;
        private bool __isString => this.variable is StringVariable;

        public void Apply() {
            if(this.variable is BoolVariable v) {
                v.value = this.boolValue;
            } else if(this.variable is IntVariable i) {
                i.value = this.intValue;
            } else if(this.variable is FloatVariable f) {
                f.value = this.intValue;
            } else if(this.variable is StringVariable s) {
                s.value = this.stringValue;
            }
        }
    }

    [Serializable]
    public class Empty { }
}