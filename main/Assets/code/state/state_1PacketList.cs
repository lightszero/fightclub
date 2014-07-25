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

    PacketList plist = new PacketList();

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


        //从磁盘加载
#if UNITY_STANDALONE
        IList<string> packets= plist.InitPacketListLocal();
        foreach(var p in packets)
        {
            Debug.Log("got packet=" + p);
        }

        ResmgrNative.Instance.SetCacheUrl(plist.GetPackPathLocal());
       
        loadcurcount=3;
        ResmgrNative.Instance.LoadStringFromCache("base/skeleton.json.txt", "", onloadsk);
        ResmgrNative.Instance.LoadStringFromCache("base/cur.atlas.txt", "", onloadat);
        ResmgrNative.Instance.LoadTexture2DFromCache("base/cur.png", "", onloadatt);
#endif
    }
     int loadcurcount=3;
    string curSK;
    string curAtlas;
    Texture2D curAtlasTex;
    void onloadsk(string code,string tag)
    {
        curSK=code;
        loadcurcount--;
        if(loadcurcount==0)
        {
            OnCurLoadFinish();
        }
    }
    void onloadat(string code,string tag)
    {
        curAtlas = code;
        loadcurcount--;
        if (loadcurcount == 0)
        {
            OnCurLoadFinish();
        }
    }
    void onloadatt(Texture2D tex,string tag)
    {
        curAtlasTex = tex;
        loadcurcount--;
        if (loadcurcount == 0)
        {
            OnCurLoadFinish();
        }
    }
    void OnCurLoadFinish()
    {//先临时验证一下代码构造spine动画,两个atlas的设计问题较多,必须要休整一下播放组件
        GameObject curObj=new GameObject();
        curObj.name="spine-cur";
        SkeletonAnimationCL ani = curObj.AddComponent<SkeletonAnimationCL>();
        //AtlasAsset assAtlas = new AtlasAsset();
        //assAtlas.materials = new Material[1];
        //assAtlas.materials[0] = new Material(Shader.Find("Spine/Skeleton"));
        //assAtlas.materials[0].mainTexture = curAtlasTex;
        Dictionary<string, Texture2D> texs = new Dictionary<string, Texture2D>();
        texs["cur.png"] = curAtlasTex;
        var atlas = new Spine.Atlas(new System.IO.StringReader(curAtlas), "", new TextureLoad(texs));
        atlas.FlipV();
        //assAtlas.SetAtlas(atlas);

        Spine.SkeletonJson sjson = new Spine.SkeletonJson(atlas);
        sjson.Scale = 1.0f/64.0f;
        var data=sjson.ReadSkeletonData(new System.IO.StringReader(curSK));
        
        ani.skeleton = new Spine.Skeleton(data);
        ani.Reset();
        ani.state.SetAnimation(0, "idle", true);

    }
    class TextureLoad:Spine.TextureLoader
    {
        IDictionary<string, Texture2D> texs;

        public TextureLoad(IDictionary<string, Texture2D> texs)
        {
           this.texs = texs;
            foreach(var t in texs.Keys)
            {
                Debug.Log("got tex:" + t);
            }
        }
        public void Load(Spine.AtlasPage page, string path)
        {
            Debug.Log("load page:" + page.name);
            var mat = new Material(Shader.Find("Spine/Skeleton"));
            mat.mainTexture =texs[page.name];
            page.rendererObject = mat;
            page.width = mat.mainTexture.width;
            page.height = mat.mainTexture.height;
        }

        public void Unload(object mat)
        {
            GameObject.Destroy(mat as Material);
        }
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
