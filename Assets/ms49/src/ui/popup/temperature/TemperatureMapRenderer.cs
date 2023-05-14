using UnityEngine;
using UnityEngine.Tilemaps;

public class TemperatureMapRenderer : MonoBehaviour {

    [SerializeField]
    private Tilemap tilemap = null;
    [SerializeField]
    private Tile tile = null;
    [SerializeField]
    private ColorVariable colorLow = null;
    [SerializeField]
    private ColorVariable colorMedium = null;
    [SerializeField]
    private ColorVariable colorHigh = null;
    [SerializeField]
    private PopupTemperature popup = null;

    private bool isInitialized = false;
    private float[] oldTemps;

    public void initalSetup(Layer layer) {
        this.oldTemps = new float[layer.size * layer.size];

        for(int x = 0; x < layer.size; x++) {
            for(int y = 0; y < layer.size; y++) {
                this.oldTemps[layer.size * x + y] = float.PositiveInfinity;

                Vector3Int v = new Vector3Int(x, y, 0);
                this.tilemap.SetTile(v, this.tile);
                this.tilemap.SetTileFlags(v, TileFlags.None);
            }
        }
    }

    private void Update() {
        Layer layer = this.popup.world.storage.GetLayer(CameraController.instance.currentLayer);

        if(!this.isInitialized) {
            this.initalSetup(layer);
            this.isInitialized = true;
        }

        for(int x = 0; x < layer.size; x++) {
            for(int y = 0; y < layer.size; y++) {
                float temp = layer.GetTemperature(x, y);

                if(this.oldTemps[layer.size * x + y] != temp) {
                    this.oldTemps[layer.size * x + y] = temp;

                    Color color = temperatureToRGB(temp);

                    this.tilemap.SetColor(new Vector3Int(x, y, 0), color);
                }
            }
        }
    }

    private Color temperatureToRGB(float temperature) {
        if(temperature <= 0f) {
            return Color.Lerp(this.colorLow, this.colorMedium, temperature + 1);
        } else {
            return Color.Lerp(this.colorMedium, this.colorHigh, temperature);
        }
    }
}
