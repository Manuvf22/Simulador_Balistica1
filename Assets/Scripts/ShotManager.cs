using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Evalúa cada intento: espera a que la estructura se estabilice,
/// cuenta las piezas derribadas, calcula el puntaje y muestra el reporte.
/// </summary>
public class ShotManager : MonoBehaviour
{
    [Header("Objetivos")]
    [SerializeField] List<TargetPiece> pieces = new List<TargetPiece>();

    [Header("Tiempos")]
    [SerializeField] float settleTime = 3f;    // espera tras el impacto
    [SerializeField] float maxShotTime = 15f;  // corte si el tiro nunca impacta

    [Header("UI")]
    [SerializeField] TextMeshProUGUI reportText;
    [SerializeField] TextMeshProUGUI totalScoreText;

    [Header("Puntaje")]
    [SerializeField] int targetHitBonus = 50;    // impacto directo sobre una pieza
    [SerializeField] float impulseFactor = 2f;   // puntos por unidad de impulso

    public bool IsShotInProgress { get; private set; }

    readonly HashSet<TargetPiece> counted = new HashSet<TargetPiece>();
    int totalScore;
    int shotCount;
    bool impactRegistered;

    void Start()
    {
        // Siempre buscamos las piezas solos, sin importar lo que haya
        // cargado a mano en el Inspector. Esto evita que un campo tocado
        // por error (sobre todo durante Play) deje la lista vacía para
        // siempre, ya que Start() sólo corre una vez.
        pieces.Clear();
        pieces.AddRange(FindObjectsByType<TargetPiece>(FindObjectsSortMode.None));

        Debug.Log("[DIAGNOSTICO] ShotManager encontró " + pieces.Count + " piezas al iniciar.");

        UpdateTotals();

        if (reportText != null)
            reportText.text = "Ajustá ángulo, fuerza y masa.\nESPACIO para disparar.";
    }

    public void BeginShot()
    {
        IsShotInProgress = true;
        impactRegistered = false;
        shotCount++;

        if (reportText != null)
            reportText.text = "Tiro #" + shotCount + " en curso...";

        StartCoroutine(ShotTimeout());
    }

    public void RegisterImpact(ShotData data)
    {
        if (impactRegistered)
            return;

        impactRegistered = true;
        StartCoroutine(EvaluateShot(data));
    }

    IEnumerator ShotTimeout()
    {
        yield return new WaitForSeconds(maxShotTime);

        if (!impactRegistered && IsShotInProgress)
        {
            if (reportText != null)
                reportText.text = "Tiro #" + shotCount + "\nSin impacto registrado.";

            IsShotInProgress = false;
        }
    }

    IEnumerator EvaluateShot(ShotData data)
    {
        // Le damos tiempo a la estructura para terminar de caer.
        yield return new WaitForSeconds(settleTime);

        int piecePoints = 0;

        foreach (TargetPiece piece in pieces)
        {
            if (piece == null || counted.Contains(piece))
                continue;

            if (piece.IsKnockedDown())
            {
                counted.Add(piece);
                data.piecesKnocked++;
                piecePoints += piece.Points;
            }
        }

        data.score = piecePoints
            + (data.hitTarget ? targetHitBonus : 0)
            + Mathf.RoundToInt(data.collisionImpulse * impulseFactor);

        totalScore += data.score;

        ShowReport(data);
        UpdateTotals();

        IsShotInProgress = false;
    }

    void ShowReport(ShotData d)
    {
        string report =
            "REPORTE DE TIRO #" + shotCount + "\n" +
            "Ángulo: " + d.angle.ToString("F1") + "°   " +
            "Fuerza: " + d.force.ToString("F1") + "   " +
            "Masa: " + d.mass.ToString("F1") + " kg\n" +
            "Tiempo de vuelo: " + d.flightTime.ToString("F2") + " s\n" +
            "Punto de impacto: " + d.impactPoint.ToString("F2") + "\n" +
            "Alcance horizontal: " + d.horizontalRange.ToString("F2") + " m\n" +
            "Altura máxima: " + d.maxHeight.ToString("F2") + " m\n" +
            "Velocidad relativa: " + d.relativeVelocity.ToString("F2") + " m/s\n" +
            "Impulso de colisión: " + d.collisionImpulse.ToString("F2") + " N·s\n" +
            "Impacto directo: " + (d.hitTarget ? "sí" : "no") + "\n" +
           
            "PUNTAJE: " + d.score;

        Debug.Log(report);

        if (reportText != null)
            reportText.text = report;
    }

    void UpdateTotals()
    {
        if (totalScoreText != null)
            totalScoreText.text = "Total: " + totalScore + "   Tiros: " + shotCount;
    }

    /// <summary>
    /// Botón "Reiniciar objetivos": devuelve las piezas a su pose original.
    /// </summary>
    public void ResetTargets()
    {
        Debug.Log("[DIAGNOSTICO] ResetTargets ejecutado. La lista tiene " + pieces.Count + " piezas.");

        foreach (TargetPiece piece in pieces)
        {
            if (piece != null)
            {
                Debug.Log("[DIAGNOSTICO] Reseteando " + piece.name);
                piece.ResetPiece();
            }
            else
            {
                Debug.Log("[DIAGNOSTICO] Elemento nulo en la lista, se saltea.");
            }
        }

        counted.Clear();

        if (reportText != null)
            reportText.text = "Objetivos reiniciados.";
    }
}