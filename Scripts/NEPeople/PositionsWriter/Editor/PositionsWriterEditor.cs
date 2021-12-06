using UnityEngine;
using System.Collections;
using UnityEditor;
using Nexus.Positionswriter.Runtime;

[CustomEditor(typeof(PositionsWriter))]
public class PositionsWriterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		PositionsWriter myScript = (PositionsWriter)target;
		if (GUILayout.Button("Write Positions Data"))
		{
			myScript.WritePositions();
		}
    }
}
