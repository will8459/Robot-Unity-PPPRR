using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class RobotLigneAuto : MonoBehaviour
{
    
    [Header("Références Hiérarchie")]
    public Transform q1_Z; 
    public Transform q2_X; 
    public Transform q3_Y;
    public Transform q4_RotX;
    public Transform pointe_E; 

    [Header("Paramètres UI")]
    public TextMeshProUGUI boutonTexte; // Glissez ici le composant Text de votre bouton

    [Header("Paramètres Géométriques")]
    public float D1 = 1.0f;
    public float D2 = 0.5f;
    public float D3 = 0.5f;

    [Header("Mouvement")]
    public float vitesse = 0.5f;
    public float angleInclinaison = 45.0f;
    public bool IsMoving { get { return isMoving; } } // Ajout d'une propriété publique
    private bool isMoving = false;
    private float tempsEcoule = 0f;
    
    private Vector3 startPos1, startPos2, startPos3;
    private Quaternion startRot4;
    private float pxd_initial, pyd_initial, pzd_initial;

    void Start()
    {
        // Initialisation des positions
        if (q1_Z) startPos1 = q1_Z.localPosition;
        if (q2_X) startPos2 = q2_X.localPosition;
        if (q3_Y) startPos3 = q3_Y.localPosition;
        if (q4_RotX) startRot4 = q4_RotX.localRotation;

        // Calcul du point de départ trajectoire
        float rad4 = angleInclinaison * Mathf.Deg2Rad;
        pxd_initial = D1 + (q1_Z.localPosition.z) + Mathf.Cos(rad4) * D2 - Mathf.Sin(rad4) * D3;
        pyd_initial = q2_X.localPosition.x;
        pzd_initial = q3_Y.localPosition.y + Mathf.Sin(rad4) * D2 + Mathf.Cos(rad4) * D3;

        // Initialisation du texte si assigné
        if (boutonTexte != null) boutonTexte.text = "DEMARRER";
    }

    public void ToggleMovement()
    {
        isMoving = !isMoving;

        // Mise à jour visuelle du bouton
        if (boutonTexte != null)
        {
            boutonTexte.text = isMoving ? "ARRETER" : "REPRENDRE";
            boutonTexte.color = isMoving ? Color.red : Color.green;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            tempsEcoule += Time.deltaTime;

            float deplacement = vitesse * tempsEcoule; 
            
            float pxd = pxd_initial + deplacement; 
            float pyd = pyd_initial + deplacement;      
            float pzd = pzd_initial; 
            float rad4 = angleInclinaison * Mathf.Deg2Rad;

            // MDI (Modèle Géométrique Inverse)
            float q2 = pyd; 
            float q1 = pxd - D1 - (Mathf.Cos(rad4) * D2) + (Mathf.Sin(rad4) * D3);
            float q3 = pzd - (Mathf.Sin(rad4) * D2) - (Mathf.Cos(rad4) * D3);

            // Application des positions
            q1_Z.localPosition = new Vector3(startPos1.x, startPos1.y, q1);
            q2_X.localPosition = new Vector3(q2, startPos2.y, startPos2.z);
            q3_Y.localPosition = new Vector3(startPos3.x, q3, startPos3.z);
            q4_RotX.localRotation = startRot4 * Quaternion.Euler(angleInclinaison, 0, 0);
        }
    }
}