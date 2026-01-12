using UnityEngine;
using System.Collections;

public class RobotAutoControl : MonoBehaviour
{
    [Header("Articulations (Pivots)")]
    public Transform q1_Z; public Transform q2_X; public Transform q3_Y;
    public Transform q4_RotX; public Transform q5_RotZ;

    [Header("Paramètres DH")]
    public float D1; public float D2; public float D3; public float A;

    private enum Mode { Idle, Line, Circle }
    private Mode currentMode = Mode.Idle;
    private float timer = 0f;

    [Header("Réglages Tracé")]
    public TrailRenderer traceur; 
    public float hauteurEcriture = 1.0f; // Ajustez cette valeur si le stylo ne touche pas le sol

    // Résolution du MGI (Modèle Géométrique Inverse)
    // On calcule q1, q2, q3 pour atteindre une position (Px, Py, Pz) désirée
    void SolveMGI(float targetPx, float targetPy, float targetPz, float q4, float q5)
    {
        float r4 = q4 * Mathf.Deg2Rad;
        float r5 = q5 * Mathf.Deg2Rad;

        // Équations dérivées par identification de la matrice U0
        // q2 = Py + A * sin(q5)
        float q2 = targetPy + (A * Mathf.Sin(r5));

        // q3 = Px - D1 - D3*cos(q4) - A*sin(q4)*cos(q5)
        float q3 = targetPx - D1 - (D3 * Mathf.Cos(r4)) - (A * Mathf.Sin(r4) * Mathf.Cos(r5));

        // q1 = Pz - D2 + D3*sin(q4) - A*cos(q4)*cos(q5)
        float q1 = targetPz - D2 + (D3 * Mathf.Sin(r4)) - (A * Mathf.Cos(r4) * Mathf.Cos(r5));

        // Application aux articulations
        q1_Z.localPosition = new Vector3(0, 0, q1);
        q2_X.localPosition = new Vector3(q2, D1, 0);
        q3_Y.localPosition = new Vector3(0, q3, D2);
        q4_RotX.localEulerAngles = new Vector3(q4, 0, 0);
        q5_RotZ.localEulerAngles = new Vector3(0, 0, q5);
    }

    void Update()
    {
        if (currentMode == Mode.Idle) {
            if (traceur != null) traceur.emitting = false; // Ne trace pas en mode manuel
            return;
        }

        timer += Time.deltaTime * 0.5f;
        traceur.emitting = true; // Active le tracé dès que l'automatique démarre

        if (currentMode == Mode.Line) {
            float x = Mathf.Lerp(0, 2, timer);
            float z = Mathf.Lerp(0, 2, timer);
            // On utilise hauteurEcriture pour Px (qui est l'axe Y vertical chez vous)
            SolveMGI(hauteurEcriture, x, z, 0, 0); 
        }
        else if (currentMode == Mode.Circle) {
            float radius = 1.0f;
            float x = radius * Mathf.Cos(timer * 2 * Mathf.PI);
            float z = radius * Mathf.Sin(timer * 2 * Mathf.PI);
            SolveMGI(hauteurEcriture, x, z, 0, 0);
        }

        // if (timer >= 1f) StopAutomation();
    }

    public void StopAutomation() { 
        currentMode = Mode.Idle; 
        if (traceur != null) traceur.emitting = false;
        GetComponent<RobotManualControl>().enabled = true; 
        ToggleManual(true); 
    }

    public void StartLine() { timer = 0; currentMode = Mode.Line; ToggleManual(false); }
    public void StartCircle() { timer = 0; currentMode = Mode.Circle; ToggleManual(false); }
    

    void ToggleManual(bool state) {
        GetComponent<RobotManualControl>().enabled = state;
    }
}