using UnityEngine;
using UnityEngine.Tilemaps;

public class CommandToggleFog : CommandBase {

    public override string runCommand(World world, string[] args) {
        TilemapRenderer tr;

        FogRenderer fr = GameObject.FindObjectOfType<FogRenderer>();
        if(fr != null) {
            tr = fr.GetComponentInChildren<TilemapRenderer>();
        } else {
            return "Could not find Component FogRenderer";
        }

        if(tr != null) {
            tr.enabled = !tr.enabled;
            return "toggling fog visibility";
        } else {
            return "Could not find Fog Tilemap";
        }
    }
}