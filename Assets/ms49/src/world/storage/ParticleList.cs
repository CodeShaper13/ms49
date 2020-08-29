using UnityEngine;
using System.Collections.Generic;

public class ParticleList : MonoBehaviour {

    [SerializeField]
    private World world = null;

    public List<Particle> list { get; private set; }
    private Transform particleHolder;
    private WorldRenderer worldRenderer;

    private void Awake() {
        this.list = new List<Particle>();

        this.particleHolder = this.world.createHolder("PARTICLE_HOLDER");
    }

    private void Start() {
        this.worldRenderer = GameObject.FindObjectOfType<WorldRenderer>();
    }

    private void Update() {
        if(!Pause.isPaused()) {
            for(int i = this.list.Count - 1; i >= 0; i--) {
                this.list[i].onUpdate();
            }
        }
    }

    private void LateUpdate() {
        foreach(Particle particle in this.list) {
            if(particle.depth == this.worldRenderer.targetLayer.depth) {
                particle.gameObject.SetActive(true);
            }
            else {
                particle.gameObject.SetActive(false);
            }
        }
    }

    public void spawn(Position pos, GameObject particlePrefab) {
        GameObject obj = GameObject.Instantiate(particlePrefab, this.particleHolder);
        Particle particle = obj.GetComponent<Particle>();
        if(particle == null) {
            Debug.LogWarning("Tried to create a particle from a prefab without a Particle Componenet!");
            GameObject.Destroy(obj.gameObject);
        }
        else {
            particle.transform.position = pos.vec2;
            particle.initialize(this.world, pos.depth);
            this.list.Add(particle);
        }
    }

    public void remove(Particle particle) {
        this.list.Remove(particle);
        particle.onEnd();
        GameObject.Destroy(particle.gameObject);
    }
}
