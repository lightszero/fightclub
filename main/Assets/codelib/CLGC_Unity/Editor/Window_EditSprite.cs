using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
public class Window_EditSprite :EditorWindow
{
    	public static void Init (Com_Sprite sprite) 
        {
		// Get existing open window or if none, make a new one:
		//获取现有的打开窗口或如果没有，创建一个新的


            Debug.Log("Window_EditSprite.Init(" + (sprite != null) + ")");
            var window = EditorWindow.GetWindow<Window_EditSprite>(true, "精灵查看");
            //window.title = "才";
            window.selfInit(sprite);
        }

        List<string> filesPuppet = new List<string>();
        Com_Sprite EditSprite;
        void selfInit(Com_Sprite sprite)
        {
            EditSprite = sprite;
            filesPuppet.Clear();


            TextureBlockMgr.InitOnce();
            MeshGroupMgr.InitOnce();
            foreach (var gg in MeshGroupMgr.mapGroupGroup)
            {
                filesPuppet.Add(gg.Key);
            }
            
        }

        Vector2 sviewpos = new Vector2(0, 0);
        Vector2 sviewpos2 = new Vector2(0, 0);
        public float hslider = 0.5f;
        void OnGUI()
        {
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
                        onSelectAnim(filesPuppetAnim[newSelect]);
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
        bool bPlay = false;
        void OnGUIPlayerPanel(float width)
        {
            bPlay = GUILayout.Toggle(bPlay, "Play", GUILayout.Width(100));
            if(GUILayout.Button("frame++",GUILayout.Width(100)))
            {
                this.EditSprite.updateAnim(1/30.0f);
            }
        }
        ha_ani_tool.hadata.puppetdata selectdata;
        void onSelectFile(string filename)
        {
            EditSprite.puppetfile = filename;
            

            filesPuppetAnim.Clear();
            filesPuppetAnimFail.Clear();
            foreach (var ani in MeshGroupMgr.mapGroupGroup[filename].data.GetAllAniname())
            {
                if (MeshGroupMgr.GetMeshGroup(filename, ani)!=null)
                {
                     filesPuppetAnim.Add(ani);
                }
                else
                {
                    filesPuppetAnimFail.Add(ani);
                }
            }
           
        }

        void onSelectAnim(string filename)
        {
            EditSprite.defaultanim = filename;
            EditSprite.updateMesh();
        }
        public int tt = 0;
        DateTime t = DateTime.Now;
        void Update()
        {
            
            if (bPlay)
            {
                DateTime now =DateTime.Now;

                float delta =(float)((now-t).TotalSeconds);
                if(delta>0.3f)delta=0.3f;
                t = now;
                //this.EditSprite.updateMesh();
                this.EditSprite.updateAnim(delta);
;               
                //Debug.Log("ua");
            }
            tt++;
            this.Repaint();
        }

}
