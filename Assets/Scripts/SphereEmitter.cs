using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SphereEmitter : Emitter
{
    // Calulcate spawn position for particle
    protected override Vector3 GetSpawnPoint()
    {
        Vector2 direction = Random.onUnitCircle;
        float radius = Random.Range(emissionLength, emissionWidth);
        Vector3 localSpawn = new Vector3(direction.x * radius, direction.y * radius, 0f);
        return transform.TransformPoint(localSpawn);
    }

    // Draw gizmo to visualize emission
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, emissionLength);
        Gizmos.DrawWireSphere(transform.position, emissionWidth);
        Vector3 forward = transform.TransformDirection(Vector3.forward.normalized);
        Gizmos.DrawRay(transform.position, forward * 2f);
    }
}