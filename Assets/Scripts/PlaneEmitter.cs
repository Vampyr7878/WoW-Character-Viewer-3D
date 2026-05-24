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
        Gizmos.DrawWireCube(transform.position, new Vector3(emissionWidth, 0.001f, emissionLength));
        Vector3 forward = transform.TransformDirection(Vector3.forward.normalized);
        Gizmos.DrawRay(transform.position, forward * 2f);
    }
}