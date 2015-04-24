using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DestroyOnEditor))]
public class DestroyOnEditorEditor : Editor {
    
    public void OnEnable() {
        Destroy((target as GameObject).GetComponent<DestroyOnEditor>());
    }
}