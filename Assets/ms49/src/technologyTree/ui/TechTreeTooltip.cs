using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechTreeTooltip : MonoBehaviour {

    [SerializeField]
    private TMP_Text _textTitle = null;
    [SerializeField]
    private TMP_Text _textDescription = null;
    [SerializeField]
    private RectTransform _unlockedIconParent = null;
    [SerializeField]
    private GameObject _prefab = null;

    private List<GameObject> objs;

    private void Awake() {
        this.objs = new List<GameObject>();
    }

    /// <summary>
    /// Sets the target Node ot display information about.  It is
    /// safe to pass null.
    /// </summary>
    public void SetNode(NodeTechTree node) {
        // Clear out the old Buildables
        foreach(GameObject go in this.objs) {
            GameObject.Destroy(go);
        }
        this.objs.Clear();


        if(node == null) {
            this.SetText("nul", "nul");
        } else {
            this.SetText(node.TechName, node.TechDescription);

            List<BuildableBase> list = node.UnlockedBuildables;
            this._unlockedIconParent.gameObject.SetActive(list.Count > 0);        
            if(list.Count > 0) {
                foreach(var buildable in list) {
                    GameObject go = GameObject.Instantiate(this._prefab, this._unlockedIconParent);
                    go.gameObject.SetActive(true);
                    go.GetComponentInChildren<BuildableUiRenderer>().SetBuildable(buildable);

                    this.objs.Add(go);
                }
            }
        }
    }

    private void SetText(string name, string description) {
        if(this._textTitle != null) {
            this._textTitle.text = name;
        }

        if(this._textDescription != null) {
            this._textDescription.text = description;
        }
    }
}