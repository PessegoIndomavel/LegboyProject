using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateVerticalIsleMap))]
public class GenerateVerticalIsleMapEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GenerateVerticalIsleMap vertIsleGenerator = (GenerateVerticalIsleMap) target;
        if (GUILayout.Button("Generate"))
        {
            vertIsleGenerator.GenerateVertIsleMap();
        }
    }
}
