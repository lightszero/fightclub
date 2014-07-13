using UnityEngine;
using System.Collections;
using CLUI;
using System.Collections.Generic;

public class state_1PacketList : IState
{


    IStateMgr mgr;
    CLUI_Node_Box guiBack = null;
    CLUI_Node_Empty guiListBack = null;

    Queue<string> cacheurls = new Queue<string>();
    public void OnInit(IStateMgr mgr)
    {
        this.mgr = mgr;
        var back = new CLUI.CLUI_Node_Box("_back");
        back.rectScale = CLUI_Border.ScaleFill;
        mgr.nodeUIRoot.AddNode(back);

        guiBack = new CLUI.CLUI_Node_Box("back");
        guiBack.rectScale = new CLUI_Border(0.1f, 0.1f, 0.9f, 0.9f);
        back.AddNode(guiBack);

        guiListBack = new CLUI_Node_Empty("listback");
       
        //var btn01 = new CLUI.CLUI_Node_Button("btn01");
        //btn01.rectScale = new CLUI_Border(0.1f, 0.1f, 0.5f, 0.2f);
        //btn01.text = "hello";
        //mgr.nodeUIRoot.AddNode(btn01);


        //从磁盘加载
        string pakpath = "";
        if (Application.isEditor)
        {
            pakpath = System.IO.Path.GetFullPath("../game/paks");
        }
        else
        {
            pakpath = System.IO.Path.GetFullPath("paks");
            Debug.Log("path1 =" + pakpath);
        }

        string[] pakdesc = System.IO.Directory.GetFiles(pakpath, "packet.desc.txt", System.IO.SearchOption.AllDirectories);
       
        foreach (var p in pakdesc)
        {
            string path = System.IO.Path.GetDirectoryName(p);
            string cacheurl = path;  //"file://" +                path;
            cacheurls.Enqueue(cacheurl);

            ResmgrNative.Instance.LoadTexture2DFromCache("logo.jpg",cacheurl, (tex, _cacheurl) =>
            {
                AddPacket(_cacheurl,tex);
            });
        }
        ResmgrNative.Instance.SetCacheUrl(cacheurls.Dequeue());
    }
    void AddPacket(string _cacheurl, Texture2D tex)
    {
        CLUI_Node_Texture texture = new CLUI_Node_Texture("logo:" + _cacheurl);
        texture.rectScale = new CLUI_Border(0, 0, 1.0f, 0.45f);
        texture.texture = tex;

        CLUI_Node_Label label = new CLUI_Node_Label("label");
        label.text = System.IO.Path.GetFileNameWithoutExtension(_cacheurl);
 
        texture.AddNode(label);
        guiListBack.AddNode(texture);
        //ResmgrNative.Instance.SetCacheUrl(cacheurls.Dequeue());

    }
    public void OnExit()
    {

    }
    Rect viewRect = new Rect();
    public void OnUpdate()
    {
        viewRect = new Rect(0, 0, guiBack.rectWorld.width, guiBack.rectWorld.height);
        guiListBack.rectPosition = new CLUI_Border(0, 0, viewRect.width, viewRect.height);
        guiListBack.rectScale = CLUI_Border.PosZero;
        guiListBack.DoUpdate();
    }
    Vector2 pos = Vector2.zero;
    public void OnGUI()
    {
        Rect viewRect = new Rect(0, 0, guiBack.rectWorld.width, guiBack.rectWorld.height*2);
        pos = GUI.BeginScrollView(guiBack.rectWorld, pos, viewRect);
        {
            guiListBack.DoGUI();
        }
        GUI.EndScrollView();
    }
}
