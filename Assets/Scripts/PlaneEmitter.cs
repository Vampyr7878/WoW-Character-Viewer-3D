using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PlaneEmitter : Emitter
{
    // Calulcate spawn position for particle
    protected override Vector3 GetSpawnPoint()
    {
        float x = Random.Range(-emissionWidth * 0.5f, emissionWidth * 0.5f);
        float y = Random.Range(-emissionLength * 0.5f, emissionLength * 0.5f);
        return transform.TransformPoint(new Vector3(x, y, 0f));
    }

    // Draw gizmo to visualize emission
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (emissionWidth > 0 && emissionLength > 0)
        {
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(emissionWidth, emissionLength, 0.001f));
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