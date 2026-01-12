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
    public Transform origine_4; // Rotoide 1x 
    public Transform origine_5; // Rotoide 2z
    public Transform pointe_E;  

    [Header("Interface UI")]
    public Slider slider_q1; public Slider slider_q2; public Slider slider_q3;
    public Slider slider_q4; public Slider slider_q5;
    public Button btn_reset;
    public TextMeshProUGUI txt_comparaison;

    [Header("Paramètres DH")]
    public float D1 = 1f; public float D2 = 0.5f; public float D3 = 0.5f; public float A = 0.8f;

    void Start() {
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
        origine_1.localPosition = new Vector3(0, 0, slider_q1.value);
        origine_2.localPosition = new Vector3(slider_q2.value, 0, 0);
        origine_3.localPosition = new Vector3(0, -slider_q3.value, 0);

        origine_4.localRotation = Quaternion.Euler(slider_q4.value, 0, 0);

        origine_5.localRotation = Quaternion.Euler(0, 0, slider_q5.value);
    }

    void UpdateComparison() {
        float q1 = slider_q1.value; float q2 = slider_q2.value; 
        float q3 = -slider_q3.value;
        float q4 = slider_q4.value * Mathf.Deg2Rad; float q5 = slider_q5.value * Mathf.Deg2Rad;
        
        float C4 = Mathf.Cos(q4); float S4 = Mathf.Sin(q4);
        float C5 = Mathf.Cos(q5); float S5 = Mathf.Sin(q5);

        float Px_rapport = D1 + q3 + D3 * C4 + A * S4 * C5;
        float Py_rapport = q2 - A * S5;
        float Pz_rapport = q1 + D2 - D3 * S4 + A * C4 * C5;

        Vector3 mgdUnity = new Vector3(Py_rapport, Px_rapport, Pz_rapport);
        
        Vector3 localPosReal = origine_0.InverseTransformPoint(pointe_E.position);

        txt_comparaison.text = $"<b>Comparaison Coordonnées</b>\n" +
                               $"Unity (Réel) : {localPosReal.x:F2}, {localPosReal.y:F2}, {localPosReal.z:F2}\n" +
                               $"MGD (Calculé) : {mgdUnity.x:F2}, {mgdUnity.y:F2}, {mgdUnity.z:F2}\n" +
                               $"Écart : {Vector3.Distance(localPosReal, mgdUnity):F4}";
    }
}