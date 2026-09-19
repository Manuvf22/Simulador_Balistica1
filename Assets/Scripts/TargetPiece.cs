using UnityEngine;

/// <summary>
/// Pieza de una estructura objetivo. Guarda su pose inicial y decide
/// si fue derribada comparando inclinación y desplazamiento.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class TargetPiece : MonoBehaviour
{
    [SerializeField] float tiltThreshold = 40f;   // grados de rotación respecto al inicio
    [SerializeField] float moveThreshold = 0.5f;  // metros de desplazamiento
    [SerializeField] int points = 100;

    Rigidbody rb;
    Vector3 initialPosition;
    Quaternion initialRotation;

    public int Points => points;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public bool IsKnockedDown()
    {
        float tilt = Quaternion.Angle(transform.rotation, initialRotation);
        float moved = Vector3.Distance(transform.position, initialPosition);
        return tilt > tiltThreshold || moved > moveThreshold;
    }

    /// <summary>
    /// Devuelve la pieza a su pose inicial y frena su movimiento.
    /// </summary>
    public void ResetPiece()
    {
        rb.linearVelocity = Vector3.zero;   // Unity 2022 o anterior: rb.velocity
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
    }
}
