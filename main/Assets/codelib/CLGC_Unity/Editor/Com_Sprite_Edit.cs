using UnityEngine;
using System.Collections;
using UnityEditor;


//[CanEditMultipleObjects]
[CustomEditor(typeof(Com_Sprite))]
public class Com_Sprite_Edit : Editor {

    //Load
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Com_Sprite s = this.target as Com_Sprite;
        if (GUILayout.Button("LoadSprite"))
        {
            
            Window_EditSprite.Init(this.target as Com_Sprite);
            //RenderBoard t = target as RenderBoard;
            //if (t != null)
            //{
            //    t.ReBuild();

            //}
        }
        if (GUILayout.Button("UpdateSprtie"))
        {
            s.RupdateMesh();
        }
        
    }
    

}
