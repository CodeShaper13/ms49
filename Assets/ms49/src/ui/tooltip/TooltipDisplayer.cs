﻿using UnityEngine;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {

    [SerializeField]
    private Text textHeldItemName = null;
    [SerializeField]
    private Image img = null;

    private void Start() {
        this.setText(string.Empty);
        this.textHeldItemName.text = string.Empty;
    }

    private void Update() {
        this.transform.position = Input.mousePosition;
    }

    public void setText(string text) {
        if(this.img != null) {
            this.img.enabled = !string.IsNullOrEmpty(text);
        }
        if(this.textHeldItemName != null) {
            this.textHeldItemName.text = text;
        }
    }
}
