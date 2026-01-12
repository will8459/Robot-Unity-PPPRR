using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class TransformWorldEditor : Editor {
    public override void OnInspectorGUI() {
        Transform t = (Transform)target;
        
        // Affiche l'inspector par défaut (Local)
        base.OnInspectorGUI();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("World Space (Scene Center Origin)", EditorStyles.boldLabel);
        
        // Affiche et permet de modifier la position globale
        Vector3 worldPos = EditorGUILayout.Vector3Field("World Position", t.position);
        if (GUI.changed) {
            Undo.RecordObject(t, "Transform Change");
            t.position = worldPos;
        }
    }
}