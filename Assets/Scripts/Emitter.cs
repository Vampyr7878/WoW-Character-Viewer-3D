using M2Lib;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class Emitter : MonoBehaviour
{
    // Reference to the parten model
    public ModelRenderer model;
    // Spin both ways
    public bool spinBothWays;
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
    // Width of particle emission
    public float emissionWidth;
    // Z Source
    public float Source;
    // Partcile blinking speed
    public float twinkleSpeed;
    // Particle blinking percentage
    public float twinklePercent;
    // Particle blinking min size
    public float twinkleMin;
    // Particle blinking max size
    public float twinkleMax;
    // Min particle orientation
    public float minOrienation;
    // Max particle orientation
    public float maxOrienation;
    // Min particle spin
    public float minSpin;
    // Max particle spin
    public float maxSpin;

    // Partcile system
    private ParticleSystem ps;
    // M2Particle structure for variable properties
    private M2Particle m2Particle;
    // Current frames for properties
    private Dictionary<string, int> m2Frames;
    // Timers to change properties
    private Dictionary<string, float> m2Timers;
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
        currentEmission = Random.Range(minEmission, maxEmission);
        emissionTimer += currentEmission * Time.deltaTime;
        while (emissionTimer >= 1f)
        {
            emissionTimer -= 1f;
            EmitParticle();
        }
        ApplyGravity();
        Twinkle();
        if (model.Loaded)
        {
            UpdateProperties();
        }
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
        ParticleSystem.EmitParams emit = new()
        {
            position = GetSpawnPoint(),
            velocity = velocity,
            startLifetime = Random.Range(minLifespan, maxLifespan),
            startSize = 1f,
            rotation = Random.Range(minOrienation, maxOrienation) * Mathf.Rad2Deg,
            angularVelocity = (Random.Range(0, 2) == 0 ? Random.Range(minSpin, maxSpin) : Random.Range(-maxSpin, -minSpin)) * Mathf.Rad2Deg,
        };
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

    // Twinkle
    void Twinkle()
    {
        int count = ps.particleCount;
        if (count == 0)
        {
            return;
        }
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[count];
        ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            float age = particles[i].startLifetime - particles[i].remainingLifetime;
            float seed = (particles[i].randomSeed & 0xFFFF) / 65535f;
            float phase = Mathf.Repeat(seed + age * twinkleSpeed, 1f);
            bool visible = phase < twinklePercent;
            var color = particles[i].startColor;
            color.a = (byte)(visible ? 255 : 0);
            particles[i].startColor = color;
            if (visible)
            {
                float scale = Mathf.Lerp(twinkleMin, twinkleMax, seed);
                particles[i].startSize = scale;
            }
            else
            {
                particles[i].startSize = 0f;
            }
        }
        ps.SetParticles(particles, count);
    }
    
    // Update properties using particle tracks
    void UpdateProperties()
    {
        float speed = UpdateProperty(m2Particle.Speed, "Speed");
        float speedVariation = UpdateProperty(m2Particle.SpeedVariation, "SpeedVariation");
        float vertical = UpdateProperty(m2Particle.VerticalRange, "VerticalRange");
        float horizontal = UpdateProperty(m2Particle.HorizontalRange, "HorizontalRange");
        float currentGravity = UpdateProperty(m2Particle.Gravity, "Gravity");
        float lifespan = UpdateProperty(m2Particle.Lifespan, "Lifespan");
        float emissionRate = UpdateProperty(m2Particle.EmissionRate, "EmissionRate");
        float Length = UpdateProperty(m2Particle.EmissionLength, "EmissionLength");
        float Width = UpdateProperty(m2Particle.EmissionWidth, "EmissionWidth");
        minSpeed = (speed * (1f - speedVariation)) / 2f;
        maxSpeed = (speed * (1f + speedVariation)) / 2f;
        verticalRange = vertical;
        horizontalRange = horizontal;
        gravity = Gravity(currentGravity);
        minLifespan = (lifespan - m2Particle.LifespanVariation) / 2f;
        maxLifespan = (lifespan + m2Particle.LifespanVariation) / 2f;
        minEmission = emissionRate - m2Particle.EmissionVariation;
        maxEmission = emissionRate + m2Particle.EmissionVariation;
        emissionLength = Length;
        emissionWidth = Width;
    }

    // Update specific property using particle tracks
    float UpdateProperty(ParticleTrack track, string key)
    {
        float property = track.Values[0][m2Frames[key]];
        if (track.Timestamps[0].Length > 1)
        {
            m2Timers[key] += Time.deltaTime;
            if (m2Timers[key] >= track.Timestamps[0][m2Frames[key]] / 1000f)
            {
                m2Frames[key]++;
                if (m2Frames[key] == track.Timestamps[0].Length)
                {
                    m2Frames[key] = 0;
                    m2Timers[key] = 0f;
                }
            }
            if (m2Frames[key] > 0)
            {
                property = track.Values[0][m2Frames[key] - 1];
            }
        }
        return property;
    }

    // Set correct gravity value
    float Gravity(float value)
    {
        if ((m2Particle.Flags & 0x800000) == 0)
        {
            return value;
        }
        byte[] bytes = System.BitConverter.GetBytes(value);
        byte x = bytes[0], y = bytes[1];
        short z = System.BitConverter.ToInt16(bytes, 2);
        Vector3 direction = new Vector3(x, y, 0f) * (1f / 128f);
        float result = Mathf.Sqrt(1f - Vector3.Dot(direction, direction));
        float mag = z * 0.04238648f;
        if(mag < 0)
        {
            result = -result;
            mag = -mag;
        }
        direction.z = result;
        direction *= mag;
        return direction.z;
    }
    
    // Set properties using M2Particle
    public void Setup(M2Particle particle)
    {
        m2Particle = particle;
        m2Frames = new Dictionary<string, int>
        {
            ["Speed"] = 0,
            ["SpeedVariation"] = 0,
            ["VerticalRange"] = 0,
            ["HorizontalRange"] = 0,
            ["Gravity"] = 0,
            ["Lifespan"] = 0,
            ["EmissionRate"] = 0,
            ["EmissionLength"] = 0,
            ["EmissionWidth"] = 0
        };
        m2Timers = new Dictionary<string, float>
        {
            ["Speed"] = 0f,
            ["SpeedVariation"] = 0f,
            ["VerticalRange"] = 0f,
            ["HorizontalRange"] = 0f,
            ["Gravity"] = 0f,
            ["Lifespan"] = 0f,
            ["EmissionRate"] = 0f,
            ["EmissionLength"] = 0f,
            ["EmissionWidth"] = 0f
        };
        spinBothWays = (particle.Flags & 0x200) != 0;
        minSpeed = (particle.Speed.Values[0][0] * (1f - particle.SpeedVariation.Values[0][0])) / 2f;
        maxSpeed = (particle.Speed.Values[0][0] * (1f + particle.SpeedVariation.Values[0][0])) / 2f;
        verticalRange = particle.VerticalRange.Values[0][0];
        horizontalRange = particle.HorizontalRange.Values[0][0];
        gravity = Gravity(particle.Gravity.Values[0][0]);
        minLifespan = (particle.Lifespan.Values[0][0] - particle.LifespanVariation) / 2f;
        maxLifespan = (particle.Lifespan.Values[0][0] + particle.LifespanVariation) / 2f;
        minEmission = particle.EmissionRate.Values[0][0] - particle.EmissionVariation;
        maxEmission = particle.EmissionRate.Values[0][0] + particle.EmissionVariation;
        emissionLength = particle.EmissionLength.Values[0][0];
        emissionWidth = particle.EmissionWidth.Values[0][0];
        twinkleSpeed = particle.TwinkleSpeed;
        twinklePercent = particle.TwinklePercent;
        twinkleMin = particle.TwinkleMin;
        twinkleMax = particle.TwinkleMax;
        minOrienation = particle.Orientation - particle.OrientationVariation;
        maxOrienation = particle.Orientation + particle.OrientationVariation;
        minSpin = particle.Spin - particle.SpinVariation;
        maxSpin = particle.Spin + particle.SpinVariation;
    }

    // Calulcate spawn position for particle
    protected abstract Vector3 GetSpawnPoint();

    // Draw gizmo to visualize emission
    protected abstract void OnDrawGizmosSelected();
}