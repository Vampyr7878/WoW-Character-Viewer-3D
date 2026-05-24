using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class Emitter : MonoBehaviour
{
    // Min particle speed
    public float minSpeed;
    // Max particle speed
    public float maxSpeed;
    // Bertical speed range
    public float verticalRange;
    // Horizontal speed range
    public float horizontalRange;
    // How much gravity affects particle
    public float gravity;
    // Min partcile lifespan
    public float minLifespan;
    // Max particle lifespan
    public float maxLifespan;
    // Min particle emission rate
    public float minEmission;
    // Max particle emission rate
    public float maxEmission;
    // Length of particle emission
    public float emissionLength;
    // Eidth of particle emission
    public float emissionWidth;

    // Partcile system
    private ParticleSystem ps;
    // Timer to emit particles
    private float emissionTimer;
    // Current emission rate
    private float currentEmission;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        var emission = ps.emission;
        emission.enabled = false;
        var shape = ps.shape;
        shape.enabled = false;
        var main = ps.main;
        main.loop = false;
        main.playOnAwake = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        emissionTimer = 0;
        ps.Play();
    }

    void Update()
    {
        emissionTimer += currentEmission == 0 ? minEmission : currentEmission * Time.deltaTime;
        while (emissionTimer >= 1f)
        {
            emissionTimer -= 1f;
            EmitParticle();
            currentEmission = Random.Range(minEmission, maxEmission);
        }
        ApplyGravity();
    }

    // Emit 1 particle
    void EmitParticle()
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        float yaw = Random.Range(-horizontalRange, horizontalRange);
        float pitch = Random.Range(-verticalRange, verticalRange);
        Quaternion rotation = Quaternion.Euler(Mathf.Rad2Deg * pitch, Mathf.Rad2Deg * yaw, 0f);
        Vector3 direction = rotation * Vector3.forward.normalized;
        direction = transform.TransformDirection(direction);
        Vector3 velocity = direction * speed;
        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
        emit.position = GetSpawnPoint();
        emit.velocity = velocity;
        emit.startLifetime = Random.Range(minLifespan, maxLifespan);
        emit.startSize = 1f;
        emit.rotation = 0f;
        ps.Emit(emit, 1);
    }

    // Apply gravity to the particles
    void ApplyGravity()
    {
        int count = ps.particleCount;
        if (count == 0)
        {
            return;
        }
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[count];
        ps.GetParticles(particles);
        float dt = Time.deltaTime;
        for (int i = 0; i < count; i++)
        {
            particles[i].velocity += dt * gravity * Vector3.up;
        }
        ps.SetParticles(particles, count);
    }

    public void Play()
    {
        ps.Clear();
        ps.Play();
    }

    // Calulcate spawn position for particle
    protected abstract Vector3 GetSpawnPoint();

    // Draw gizmo to visualize emission
    protected abstract void OnDrawGizmosSelected();
}