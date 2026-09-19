using UnityEngine;

/// <summary>
/// Dibuja una línea que anticipa la trayectoria parabólica del próximo disparo,
/// usando los mismos valores de fuerza y masa que va a usar Weapon al disparar.
///
/// Es sólo una VISTA PREVIA: calcula la caída libre ideal (sin colisiones),
/// con la misma fórmula física que ya usa Unity: posición = pos0 + v0*t + 1/2*g*t².
/// No hace falta entenderla a fondo, sólo saber que "resolution" es cuántos
/// puntitos tiene la línea y "timeStep" cuánto tiempo simulado hay entre cada uno.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] int resolution = 30;      // cantidad de puntos de la línea
    [SerializeField] float timeStep = 0.1f;    // segundos simulados entre puntos
    [SerializeField] float groundY = 0f;       // altura del suelo, para cortar la línea ahí

    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
    }

    /// <summary>
    /// Recalcula y redibuja la línea. Llamala cada vez que cambie
    /// ángulo, giro, fuerza o masa (Weapon ya lo hace automáticamente).
    /// </summary>
    public void UpdatePreview(float force, float mass)
    {
        Vector3 startPos = spawnPoint.position;

        // Misma cuenta que usa Weapon al disparar: Impulse / masa = velocidad inicial.
        Vector3 startVelocity = spawnPoint.forward * (force / mass);
        Vector3 gravity = Physics.gravity;

        Vector3[] points = new Vector3[resolution];
        int usedPoints = 0;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPos + startVelocity * t + 0.5f * gravity * t * t;

            points[usedPoints] = point;
            usedPoints++;

            if (point.y <= groundY)
                break;
        }

        line.positionCount = usedPoints;

        Vector3[] trimmedPoints = new Vector3[usedPoints];
        System.Array.Copy(points, trimmedPoints, usedPoints);
        line.SetPositions(trimmedPoints);
    }
}
