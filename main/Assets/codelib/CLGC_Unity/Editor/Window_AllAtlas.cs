using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class Window_AllAtlas : EditorWindow
{
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        //获取现有的打开窗口或如果没有，创建一个新的



        var window = EditorWindow.GetWindow<Window_AllAtlas>(true, "All Atlas Window");
        //window.title = "才";
        window.selfInit();
    }

    List<string> filesSheets = new List<string>();
    void selfInit()
    {
        filesSheets.Clear();

        TextureBlockMgr.InitOnce();
      
        filesSheets.AddRange(TextureBlockMgr.getAllTextureFileName());
    }
    Vector2 sviewpos = new Vector2(0, 0);

    public int tt = 0;
    public float hslider =0.5f;
    Vector2 sviewpos2 = new Vector2(0, 0);
    void OnGUI()
    {
        hslider = GUILayout.HorizontalSlider(hslider, 0.1f, 0.9f, GUILayout.MinWidth(this.position.width),GUILayout.Height(20));
        GUILayout.BeginHorizontal(GUILayout.Width(this.position.width));
        {
            sviewpos = EditorGUILayout.BeginScrollView(sviewpos, GUILayout.Width(hslider * this.position.width));
            OnGUIAtlasSelectPanel(this.position.width - 30);
            CLGCInEditor.Layout_DrawSeparator(new Color(0, 0, 0, 0.5f));
            OnGUIPuppetAnimSelectPanel(this.position.width - 30);
            EditorGUILayout.EndScrollView();
        }
        CLGCInEditor.Layout_DrawSeparatorV(new Color(0, 0, 0, 0.5f));
        {
            var width=this.position.width * (1 - hslider) -5;
            sviewpos2=GUILayout.BeginScrollView(sviewpos2, GUILayout.Width(width));
            OnGUIPlayerPanel(width);
            GUILayout.EndScrollView();
        }
        GUILayout.EndHorizontal();
    }

    bool bSheetToggle = true;
    int nPuppetSelect = -1;
    void OnGUIAtlasSelectPanel(float width)
    {


        bSheetToggle = GUILayout.Toggle(bSheetToggle, "Select Atlas", GUILayout.Height(20));
        if (bSheetToggle)
        {

            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
            GUILayout.Space(30);

           
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUI.backgroundColor = Color.cyan;
                int newSelect = GUILayout.SelectionGrid(nPuppetSelect, filesSheets.ToArray(), 1, GUILayout.MaxWidth(width - 30));
                if (newSelect != nPuppetSelect)
                {
                    onSelectFile(filesSheets[newSelect]);
                    nPuppetSelect = newSelect;
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
        }

    }
    bool bAtlasDetailToggle = true;
    List<string> spritesAtlasDetail = new List<string>();
    void OnGUIPuppetAnimSelectPanel(float width)
    {
        if (nPuppetSelect < 0)
        {
            GUILayout.TextArea("Please select atlas first.", GUILayout.MaxWidth(width));
            return;
        }
        bAtlasDetailToggle = GUILayout.Toggle(bAtlasDetailToggle, "Atlas detail", GUILayout.Height(20));
        if (bAtlasDetailToggle)
        {
            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
            GUILayout.Space(30);
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
               
                foreach (var i in spritesAtlasDetail)
                {
                    GUILayout.Label(i, GUILayout.MaxWidth(width - 30));
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
        }
    }

    TextureBlock selectTex;
    TextureBlockMgr.DetailBlock tdata;
    void onSelectFile(string filename)
    {
        //TextureAtlas tdata = null;
        {
            tdata = TextureBlockMgr.getDetail(filename);
            selectTex = TextureBlockMgr.getTexture(filename).Value;
        }
        spritesAtlasDetail.Clear();
        foreach (var ani in tdata.details)
        {
            spritesAtlasDetail.Add(ani.Key);
        }
    }
    void onSelectAnimFile(string filename)
    {
    }


    void OnGUIPlayerPanel(float width)
    {
        GUILayout.Box(selectTex.tex, GUILayout.Width(width),GUILayout.Height(width-10));
        if (tdata != null)
        {
            foreach (var a in tdata.details)
            {
                GUILayout.Label(a.Key, GUILayout.MaxWidth(width - 10));
                var rect=GUILayoutUtility.GetLastRect();
                var srect = new Rect((float)a.Value.x , (float)a.Value.y ,
                    (float)a.Value.width , (float)a.Value.height );
                srect.y = 1 - srect.y - srect.height;
                var drect= new Rect(rect.xMin,rect.yMax,a.Value.width*selectTex.tex.width,a.Value.height*selectTex.tex.height);
                //drect.width = 100;
                //drect.height = 100;
                //Debug.Log(drect);
                //srect.y = 0;
                //srect.height = 0.5f;
                GUI.DrawTextureWithTexCoords(drect, selectTex.tex, srect);
                //GUI.DrawTexture(drect, selectTex);
                GUILayout.Space(drect.height);
            }
        }
    }
    void Update()
    {
        tt++;
        this.Repaint();
    }
    public override string ToString()
    {
        return "才";
    }
}
