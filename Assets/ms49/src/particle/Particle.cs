using UnityEngine;

public class Particle : MonoBehaviour {

    [Tooltip("How long, in seconds, the Particle Effect should be alive.  -1 will make it live forever.")]
    public float lifespan = 1f;

    public World world { get; private set; }
    public int depth { get; private set; }
    private float timeAlive;

    public virtual void initialize(World world, int depth) {
        this.world = world;
        this.depth = depth;
    }

    public virtual void onUpdate() {
        if(this.lifespan != -1) {
            this.timeAlive += Time.deltaTime;

            if(this.timeAlive > this.lifespan) {
                this.world.particles.remove(this);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position, 0.1f);
    }

    /// <summary>
    /// Called when the Particle is removed from the world when it's life runs out.
    /// It is NOT called when the application shuts down.
    /// </summary>
    public virtual void onEnd() {

    }
}
