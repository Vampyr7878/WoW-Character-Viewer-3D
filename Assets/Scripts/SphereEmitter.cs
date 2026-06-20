using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SphereEmitter : Emitter
{
    // Calulcate spawn position for particle
    protected override Vector3 GetSpawnPoint()
    {
        Vector2 direction = Random.onUnitCircle;
        float radius = Random.Range(emissionLength, emissionWidth);
        Vector3 localSpawn = new(direction.x * radius, direction.y * radius, 0f);
        return transform.TransformPoint(localSpawn);
    }

    // Draw gizmo to visualize emission
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (emissionLength > 0)
        {
            Gizmos.DrawWireSphere(Vector3.zero, emissionLength);
        }
        if (emissionWidth > 0)
        {
            Gizmos.DrawWireSphere(Vector3.zero, emissionWidth);
        }
        Vector3 direction = Quaternion.Euler(-verticalRange * Mathf.Rad2Deg, -horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(Vector3.zero, direction * 5f);
        direction = Quaternion.Euler(-verticalRange * Mathf.Rad2Deg, horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(Vector3.zero, direction * 5f);
        direction = Quaternion.Euler(verticalRange * Mathf.Rad2Deg, horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(Vector3.zero, direction * 5f);
        direction = Quaternion.Euler(verticalRange * Mathf.Rad2Deg, -horizontalRange * Mathf.Rad2Deg, 0f) * Vector3.forward;
        Gizmos.DrawRay(Vector3.zero, direction * 5f);
        Gizmos.matrix = oldMatrix;
        direction = transform.TransformDirection(Vector3.forward.normalized);
        Gizmos.DrawRay(transform.position, direction * 5f);
    }
}