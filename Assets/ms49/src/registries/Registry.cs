using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry<T> : ScriptableObject, IEnumerable<T> {

    [SerializeField]
    private T[] elements;

    public int RegistrySize => this.elements.Length;

    /// <summary>
    /// Returns the element at the passed location.
    /// If the location is out of range, null is returned.
    /// </summary>
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

    public IEnumerator<T> GetEnumerator() {
        for(int i = 0; i < this.elements.Length; i++) {
            yield return this.elements[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.elements.GetEnumerator();
    }

    [Button]
    private void CheckForDuplicates() {
        bool noDuplicates = true;

        for(int i = 0; i < this.RegistrySize; i++) {
            T element = this[i];
            if(element == null) {
                continue;
            }

            for(int j = 0; j < this.RegistrySize; j++) {
                if(i == j) {
                    continue;
                }

                if(element.Equals(this[j])) {
                    noDuplicates = false;
                    Debug.LogErrorFormat("there is a duplicate entry \"{0}\" at {1} and {2}!", element, i, j);
                }
            }
        }

        if(noDuplicates) {
            Debug.Log("No duplicates found!");
        }
    }
}