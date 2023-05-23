﻿using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TechTreeTechnology : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public TMP_Text textNodeName;
    public Image img;
    public UILineRenderer line;
    public GameObject costLabel;
    public TechTreeTooltip tooltip;

    [Space]

    [SerializeField, Required]
    private Image _imgBackground = null;
    [SerializeField]
    private Sprite _spriteLocked = null;
    [SerializeField]
    private Sprite _spriteAvailable = null;
    [SerializeField]
    private Sprite _spriteUnlocked = null;

    public Color lineColorLocked;
    public Color lineColorUnlocked;

    public NodeTechTree Node { get; private set; }

    private void OnValidate() {
        if(this.line != null) {
            this.line.color = this.lineColorLocked;
        }
    }

    // TODO make this not run every frame.
    private void Update() {
        this.Refresh(Main.instance.activeWorld.technologyTree);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(this.tooltip != null) {
            this.tooltip.gameObject.SetActive(true);
            this.tooltip.transform.position = this.transform.position;
            this.tooltip.SetNode(this.Node);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(this.tooltip != null) {
            this.tooltip.gameObject.SetActive(false);
        }
    }

    public void SetNode(NodeTechTree node) {
        this.Node = node;

        if(this.textNodeName != null) {
            this.textNodeName.text = node.TechName;
        }

        if(this.img != null) {
            this.img.sprite = node.Image;
        }

        if(this.costLabel != null) {
            if(node.Costs.Length == 0) {
                this.costLabel.gameObject.SetActive(false);
            } else {
                for(int i = 0; i < node.Costs.Length; i++) {
                    GameObject go;
                    if(i == 0) {
                        go = this.costLabel;
                    } else {
                        go = GameObject.Instantiate(this.costLabel, this.costLabel.transform.parent);
                    }

                    var unlockCost = node.Costs[i];
                    if(unlockCost.item != null) {
                        go.GetComponent<Image>().sprite = unlockCost.item.sprite;
                    }
                    go.GetComponentInChildren<TMP_Text>().text = string.Format("x{0}", unlockCost.cost);
                }
            }
        }
    }

    public void Refresh(TechnologyTree techTree) {
        if(this.Node != null) {
            bool isUnlocked = techTree.IsTechnologyUnlocked(this.Node);

            // Line color.
            if(this.line != null) {
                this.line.color = isUnlocked ? this.lineColorUnlocked : this.lineColorLocked;
            }

            // Sprite.
            Sprite sprite;
            if(isUnlocked) {
                sprite = this._spriteUnlocked;
            }
            else if(techTree.IsTechnologyAvailable(this.Node)) {
                sprite = this._spriteAvailable;
            }
            else {
                sprite = this._spriteLocked;
            }
            this._imgBackground.sprite = sprite;
        }
    }

    public void SetupLine(Transform lineEnd) {
        if(this.line != null) {
            if(lineEnd == null) {
                this.line.gameObject.SetActive(false);
            } else {
                this.line.gameObject.SetActive(true);
                Vector3 end = this.transform.InverseTransformPoint(lineEnd.transform.position + Vector3.up * 160);
                this.line.Points = new Vector2[] {
                    Vector2.zero,
                    new Vector2(0, end.y / 2),
                    new Vector2(end.x, end.y / 2),
                    end,
                };
            }
        }
    }
}