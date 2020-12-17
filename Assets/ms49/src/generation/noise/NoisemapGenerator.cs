using UnityEngine;
using System.Collections;

public static class NoisemapGenerator {

    public static float[,] computeNoiseMap(NoiseSettings settings, int seed, int mapSize, int layer) {
        float[,] noiseMap = new float[mapSize, mapSize];

        System.Random prng = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        for(int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + (layer * 1000);
            float offsetY = prng.Next(-100000, 100000) + (layer * 1000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < settings.octaves; i++) {
                    float sampleX = x / settings.scale * frequency + octaveOffsets[i].x;
                    float sampleY = y / settings.scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);// * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the values.
        for(int y = 0; y < mapSize; y++) {
            for(int x = 0; x < mapSize; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
