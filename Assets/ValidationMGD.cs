using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValidationMGD : MonoBehaviour
{
    public Transform q1_Z; 
    public Transform q2_X; 
    public Transform q3_Y;
    public Transform q4_RotX;
    public Transform q5_RotZ;
    public Transform pointe_E; 

    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;
    public Slider slider5;

    public float D1 = 1.0f;
    public float D2 = 0.5f;
    public float D3 = 0.5f;

    public TMP_Text affichageTexte; 

    public RobotLigneAuto autoScript;

    private Vector3 startPos1, startPos2, startPos3;
    private Quaternion startRot4, startRot5;

    void Start()
    {
        // On mémorise la position/rotation définie dans l'inspecteur au lancement
        if (q1_Z) startPos1 = q1_Z.localPosition;
        if (q2_X) startPos2 = q2_X.localPosition;
        if (q3_Y) startPos3 = q3_Y.localPosition;
        
        if (q4_RotX) startRot4 = q4_RotX.localRotation;
        if (q5_RotZ) startRot5 = q5_RotZ.localRotation;

        // Calcule l'offset relatif entre le pivot de rotation et la pointe
        Vector3 offsetFinal = q4_RotX.InverseTransformPoint(pointe_E.position);
        Debug.Log("D2 probable (X) : " + offsetFinal.x);
        Debug.Log("D3 probable (Z) : " + offsetFinal.z);
    }

    void Update()
    {

        if (autoScript != null && autoScript.IsMoving)
        {
            // MODE AUTO : On synchronise les Sliders sur la position réelle du robot
            // pour éviter que le robot "saute" quand on repasse en manuel.
            slider1.value = q1_Z.localPosition.z;
            slider2.value = q2_X.localPosition.x;
            slider3.value = q3_Y.localPosition.y;
            // Note: pour les angles, il faudrait convertir startRot * q4 en float
            return; // On arrête l'Update ici pour ne pas écraser la position
        }
        
        float q1 = slider1.value;
        float q2 = slider2.value;
        float q3 = slider3.value;
        float q4 = slider4.value;
        float q5 = slider5.value;

        // Application des mouvements RELATIFS à la position d'origine
        // On garde les X et Y d'origine et on ajoute q1 au Z d'origine
        q1_Z.localPosition = new Vector3(startPos1.x, startPos1.y, startPos1.z + q1);
        
        // On garde les Y et Z d'origine et on ajoute q2 au X d'origine
        q2_X.localPosition = new Vector3(startPos2.x + q2, startPos2.y, startPos2.z);
        
        // On garde les X et Z d'origine et on ajoute q3 au Y d'origine
        q3_Y.localPosition = new Vector3(startPos3.x, startPos3.y + q3, startPos3.z);

        // Pour les rotations, on multiplie la rotation initiale par la nouvelle rotation
        // L'ordre des multiplications de Quaternions est important (Initial * Offset)
        q4_RotX.localRotation = startRot4 * Quaternion.Euler(q4, 0, 0);
        q5_RotZ.localRotation = startRot5 * Quaternion.Euler(0, 0, q5);

        // 3. Calcul du Modèle Géométrique Direct (MGD)
        // Formules extraites de ton schéma
        float rad4 = q4 * Mathf.Deg2Rad;

        float px = D1 + q1 + Mathf.Cos(rad4) * D2 - Mathf.Sin(rad4) * D3;
        float py = q2;
        float pz = q3 + Mathf.Sin(rad4) * D2 + Mathf.Cos(rad4) * D3;

        // 4. Affichage des coordonnées pour la vérification
        Vector3 posUnity = pointe_E.position;
        affichageTexte.text = "Coordonnées Unity : " + posUnity.ToString("F2") + "\n" +
                              "Calcul MGD (U0) : (" + px.ToString("F2") + ", " + py.ToString("F2") + ", " + pz.ToString("F2") + ")";
    }
}