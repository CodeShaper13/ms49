using UnityEngine;

public static class SpriteToTexture {

    public static Texture2D convert(Sprite sprite) {
        Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] newColors = sprite.texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height);

        newText.SetPixels(newColors);
        newText.Apply();
        return newText;
    }
}
