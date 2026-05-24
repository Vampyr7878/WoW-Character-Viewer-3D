using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PointEmitter : Emitter
{
    // Calulcate spawn position for particle
    protected override Vector3 GetSpawnPoint()
    {
        return transform.position;
    }

    // Draw gizmo to visualize emission
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 forward = transform.TransformDirection(Vector3.forward.normalized);
        Gizmos.DrawRay(transform.position, forward * 2f);
    }
}