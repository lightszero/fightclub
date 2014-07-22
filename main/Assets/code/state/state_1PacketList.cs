using UnityEngine;
using System.Collections;
using CLUI;
using System.Collections.Generic;
using control;
using System.Net;

public class state_1PacketList : IState
{


    IStateMgr mgr;
    //CLUI_Node_Box guiBack = null;
    //CLUI_Node_Empty guiListBack = null;

    Queue<string> cacheurls = new Queue<string>();
    public void OnInit(IStateMgr mgr)
    {
        this.mgr = mgr;
        //var back = new CLUI.CLUI_Node_Box("_back");
        //back.rectScale = CLUI_Border.ScaleFill;
        //mgr.nodeUIRoot.AddNode(back);

        //guiBack = new CLUI.CLUI_Node_Box("back");
        //guiBack.rectScale = new CLUI_Border(0.1f, 0.1f, 0.9f, 0.9f);
        //back.AddNode(guiBack);

        //guiListBack = new CLUI_Node_Empty("listback");
       
        //var btn01 = new CLUI.CLUI_Node_Button("btn01");
        //btn01.rectScale = new CLUI_Border(0.1f, 0.1f, 0.5f, 0.2f);
        //btn01.text = "hello";
        //mgr.nodeUIRoot.AddNode(btn01);
        if (player == null)
        {
            player = new control.Player();
            player.SetInput(controller);
            player.PlayerState = new PlayerState();
            {
                SkillCmd cmd = new SkillCmd();
                cmd.SkillName = "超级波动拳";
                cmd.DriveStyle = DriveStyle.BtnPressDrive;
                cmd.ClearCmd = true;
                cmd.BtnReleaseDrive = false;
                cmd.padtag.Add((byte)PADTAG.PADKEY_BACK);//后
                cmd.padtag.Add((byte)PADTAG.PADKEY_DOWN);//下
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.padtag.Add((byte)PADTAG.PADKEY_BACK);//后
                cmd.padtag.Add((byte)PADTAG.PADKEY_DOWN);//下
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.btntag.Add((byte)KEYTAG.GAMEKEY_2);//拳
                player.PlayerState.listSkill.Add(cmd);
            }

            {
                SkillCmd cmd = new SkillCmd();
                cmd.SkillName = "波动拳";
                cmd.DriveStyle = DriveStyle.BtnPressDrive;
                cmd.ClearCmd = true;
                cmd.BtnReleaseDrive = false;
                cmd.padtag.Add((byte)PADTAG.PADKEY_BACK);//后
                cmd.padtag.Add((byte)PADTAG.PADKEY_DOWN);//下
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.btntag.Add((byte)KEYTAG.GAMEKEY_2);//拳
                player.PlayerState.listSkill.Add(cmd);
            }
            {
                SkillCmd cmd = new SkillCmd();
                cmd.SkillName = "升龙拳";
                cmd.DriveStyle = DriveStyle.BtnPressDrive;
                cmd.ClearCmd = true;
                cmd.BtnReleaseDrive = false;
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.padtag.Add((byte)PADTAG.PADKEY_DOWN);//下
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.btntag.Add((byte)KEYTAG.GAMEKEY_2);//拳
                player.PlayerState.listSkill.Add(cmd);
            }
            {
                SkillCmd cmd = new SkillCmd();
                cmd.SkillName = "冲拳";
                cmd.DriveStyle = DriveStyle.BtnPressDrive;
                cmd.ClearCmd = true;
                cmd.BtnReleaseDrive = false;
                cmd.padtag.Add((byte)PADTAG.PADKEY_RELEASE);//空
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.padtag.Add((byte)PADTAG.PADKEY_RELEASE);//空
                cmd.padtag.Add((byte)PADTAG.PADKEY_FRONT);//前
                cmd.btntag.Add((byte)KEYTAG.GAMEKEY_2);//拳
                player.PlayerState.listSkill.Add(cmd);
            }
            {
                SkillCmd cmd = new SkillCmd();
                cmd.SkillName = "飞踢";
                cmd.DriveStyle = DriveStyle.BtnPressDrive;
                cmd.ClearCmd = true;
                cmd.BtnReleaseDrive = false;
                cmd.padtag.Add((byte)PADTAG.PADKEY_DOWN);//下
                cmd.padtag.Add((byte)PADTAG.PADKEY_UP);//上
                cmd.btntag.Add((byte)KEYTAG.GAMEKEY_2);//脚
                player.PlayerState.listSkill.Add(cmd);
            }
        }

        //应付Oray域名转向的方法
        //第一次请求 会返回一个Set-Cookie
        string cook = null;
        {
            System.Net.HttpWebRequest req = (HttpWebRequest)System.Net.HttpWebRequest.Create("http://w2.cltri.com");

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            cook =response.Headers["Set-Cookie"];
            Debug.Log("cook=" + cook);
            Debug.Log(response.ResponseUri + "(" + response.ContentLength + ")" + response.StatusCode + " " + response.StatusDescription);

            System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
            string text = sr.ReadToEnd();
            Debug.Log(text);
        }
        string[] cc = cook.Split(new string[] { ";", " " }, System.StringSplitOptions.RemoveEmptyEntries);
        string cpath="";
        Dictionary<string, string> cookie = new Dictionary<string, string>();
        foreach(var c in cc)
        {
            Debug.Log("c=" + c);
            
            string[] v = c.Split('=');
            if(v[0]=="path")
            {
                cpath=v[1];
                continue;
            }
            cookie[v[0]] =v[1];
        }
        //第二次请求用此SetCookie的值再请求才能返回正确值
        {
            System.Net.HttpWebRequest req = (HttpWebRequest)System.Net.HttpWebRequest.Create("http://w2.cltri.com");
            req.CookieContainer = new CookieContainer();
            foreach (var c in cookie)
            {
                req.CookieContainer.Add(new Cookie(c.Key, c.Value,cpath,req.RequestUri.Host));
            }
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            cook = response.Headers["Set-Cookie"];
            Debug.Log(response.ResponseUri + "(" + response.ContentLength + ")" + response.StatusCode + " " + response.StatusDescription);

            foreach(string k in response.Headers)
            {
                Debug.Log("k=" + k + " v= " + response.Headers[k]);
            }
            System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
            string text = sr.ReadToEnd();
            Debug.Log(text);// .Substring(0,100));
        }
        //www = new WWW("http://w1.cltri.com");

        //从磁盘加载
#if UNITY_STANDALONE
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
#endif
    }
    //WWW www;
    void AddPacket(string _cacheurl, Texture2D tex)
    {
        //CLUI_Node_Texture texture = new CLUI_Node_Texture("logo:" + _cacheurl);
        //texture.rectScale = new CLUI_Border(0, 0, 1.0f, 0.45f);
        //texture.texture = tex;

        //CLUI_Node_Label label = new CLUI_Node_Label("label");
        //label.text = System.IO.Path.GetFileNameWithoutExtension(_cacheurl);

        //texture.AddNode(label);
        //guiListBack.AddNode(texture);
        //ResmgrNative.Instance.SetCacheUrl(cacheurls.Dequeue());

    }
    public void OnExit()
    {

    }
    //Rect viewRect = new Rect();
    public void OnUpdate()
    {
        //if(www!=null&&www.isDone)
        //{
        //    Debug.Log(www.url);
        //    string text = www.text;
        //    System.IO.File.WriteAllText("d:\\1.txt",text);
        //    Debug.Log(www.text);
        //    www = null;
        //}
        controller.Update();

        player.Update();
        //viewRect = new Rect(0, 0, guiBack.rectWorld.width, guiBack.rectWorld.height);
        //guiListBack.rectPosition = new CLUI_Border(0, 0, viewRect.width, viewRect.height);
        //guiListBack.rectScale = CLUI_Border.PosZero;
        //guiListBack.DoUpdate();

    }
    Vector2 pos = Vector2.zero;
    control.Player player = null;

    control.GamePadController controller = new control.GamePadController();
    string lastcode = "";
    string cmdlist = "";
    public void OnGUI()
    {
        if (lastcode != controller.ToString())
        {
            lastcode = controller.ToString();
            if (lastcode == "")
            {
                cmdlist += ".";
            }
            else
            {
                cmdlist += lastcode;
            }
            if (cmdlist.Length > 25) cmdlist = cmdlist.Substring(cmdlist.Length - 25);
        }
        GUI.Box(new Rect(100, 100, 600, 150), "");
        GUI.Label(new Rect(100, 100, 600, 50), cmdlist);
        GUI.Label(new Rect(100, 175, 600, 50), player.GetSkillStr());
        //Rect viewRect = new Rect(0, 0, guiBack.rectWorld.width, guiBack.rectWorld.height*2);
        //pos = GUI.BeginScrollView(guiBack.rectWorld, pos, viewRect);
        //{
        //    guiListBack.DoGUI();
        //}
        //GUI.EndScrollView();
    }
}
