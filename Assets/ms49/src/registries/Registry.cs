using System;
using System.Collections.Generic;
using UnityEngine;

public class Registry<T> : ScriptableObject {

    [SerializeField]
    private T[] elements;

    public int RegistrySize => this.elements.Length;

    public T this[int index] {
        get {
            if(index < 0 || index >= this.elements.Length) {
                Debug.LogWarningFormat("Index \"{0}\" is out of range", index);
                return default;
            }
            return this.elements[index];
        } set {
            this.elements[index] = value;
        }
    }

    /// <summary>
    /// Returns the element at the passed location.
    /// If the location is out of range, null is returned.
    /// </summary>
    [Obsolete("Use index operator instead.")]
    public virtual T GetElement(int location) {
        if(location < 0 || location >= this.elements.Length) {
            return default;
        }
        return this.elements[location];
    }

    /// <summary>
    /// Returns the id of the passed element in the registry.
    /// If the passed element is not in the registry, -1 is returned.
    /// </summary>
    public virtual int GetIdOfElement(T other) {
        for(int i = 0; i < this.elements.Length; i++) {
            if(EqualityComparer<T>.Default.Equals(this.elements[i], other)) {
                return i;
            }
        }
        return -1;
    }
}