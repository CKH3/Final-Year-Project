using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor (typeof (WaterGenerator))]
public class WaterEditorGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        WaterGenerator waterGenerator = (WaterGenerator)target;
        
        DrawDefaultInspector();

        if(GUILayout.Button("Generate")){
            waterGenerator.CreateWaterMap(Vector3.zero);
        }

        if(GUILayout.Button("Clear")){
            waterGenerator.ClearWaterMap();
        }
    }
}