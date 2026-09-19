using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla la orientación del cañón (horizontal + vertical) y el disparo.
///
/// Idea simple: el cañón tiene DOS ejes de giro, como una torreta.
///   - verticalAngle: cuánto se levanta del piso (0° = horizontal, apunta al frente).
///   - horizontalAngle: hacia dónde gira mirando desde arriba (izquierda/derecha).
/// Cada uno viene de un Slider distinto. No se mezclan entre sí.
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform spawnPoint;

    [Header("Control vertical (arriba / abajo)")]
    [SerializeField] Slider angleSlider;
    [SerializeField] TextMeshProUGUI angleText;

    [Header("Control horizontal (izquierda / derecha)")]
    [SerializeField] Slider yawSlider;
    [SerializeField] TextMeshProUGUI yawText;

    [Header("Fuerza y masa")]
    [SerializeField] Slider forceSlider;
    [SerializeField] Slider massSlider;
    [SerializeField] TextMeshProUGUI forceText;
    [SerializeField] TextMeshProUGUI massText;

    [Header("Referencias")]
    [SerializeField] ShotManager shotManager;
    [SerializeField] TrajectoryPreview trajectoryPreview;

    void Start()
    {
        OnParametersChanged();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    /// <summary>
    /// Se conecta al evento On Value Changed de TODOS los Sliders
    /// (ángulo, giro, fuerza y masa). Sólo actualiza rotación y textos.
    /// </summary>
    public void OnParametersChanged()
    {
        float verticalAngle = angleSlider.value;     // 0 = horizontal, sube = apunta más arriba
        float horizontalAngle = yawSlider.value;      // gira el cañón a izquierda/derecha

        // Rotación en X = vertical, rotación en Y = horizontal.
        // El signo negativo en X es lo único "raro": es porque en Unity,
        // para que el cañón apunte hacia ARRIBA hay que rotar en X hacia
        // el lado NEGATIVO. Si alguna vez lo ves invertido, cambiá el signo.
        transform.localRotation = Quaternion.Euler(-verticalAngle, horizontalAngle, 0f);

        if (angleText != null)
            angleText.text = "Vertical: " + verticalAngle.ToString("F1") + "°";

        if (yawText != null)
            yawText.text = "Horizontal: " + horizontalAngle.ToString("F1") + "°";

        if (forceText != null)
            forceText.text = "Fuerza: " + forceSlider.value.ToString("F1");

        if (massText != null)
            massText.text = "Masa: " + massSlider.value.ToString("F1") + " kg";

        // La estela se recalcula con los mismos valores de fuerza y masa
        // que Fire() va a usar para el próximo disparo.
        if (trajectoryPreview != null)
            trajectoryPreview.UpdatePreview(forceSlider.value, massSlider.value);
    }

    /// <summary>
    /// Instancia el proyectil, le asigna la masa elegida y le aplica el impulso
    /// en la dirección hacia donde apunta el cañón (spawnPoint.forward).
    /// </summary>
    public void Fire()
    {
        if (shotManager != null && shotManager.IsShotInProgress)
            return;

        GameObject instance = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Rigidbody rb = instance.GetComponent<Rigidbody>();
        rb.mass = massSlider.value;
        rb.AddForce(spawnPoint.forward * forceSlider.value, ForceMode.Impulse);

        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.Launch(angleSlider.value, forceSlider.value, rb.mass, shotManager);

        if (shotManager != null)
            shotManager.BeginShot();
    }

    /// <summary>
    /// Vuelve los cuatro sliders a sus valores iniciales.
    /// Conectala al On Click () de un botón "Reset Settings".
    /// </summary>
    public void ResetSettings()
    {
        angleSlider.value = 45f;
        yawSlider.value = 0f;
        forceSlider.value = 25f;
        massSlider.value = 1f;

        // Por las dudas, forzamos la actualización de rotación y textos.
        OnParametersChanged();
    }
}