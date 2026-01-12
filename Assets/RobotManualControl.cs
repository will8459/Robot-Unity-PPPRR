using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotManualControl : MonoBehaviour
{
    [Header("Hiérarchie Robot (Empty Objects)")]
    public Transform origine_0; 
    public Transform origine_1; // Prismatique 1z
    public Transform origine_2; // Prismatique 2x
    public Transform origine_3; // Prismatique 3y
    public Transform origine_4; // Rotoide 1x (O3=O4)
    public Transform origine_5; // Rotoide 2z (O5)
    public Transform pointe_E;  

    [Header("Interface UI")]
    public Slider slider_q1; public Slider slider_q2; public Slider slider_q3;
    public Slider slider_q4; public Slider slider_q5;
    public Button btn_reset;
    public TextMeshProUGUI txt_comparaison;

    [Header("Paramètres DH")]
    public float D1 = 1f; public float D2 = 0.5f; public float D3 = 0.5f; public float A = 0.8f;

    void Start() {
        // Supprimez manuellement les Audio Listeners sur Camera_diagonale et Camera_arriere
        btn_reset.onClick.AddListener(ResetRobot);
        ResetRobot();
    }

    void Update() {
        ApplyMovements();
        UpdateComparison();
    }

    public void ResetRobot() {
        slider_q1.value = 0f; slider_q2.value = 4.5f;
        slider_q3.value = 2f; slider_q4.value = 0f; slider_q5.value = 0f;
    }

    void ApplyMovements() {
        // 1. Translations (Inversion q3 demandée)
        origine_1.localPosition = new Vector3(0, 0, slider_q1.value);
        origine_2.localPosition = new Vector3(slider_q2.value, 0, 0);
        origine_3.localPosition = new Vector3(0, -slider_q3.value, 0);

        // 2. Rotoïdes
        origine_4.localRotation = Quaternion.Euler(slider_q4.value, 0, 0);

        // 3. O5 décalé de D2 par rapport à O4 (verticalement vers le bas dans votre schéma)
        origine_5.localRotation = Quaternion.Euler(0, 0, slider_q5.value);
    }

    void UpdateComparison() {
        float q1 = slider_q1.value; float q2 = slider_q2.value; 
        float q3 = -slider_q3.value; // Utilisation de la valeur inversée pour le MGD
        float q4 = slider_q4.value * Mathf.Deg2Rad; float q5 = slider_q5.value * Mathf.Deg2Rad;
        
        float C4 = Mathf.Cos(q4); float S4 = Mathf.Sin(q4);
        float C5 = Mathf.Cos(q5); float S5 = Mathf.Sin(q5);

        float Px_rapport = D1 + q3 + D3 * C4 + A * S4 * C5;
        float Py_rapport = q2 - A * S5;
        float Pz_rapport = q1 + D2 - D3 * S4 + A * C4 * C5;

        // MAPPING AXES UNITY : X=Py, Y=Px, Z=Pz
        Vector3 mgdUnity = new Vector3(Py_rapport, Px_rapport, Pz_rapport);

        // Position réelle dans Unity par rapport à Origine_0
        Vector3 localPosReal = origine_0.InverseTransformPoint(pointe_E.position);

        txt_comparaison.text = $"<b>Comparaison Coordonnées</b>\n" +
                               $"Unity (Réel) : {localPosReal.x:F2}, {localPosReal.y:F2}, {localPosReal.z:F2}\n" +
                               $"MGD (Mapped) : {mgdUnity.x:F2}, {mgdUnity.y:F2}, {mgdUnity.z:F2}\n" +
                               $"Écart : {Vector3.Distance(localPosReal, mgdUnity):F4}";
    }
}