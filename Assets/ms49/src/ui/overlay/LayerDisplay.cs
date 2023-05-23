using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerDisplay : MonoBehaviour {

    [Header("Editor Settings")]
    [SerializeField]
    private LayerData[] _layers = null;
    [SerializeField]
    private GameObject _layerUiPrefab = null;
    [SerializeField]
    private RectTransform _parent = null;

    private List<Toggle> toggles;
    private World world;

    private void Awake() {
        this.toggles = new List<Toggle>(this.GetComponentsInChildren<Toggle>(false));
        this.toggles.Reverse();
    }

    private void OnEnable() {
        this.world = Main.instance.activeWorld;
    }

    public void LateUpdate() {
        CameraController cc = CameraController.instance;
        for(int i = 0; i < this.toggles.Count; i++) {
            Toggle toggle = this.toggles[i];
            bool isUnlocked = this.world.IsDepthUnlocked(i);
            //toggle.gameObject.SetActive(isUnlocked);
            toggle.SetIsOnWithoutNotify(i == cc.currentLayer);
            toggle.GetComponent<Image>().color = isUnlocked ?
                this._layers[i].TintColor :
                Color.black;
        }
    }

    public void Callback_ChangeLayer(int layer) {
        CameraController cc = CameraController.instance;
        if(cc != null && this.world.IsDepthUnlocked(layer)) {
            cc.changeLayer(layer);
        }
    }

    [Button]
    private void CreateElements() {
        for(int i = this._layers.Length - 1; i >= 0; i--) {
            LayerData layer = this._layers[i];

            GameObject go = GameObject.Instantiate(this._layerUiPrefab, this._parent);
            go.gameObject.SetActive(true);
            //go.GetComponent<Image>().color = layer.TintColor;
            foreach(Image img in go.GetComponentsInChildren<Image>()) {
                img.color = layer.TintColor;
            }
        }
    }

    [Button]
    private void RemoveElements() {
        foreach(Transform t in this._parent) {
            if(t.gameObject.activeSelf) { // Don't destory the "prefab".
                GameObject.DestroyImmediate(t.gameObject);
            }
        }
    }
}