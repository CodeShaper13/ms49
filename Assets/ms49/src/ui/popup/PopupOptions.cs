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

    private Options options;
    private List<GameObject> optionControls;

    protected override void initialize() {
        base.initialize();

        this.options = Main.instance.options;

        // Create Ui Control elements for every option.
        foreach(IOption option in this.options.allOptions) {
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
        this.options.saveOptions();
    }
}
