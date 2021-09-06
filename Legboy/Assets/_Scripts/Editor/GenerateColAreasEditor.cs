using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateColAreas))]
class GenerateColAreasEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GenerateColAreas colAreasGenerator = (GenerateColAreas) target;
        if (GUILayout.Button("Generate"))
        {
            colAreasGenerator.GenerateColliders();
        }
    }
}
