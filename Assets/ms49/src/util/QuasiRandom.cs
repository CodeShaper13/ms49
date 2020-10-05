using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuasiRandom {

    public static List<Vector2> poisson_disk_sampling(int k, int r, int size) {
        List<Vector2> samples = new List<Vector2>();
        List<Vector2> active_list = new List<Vector2>();
        active_list.Add(new Vector2(random(size), random(size)));

        int len;
        while((len = active_list.Count) > 0) {
            // picks random index uniformly at random from the active list
            int index = (int)random(len);
            Swap(active_list, len - 1, index);
            Vector2 sample = active_list[len - 1];
            bool found = false;
            for(int i = 0; i < k; ++i) {
                // generates a point uniformly at random in the sample's
                // disk situated at a distance from r to 2*r 
                float angle = 2 * Mathf.PI * random(1);
                float radius = random(r) + r;
                Vector2 dv = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
                Vector2 new_sample = dv + sample;

                bool ok = true;
                for(int j = 0; j < samples.Count; ++j) {
                    if(Vector2.Distance(new_sample, samples[j]) <= r) {
                        ok = false;
                        break;
                    }
                }
                if(ok) {
                    if(0 <= new_sample.x && new_sample.x < size &&
                        0 <= new_sample.y && new_sample.y < size) {
                        samples.Add(new_sample);
                        active_list.Add(new_sample);
                        len++;
                        found = true;
                    }
                }
            }
            if(!found) {
                active_list.RemoveAt(active_list.Count - 1);
            }
        }

        return samples;
    }

    private static void Swap<T>(List<T> list, int indexA, int indexB) {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    private static float random(float max) {
        return UnityEngine.Random.Range(0, max);
    }


    List<Vector2> samples;

    void setup() {
        int SIZE = 500;

        samples = poisson_disk_sampling(30, 10, SIZE);
    }
}
