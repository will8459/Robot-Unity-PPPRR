using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RobotController : MonoBehaviour
{
    [Header("Hiérarchie Robot")]
    public Transform origine_0; 
    public Transform origine_1, origine_2, origine_3, origine_4, origine_5, pointe_E;
    private TrailRenderer traceur;

    [Header("Interface UI")]
    public Slider[] sliders; 
    public Button btn_line, btn_circle, btn_stop, btn_reset;
    public TextMeshProUGUI txt_coords;

    [Header("Paramètres DH")]
    public float D1 = 1f; public float D2 = 0.5f; public float D3 = 0.5f; public float A = 0.8f;

    private float q3_UP = 4.0f;
    private float q3_DOWN = 5.5f;
    
    private bool isAuto = false;
    private Coroutine currentRoutine;

    void Start() {
        traceur = pointe_E.GetComponent<TrailRenderer>();
        if(traceur) traceur.emitting = false;

        btn_line.onClick.AddListener(() => StartTask(DrawLineSequence()));
        btn_circle.onClick.AddListener(() => StartTask(DrawCircleSequence()));
        btn_stop.onClick.AddListener(StopTask);
        btn_reset.onClick.AddListener(ResetRobot);
    }

    void Update() {
        ApplyMovements();
        UpdateInterface();
    }

    void SolvePlanarMGI(Vector3 targetP) {
        float q2_val = targetP.x;      
        float q1_val = targetP.z - D2; 

        if (sliders.Length >= 2) {
            sliders[0].value = q1_val; 
            sliders[1].value = q2_val; 
        }
    }

    IEnumerator DrawLineSequence() {
        isAuto = true;
        Vector3 startPos = new Vector3(4f, 0f, 0f); 
        Vector3 endPos   = new Vector3(8f, 0f, 2f);

        sliders[2].value = q3_UP;
        yield return MoveToPlanar(startPos, 1.0f);

        float t = 0;
        while(t < 1f) {
            t += Time.deltaTime * 2; 
            sliders[2].value = Mathf.Lerp(q3_UP, q3_DOWN, t);
            yield return null;
        }
        sliders[2].value = q3_DOWN;

        if (traceur) traceur.emitting = true;
        yield return MoveToPlanar(endPos, 2.0f);
        if (traceur) traceur.emitting = false;

        t = 0;
        while(t < 1f) {
            t += Time.deltaTime * 2;
            sliders[2].value = Mathf.Lerp(q3_DOWN, 4.5f, t);
            yield return null;
        }
        sliders[2].value = 4.5f;
        
        isAuto = false;
    }

    IEnumerator DrawCircleSequence() {
        isAuto = true;
        Vector3 center = new Vector3(6f, 0f, 0f);
        float radius = 1.5f;
        Vector3 startCircle = center + new Vector3(radius, 0, 0);

        sliders[2].value = q3_UP;
        yield return MoveToPlanar(startCircle, 1.0f);

        float duration = 0.5f;
        float elapsed = 0;
        while(elapsed < duration) {
            elapsed += Time.deltaTime;
            sliders[2].value = Mathf.Lerp(q3_UP, q3_DOWN, elapsed/duration);
            yield return null;
        }
        sliders[2].value = q3_DOWN;

        if (traceur) traceur.emitting = true;
        float t = 0;
        while (t < 1f) {
            t += Time.deltaTime * 0.2f; 
            float angle = t * Mathf.PI * 2;
            Vector3 target = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            
            SolvePlanarMGI(target);
            yield return null;
        }
        if (traceur) traceur.emitting = false;

        elapsed = 0;
        while(elapsed < duration) {
            elapsed += Time.deltaTime;
            sliders[2].value = Mathf.Lerp(q3_DOWN, 4.5f, elapsed/duration);
            yield return null;
        }
        sliders[2].value = 4.5f;
        
        isAuto = false;
    }

    IEnumerator MoveToPlanar(Vector3 targetXZ, float duration) {
        Vector3 currentPos = new Vector3(sliders[1].value, 0, sliders[0].value + D2); 

        float elapsed = 0;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            Vector3 nextP = Vector3.Lerp(currentPos, targetXZ, elapsed / duration);
            SolvePlanarMGI(nextP);
            yield return null;
        }
        SolvePlanarMGI(targetXZ);
    }

    void ApplyMovements() {
        if(origine_1) origine_1.localPosition = new Vector3(0, 0, sliders[0].value);
        
        if(origine_2) origine_2.localPosition = new Vector3(sliders[1].value, D1, 0); 

        if(origine_3) origine_3.localPosition = new Vector3(0, -sliders[2].value, 0);

        // Rotations
        if(origine_4) origine_4.localRotation = Quaternion.Euler(sliders[3].value, 0, 0);
        if(origine_5) origine_5.localRotation = Quaternion.Euler(0, 0, sliders[4].value);
    }

    void UpdateInterface() {
        if(pointe_E && txt_coords) {
            Vector3 pos = origine_0.InverseTransformPoint(pointe_E.position);
            txt_coords.text = $"<b>{(isAuto ? "AUTO" : "MANUEL")}</b>\n" +
                              $"Pos (X,Y,Z): {pos.x:F2}, {pos.y:F2}, {pos.z:F2}\n" +
                              $"q3 (Slider): {sliders[2].value:F2}";
        }
    }

    void StartTask(IEnumerator routine) {
        StopTask();
        currentRoutine = StartCoroutine(routine);
    }
    void StopTask() {
        isAuto = false;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        if (traceur) traceur.emitting = false;
    }
    void ResetRobot() {
        StopTask();
        sliders[0].value = 0; sliders[1].value = 4.5f; sliders[2].value = 4.5f;
        sliders[3].value = 0; sliders[4].value = 0;
    }
}