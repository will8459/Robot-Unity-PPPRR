using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotManualControl : MonoBehaviour
{
    [Header("Articulations (Pivots)")]
    public Transform q1_Z;      // Liaison 1 (Prismatique)
    public Transform q2_X;      // Liaison 2 (Prismatique)
    public Transform q3_Y;      // Liaison 3 (Prismatique)
    public Transform q4_RotX;   // Liaison 4 (Rotoïde)
    public Transform q5_RotZ;   // Liaison 5 (Rotoïde)
    public Transform pointe_E;  // Effecteur final

    [Header("Interface Sliders")]
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;
    public Slider slider5;

    [Header("Paramètres DH (Modèle)")]
    public float D1 = 0f; // Offset base vers liaison 2
    public float D2 = 0f; // Offset liaison 3 vers 4
    public float D3 = 0f; // Longueur outil (axe Z)
    public float A = 0f; // Offset pointe (décalage latéral)

    [Header("Affichage")]
    public TMP_Text affichageTexte;

    void Update()
    {
        // 1. Récupération des valeurs des Sliders
        float q1 = slider1.value;
        float q2 = slider2.value;
        float q3 = slider3.value;
        float q4 = slider4.value;
        float q5 = slider5.value;

        // 2. Application PHYSIQUE sur Unity (Mouvement des objets)
        // Les axes correspondent à votre schéma cinématique
        q1_Z.localPosition = new Vector3(0, 0, q1);
        q2_X.localPosition = new Vector3(q2, 0, 0);
        q3_Y.localPosition = new Vector3(0, q3, 0);
        q4_RotX.localEulerAngles = new Vector3(0, q4, 0); // Rotation q4 autour de X
        q5_RotZ.localEulerAngles = new Vector3(0, q5, 0); // Rotation q5 autour de Z

        // 3. Calcul MGD (Matrice U0)
        float c4 = Mathf.Cos(q4 * Mathf.Deg2Rad);
        float s4 = Mathf.Sin(q4 * Mathf.Deg2Rad);
        float c5 = Mathf.Cos(q5 * Mathf.Deg2Rad);
        float s5 = Mathf.Sin(q5 * Mathf.Deg2Rad);

        // Équations de la matrice U0 
        float Px = D1 + q3 + (D3 * c4) + (A * s4 * c5); // Correspond à Y Unity
        float Py = 2*q2 - (A * s5);                       // Correspond à X Unity
        float Pz = q1 + D2 - (D3 * s4) + (A * c4 * c5); // Correspond à Z Unity
        
        // 4. Mapping vers les coordonnées Unity (X, Y, Z)
        // Px=Hauteur(Y), Py=Latéral(X), Pz=Profondeur(Z)
        Vector3 posCalculée = new Vector3(Py + 2, Px, Pz);

        // 5. Comparaison avec la position réelle de pointe_E
        // transform.InverseTransformPoint donne la position mondiale relative à la Base
        Vector3 posUnityReal = transform.InverseTransformPoint(pointe_E.position);
        float erreur = Vector3.Distance(posUnityReal, posCalculée);

        // 6. Mise à jour de l'UI
        if (affichageTexte != null)
        {
            affichageTexte.text = $"Unity (Réel) : {posUnityReal.ToString("F3")}\n" +
                                  $"MGD (Calculé) : {posCalculée.ToString("F3")}\n" +
                                  $"Erreur : {(erreur * 1000f).ToString("F2")} mm";
        }
    }

    public void ResetToOrigin()
    {
        // 1. Arrêter le mode automatique s'il est actif
        RobotAutoControl auto = GetComponent<RobotAutoControl>();
        if (auto != null) {
            auto.StopAutomation(); // Cette méthode réactive aussi le script manuel
        }

        // 2. Remettre tous les sliders à zéro
        // Cela déclenchera automatiquement le mouvement dans l'Update() suivant
        if(slider1) slider1.value = 0;
        if(slider2) slider2.value = 0;
        if(slider3) slider3.value = 0;
        if(slider4) slider4.value = 0;
        if(slider5) slider5.value = 0;
        // 3. Forcer la mise à jour immédiate pour éviter un saut d'une frame
        Canvas.ForceUpdateCanvases(); 
        Debug.Log("Robot réinitialisé à l'état d'origine.");
    }

}

