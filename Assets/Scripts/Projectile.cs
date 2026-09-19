using UnityEngine;

/// <summary>
/// Mide el vuelo del proyectil y arma el ShotData en el primer impacto.
/// El movimiento lo resuelve íntegramente el Rigidbody.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 12f;

    ShotManager manager;
    ShotData data;

    Vector3 startPosition;
    float launchTime;
    float maxHeight;
    bool hasImpacted;

    /// <summary>
    /// Llamado por Weapon inmediatamente después de aplicar el impulso.
    /// </summary>
    public void Launch(float angle, float force, float mass, ShotManager shotManager)
    {
        manager = shotManager;
        startPosition = transform.position;
        launchTime = Time.time;
        maxHeight = 0f;

        data = new ShotData
        {
            angle = angle,
            force = force,
            mass = mass
        };

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        float height = transform.position.y - startPosition.y;
        if (height > maxHeight)
            maxHeight = height;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted || data == null)
            return;

        hasImpacted = true;

        data.flightTime = Time.time - launchTime;
        data.impactPoint = collision.GetContact(0).point;
        data.relativeVelocity = collision.relativeVelocity.magnitude;
        data.collisionImpulse = collision.impulse.magnitude;
        data.maxHeight = maxHeight;
        data.hitTarget = collision.gameObject.GetComponentInParent<TargetPiece>() != null;

        // Alcance horizontal: distancia en el plano XZ, sin contar la altura.
        Vector3 from = new Vector3(startPosition.x, 0f, startPosition.z);
        Vector3 to = new Vector3(data.impactPoint.x, 0f, data.impactPoint.z);
        data.horizontalRange = Vector3.Distance(from, to);

        if (manager != null)
            manager.RegisterImpact(data);
    }
}
