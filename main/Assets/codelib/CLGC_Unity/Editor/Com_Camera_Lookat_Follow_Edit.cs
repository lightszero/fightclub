using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Com_Camera_Lookat_Follow))]
public class Com_Camera_Lookat_Follow_Edit : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Com_Camera_Lookat_Follow controller = this.target as Com_Camera_Lookat_Follow;
        if (GUILayout.Button("Lookat Now"))
        {
            controller.LookAt();
        }
        if (GUILayout.Button("Lookat Now(Move)"))
        {
            controller.LookAtMove();
        }
    }
}
