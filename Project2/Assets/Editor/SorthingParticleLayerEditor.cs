using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SorthingParticleLayer))]
public class SorthingParticleLayerEditor : Editor {

    SerializedProperty sorthingLayerProp;

    public void OnEnable(){
        (target as SorthingParticleLayer).Start();
    }

    public void OnInspectorGUI() {
        serializedObject.Update();

        (target as SorthingParticleLayer).Start();
        
        serializedObject.ApplyModifiedProperties();
    }
}