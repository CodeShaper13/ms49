using UnityEngine;
using System;

public class SpriteReskinner : MonoBehaviour {

    private const string PATH = "characterTextures/";

    [SerializeField]
    private string spriteSheetName = null;

    private Sprite[] replacementSprites;

    private void Awake() {
        this.replacementSprites = Resources.LoadAll<Sprite>(PATH + this.spriteSheetName);
        if(this.replacementSprites == null) {
            Debug.LogWarning("Could not find sprite sheet at " + PATH + this.spriteSheetName);
        }
    }

    private void LateUpdate() {
        if(this.replacementSprites != null) {
            foreach(SpriteRenderer sr in this.GetComponentsInChildren<SpriteRenderer>()) {
                if(sr.sprite != null) {
                    string spriteName = sr.sprite.name;

                    Sprite newSprite = Array.Find(this.replacementSprites, Item => Item.name == spriteName);

                    if(newSprite != null) {
                        sr.sprite = newSprite;
                    }
                }
            }
        }
    }
}
