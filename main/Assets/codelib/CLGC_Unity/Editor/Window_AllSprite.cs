//vvv2

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
public class Window_AllSprite : EditorWindow
{
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        //获取现有的打开窗口或如果没有，创建一个新的



        var window = EditorWindow.GetWindow<Window_AllSprite>(true, "All Sprite Window");
        //window.title = "才";
        window.selfInit();
    }

    List<string> filesPuppet = new List<string>();
    void selfInit()
    {
        filesPuppet.Clear();


        TextureBlockMgr.InitOnce();
        MeshGroupMgr.InitOnce();

        foreach (var gg in MeshGroupMgr.mapGroupGroup)
        {
            filesPuppet.Add(gg.Key);
        }
            //var _files = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath("."), "*.Puppet.xml", System.IO.SearchOption.AllDirectories);
            //foreach (var f in _files)
            //{
            //    string file = f;
            //    var i = file.ToLower().LastIndexOf("resources");
            //    file = file.Substring(i + "resources".Length + 1);
            //    var i2 = file.LastIndexOf('.');
            //    file = file.Substring(0, i2);   //must be。 remove ext name
            //    file = file.Replace('\\', '/'); //must be。 use / symble
            //    filesPuppet.Add(file);
            //}
            //filesPuppet.AddRange(_files);
        

    }
    Vector2 sviewpos = new Vector2(0, 0);
    Vector2 sviewpos2 = new Vector2(0, 0);
    public int tt = 0;
    public float hslider =0.5f;
    void OnGUI()
    {
        //hslider = GUILayout.HorizontalSlider(hslider, 0, 1, GUILayout.MinWidth(this.position.width),GUILayout.Height(20));
        GUILayout.BeginHorizontal(GUILayout.Width(this.position.width));
        {
            sviewpos = EditorGUILayout.BeginScrollView(sviewpos, GUILayout.Width(hslider * this.position.width));
            OnGUIPuppetSelectPanel(this.position.width - 30);
            CLGCInEditor.Layout_DrawSeparator(new Color(0, 0, 0, 0.5f));
            OnGUIPuppetAnimSelectPanel(this.position.width - 30);
            EditorGUILayout.EndScrollView();
        }
        CLGCInEditor.Layout_DrawSeparatorV(new Color(0, 0, 0, 0.5f));
        {
            sviewpos2 = EditorGUILayout.BeginScrollView(sviewpos2, GUILayout.Width(hslider * this.position.width));
            OnGUIPlayerPanel(this.position.width);
            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndHorizontal();
    }

    bool bPuppetToggle = true;
    int nPuppetSelect = -1;
    void OnGUIPuppetSelectPanel(float width)
    {


        bPuppetToggle = GUILayout.Toggle(bPuppetToggle, "Select Puppet", GUILayout.Height(20));
        if (bPuppetToggle)
        {

            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
            GUILayout.Space(30);

           
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUI.backgroundColor = Color.cyan;
                int newSelect = GUILayout.SelectionGrid(nPuppetSelect, filesPuppet.ToArray(), 1, GUILayout.MaxWidth(width - 30));
                if (newSelect != nPuppetSelect)
                {
                    onSelectFile(filesPuppet[newSelect]);
                    nPuppetSelect = newSelect;
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
        }

    }
    bool bPuppetAnimToggle = true;
    List<string> filesPuppetAnim = new List<string>();
    List<string> filesPuppetAnimFail = new List<string>();
    int nPuppetAnimSelect = -1;
    void OnGUIPuppetAnimSelectPanel(float width)
    {
        if (nPuppetSelect < 0)
        {
            GUILayout.TextArea("Please select puppet first.", GUILayout.MaxWidth(width));
            return;
        }
        bPuppetAnimToggle = GUILayout.Toggle(bPuppetAnimToggle, "Select Puppet Anim", GUILayout.Height(20));
        if (bPuppetAnimToggle)
        {
            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
            GUILayout.Space(30);
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUI.backgroundColor = Color.cyan;
                int newSelect = GUILayout.SelectionGrid(nPuppetAnimSelect, filesPuppetAnim.ToArray(), 1, GUILayout.MaxWidth(width - 30));
                if (newSelect != nPuppetAnimSelect)
                {
                    onSelectAnimFile(filesPuppetAnim[newSelect] + ".PuppetAnim");
                    nPuppetAnimSelect = newSelect;
                }
                GUI.backgroundColor = Color.white;
                foreach (var i in filesPuppetAnimFail)
                {
                    GUILayout.Label(i, GUILayout.MaxWidth(width - 30));
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            CLGCInEditor.Layout_DrawSeparator(new Color(1, 1, 1, 0.25f));
        }
    }
    //ha_ani_tool.hadata.puppetdata selectdata;
    void onSelectFile(string filename)
    {

        filesPuppetAnim.Clear();
        filesPuppetAnimFail.Clear();
        foreach (var ani in MeshGroupMgr.mapGroupGroup[filename].data.GetAllAniname())
        {
            if (MeshGroupMgr.GetMeshGroup(filename, ani) != null)
            {
                filesPuppetAnim.Add(ani);
            }
            else
            {
                filesPuppetAnimFail.Add(ani);
            }
        }
    }

    Dictionary<string, List<ha_ani_tool.hadata.Attachment>> mapPart = new Dictionary<string, List<ha_ani_tool.hadata.Attachment>>();

    void onSelectAnimFile(string filename)
    {
        Debug.Log("onSelectAnimFile=" + filename);


    }

    int iframe = 0;
    void OnGUIPlayerPanel(float width)
    {
        GUILayout.Label("OnGUIPlayerPanel", GUILayout.MaxWidth(200));
        return;

        GUILayout.Box("", GUILayout.MaxWidth(2006));
        var rect = GUILayoutUtility.GetLastRect();

    }
    void DoAttm(string name, ha_ani_tool.hadata.Node.Frame frame,Rect rect, Vector2 pos)
    {
        GUILayout.Label("DoAttm=" + mapPart.Count, GUILayout.MaxWidth(200));
    }
    float t=0;
    void Update()
    {
        t += Time.deltaTime;
        iframe++;
        this.Repaint();
    }
    public override string ToString()
    {
        return "才";
    }
}
