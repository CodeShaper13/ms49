using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class ParticleList : MonoBehaviour {

    [SerializeField]
    private World world = null;

    private List<Particle> allParticles;

    private void Awake() {
        this.allParticles = new List<Particle>();
    }

    private void LateUpdate() {
        int depthBeingRendered = this.world.worldRenderer.getDepthRendering();
        foreach(Particle particle in this.allParticles) {
            particle.gameObject.SetActive(particle.depth == depthBeingRendered);
        }
    }

    public Particle Spawn(Position pos, GameObject particlePrefab) {
        return this.Spawn(pos.AsVec2, pos.depth, particlePrefab);
    }

    public Particle Spawn(Vector2 pos, int depth, GameObject particlePrefab) {
        GameObject obj = GameObject.Instantiate(particlePrefab, this.transform);
        Particle particle = obj.GetComponent<Particle>();
        if(particle == null) {
            Debug.LogWarning("Can not spawn Particle, it has no Particle componenet on root GameObject");
            GameObject.Destroy(obj.gameObject);
            return null;
        }
        else if(particle.GetComponentInChildren<ParticleSystem>() == null) {
            Debug.LogWarning("Can not spawn Particle, it has no ParticleSystem component on it or it's children");
            return null;
        }
        else {
            particle.transform.position = pos;
            particle.Initialize(this.world, depth);
            this.allParticles.Add(particle);
            return particle;
        }
    }

    public void Remove(Particle particle) {
        this.allParticles.Remove(particle);
        particle.OnEnd();
        GameObject.Destroy(particle.gameObject);
    }

    [Button]
    private void RemoveAllParticles() {
        if(this.allParticles == null) {
            for(int i = this.allParticles.Count - 1; i >= 0; i--) {
                this.Remove(this.allParticles[i]);
            }
        }
    }
}
