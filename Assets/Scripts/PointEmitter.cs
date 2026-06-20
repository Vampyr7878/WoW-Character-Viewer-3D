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
        Vector3 direction = transform.TransformDirection(Vector3.forward.normalized);
        Gizmos.DrawRay(transform.position, direction * 2f);
        direction = Quaternion.Euler(-verticalRange * Mathf.Rad2Deg, -horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(transform.position, direction * 5f);
        direction = Quaternion.Euler(-verticalRange * Mathf.Rad2Deg, horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(transform.position, direction * 5f);
        direction = Quaternion.Euler(verticalRange * Mathf.Rad2Deg, horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(transform.position, direction * 5f);
        direction = Quaternion.Euler(verticalRange * Mathf.Rad2Deg, -horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(transform.position, direction * 5f);
    }
}