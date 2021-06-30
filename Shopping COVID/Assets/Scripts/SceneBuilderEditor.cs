using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneBuilder))]
public class SceneBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SceneBuilder sceneBuilder = (SceneBuilder)target;
        if (GUILayout.Button("Update Map")) {
            sceneBuilder.UpdateMap();
        }
        if (GUILayout.Button("Clear Map")) {
            sceneBuilder.ClearMap();
        }
    }
}