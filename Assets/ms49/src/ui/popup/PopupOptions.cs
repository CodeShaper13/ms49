using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupOptions : PopupWindow {

    [SerializeField]
    private RectTransform area = null;
    [SerializeField]
    private GameObject sliderPrefab = null;
    [SerializeField]
    private GameObject togglePrefab = null;
    [SerializeField]
    private Options _options = null;

    private List<GameObject> optionControls;

    private void Start() {
        // Create Ui Control elements for every option.
        foreach(IOption option in this._options.allOptions) {
            GameObject prefab = this.getPrefab(option);

            if(prefab != null) {
                GameObject obj = GameObject.Instantiate(prefab, this.area);

                obj.GetComponentInChildren<Text>().text = option.name;

                option.setupControlObj(obj);
            }
        }
    }

    private GameObject getPrefab(IOption option) {
        if(option is OptionSlider) {
            return this.sliderPrefab;
        }
        else if(option is OptionToggle) {
            return this.togglePrefab;
        }

        return null;
    }

    public void callback_saveOptions() {
        this._options.saveOptions();
    }
}
