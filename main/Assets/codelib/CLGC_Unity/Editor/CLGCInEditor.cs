//ver 啊
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CLGCInEditor  {

    public static class CLGCMenu
    {
        public const string root = "CLGC/";
    }
	
	
    [MenuItem(CLGCMenu.root + "All Atlas", false, 1)]
    public static void ShowAllAtlasWindow()
    {
        Window_AllAtlas.Init();
    }
    [MenuItem(CLGCMenu.root + "All Sprite", false, 1)]
    public static void ShowAllSpriteWindow()
    {
        Window_AllSprite.Init();
    }
    [MenuItem(CLGCMenu.root + "Clear All...", false, 1)]
    public static void ClearAll()
    {
        TextureBlockMgr.ReInit();
        MeshGroupMgr.Clear();
    }
	
    [MenuItem(CLGCMenu.root + "About...", false, 1)]
    public static void ShowPreferences()
    {
        EditorUtility.DisplayDialog("About CLGC","Hello World.", "Ok");

    }
	
	
	
    //[MenuItem("GameObject/Create Other/CLGC/RenderBoard", false, 12900)]
    //static void DoCreateSpriteObject()
    //{
    //    GameObject go =  CreateGameObjectInScene("CLGCRoot");
    //    RenderBoard sprite = go.AddComponent<RenderBoard>();
    //    sprite.Build();
    //}
    public static GameObject CreateGameObjectInScene(string name)
    {
		
        string realName = name;
        int counter = 0;
        while (GameObject.Find(realName) != null)
        {
            realName = name + counter++;
        }

        GameObject go = new GameObject(realName);
        if (Selection.activeGameObject != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
            if (assetPath.Length == 0) go.transform.parent = Selection.activeGameObject.transform;
        }
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go;
    }

    public static void Layout_DrawSeparator(Color color,float height=4f)
    {
        
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(0f, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }
    public static void Layout_DrawSeparatorV(Color color, float width = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, Screen.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }
}
