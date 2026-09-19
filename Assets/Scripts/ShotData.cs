using UnityEngine;

/// <summary>
/// Datos registrados de un intento de tiro.
/// El Projectile llena la parte física, el ShotManager completa
/// las piezas derribadas y el puntaje.
/// </summary>
[System.Serializable]
public class ShotData
{
    // Parámetros de entrada
    public float angle;
    public float force;
    public float mass;

    // Resultado de la simulación
    public float flightTime;
    public Vector3 impactPoint;
    public float horizontalRange;
    public float maxHeight;
    public float relativeVelocity;
    public float collisionImpulse;
    public bool hitTarget;

    // Evaluación
    public int piecesKnocked;
    public int score;
}
