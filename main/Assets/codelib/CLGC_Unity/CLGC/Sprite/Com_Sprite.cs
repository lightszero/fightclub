//ar方法提供aa
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//[ExecuteInEditMode]
[AddComponentMenu("CLGC/Sprite/Sprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Com_Sprite : MonoBehaviour
{

    void Awake()
    {
        TextureBlockMgr.InitOnce();
        //mesh = new Mesh();
        //mesh.name = "CLGC auto";
        GetComponent<MeshFilter>().mesh = new Mesh();
    }
    // Use this for initialization
    void Start()
    {
        collobj = null;
        if (KAniGenColl)
        {
            ClearKAniColl();
        }
        updateMesh();

    }
    public void RupdateMesh()
    {


        TextureBlockMgr.InitOnce();
        MeshGroupMgr.InitOnce();


        collobj = null;
        if (KAniGenColl)
        {
            ClearKAniColl();
        }
        updateMesh();

    }
    void ClearKAniColl()
    {
        List<GameObject> dels = new List<GameObject>();
        foreach (Transform t in this.transform)
        {
            if (t.gameObject.name.IndexOf("coll:") == 0)
            {
                dels.Add(t.gameObject);
                //GameObject.Destroy(t.gameObject);

            }
        }
        foreach (var d in dels)
        {
            GameObject.DestroyImmediate(d);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateAnim(Time.deltaTime);
    }
    MeshGroup group = null;
    public void updateMesh()
    {
        //Debug.Log("pdata:" + puppetfile + "kcoll"+KAniUseController);
        group = MeshGroupMgr.GetMeshGroup(puppetfile, defaultanim);
        namemap.Clear();
        if (KAniUseController)
        {
            anicontroller = null;

            InitKAni();
            string _name = this.defaultanim;
            if (System.IO.Path.DirectorySeparatorChar == '/')
            {
                _name = _name.Replace('\\', '/');
            }
            _name = System.IO.Path.GetFileNameWithoutExtension(_name);
            _name = System.IO.Path.GetFileNameWithoutExtension(_name).ToLower();
            //Debug.Log("ani:" + _name);
            anicontroller.State(GetKAnim(_name).GetElement(null));
        }
        updateAnim(0);

        //Debug.Log("Com_Sprite::updateMesh(" + (group != null) + ")");
    }
    public IEnumerable<KAnim.Dummy> GetKAniDummy()
    {
        if (anicontroller == null) return null;
        var f = anicontroller.GetFrame();
        if (f != null)
        {
            if (f.dummys.Count > 0)
            {
                return f.dummys.AsReadOnly();
            }
        }
        return null;
    }
    public KAnim.Point GetKAniFrameSize()
    {
        if (anicontroller == null) return new KAnim.Point(0, 0);
        return anicontroller.GetFrame().size;
    }
    public int GetKAniFrameId()
    {
        if (anicontroller == null) return -1;
        return anicontroller.GetFrame().frameid;
    }
    float timer = 0;

    public Color colorBlend = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public float blendWhite = 0;
    public bool useColorBlend = false;
    public bool useWhiteBlend = false;
    public void updateAnim(float delta)
    {
        if (anicontroller != null)
        {
            anicontroller.AdvTime(delta);
            //Debug.Log("aniname"+anicontroller.GetFrame().aniname);
            group = MeshGroupMgr.GetMeshGroup(puppetfile, anicontroller.GetFrame().aniname);

            int nframe = anicontroller.GetFrame().frameid;
            var frame = group.GetFrame(nframe);
            this.GetComponent<MeshFilter>().mesh = frame.mesh;
            var mats = frame.mats.ToArray();
            this.GetComponent<MeshRenderer>().materials =mats;
            if(useColorBlend)
            {
                foreach(var m in mats)
                {
                    if (useWhiteBlend)
                    {
                        if (m.shader.name != "clgc/nolit_gray")
                        {
                            m.shader = Shader.Find("clgc/nolit_gray");
                        }
                        m.SetFloat("Gray", 0);
                        m.SetFloat("White", blendWhite);
                    }
                    else
                    {
                        if (m.shader.name == "clgc/nolit_gray")
                        {
                            m.shader = Shader.Find("clgc/nolit");
                        }
                    }
                    m.color=colorBlend;//("Main Color", colorBlend);
 
                }
            }
            if (KAniGenColl)
            {
                //生成碰撞体
                GenKAniColl();
            }
            if (KAniSound)
            {
                PlayKAniSound();
            }
        }
        else
        {
            //Debug.Log("Com_Sprite::updateAnim(" + (group != null) + ")");
            if (group == null)
                return;
            //
            //动画帧更新

            timer += delta;
            int nframe = (int)(timer * 30);
            while (nframe > group.maxFrame)
            {
                nframe -= (group.maxFrame + 1);
                timer = 0;
            }
            var frame = group.GetFrame(nframe);

            this.GetComponent<MeshFilter>().mesh = frame.mesh;
            var mats = frame.mats.ToArray();
            this.GetComponent<MeshRenderer>().materials = mats;
            if (useColorBlend)
            {
                foreach (var m in mats)
                {
                    m.color = colorBlend;//("Main Color", colorBlend);
                }
            }
        }
    }
    public string puppetfile;
    public string defaultanim;

    public Dictionary<string, KAnim.SpriteAni> kanis = new Dictionary<string, KAnim.SpriteAni>();
    KAnim.SpriteAniController anicontroller = null;


    public bool isInState
    {
        get
        {
            return anicontroller.instate;
        }
    }
    public void SetAniState(string ani, string state = null, bool bimmediate = false, System.Action whenplay = null)
    {

        InitKAni();
        if (anicontroller == null) return;
        var _ani = GetKAnim(ani);
        if(_ani==null)
        {
            Debug.Log("ani notfound:" + ani);
        }
        var _elem = _ani.GetElement(state);

        anicontroller.State(_elem, bimmediate, whenplay);
    }
    public void PlayAni(string ani, string elem = null, KAnim.SpriteAniController.playtag tag = KAnim.SpriteAniController.playtag.play_immediate, System.Action whenplay = null)
    {

        InitKAni();
        if (anicontroller == null) return;
        anicontroller.Play(GetKAnim(ani).GetElement(elem), tag, whenplay);
    }
    public bool KAniUseController;
    public bool KAniGenColl;
    public bool KAniSound;
    public LayerMask KAniColllayer;
    public LayerMask KAniCollHitlayer;
    int KAniColllayerid = -1;
    int KAniCollHitlayerid = -1;
    void initlayid()
    {
        if (KAniColllayerid < 0)
        {
            for (int i = 0; i < 32; i++)
            {
                if (((1 << i) & KAniColllayer) > 0)
                {
                    KAniColllayerid = i;
                    break;
                    //string name = LayerMask.LayerToName(i);
                    //Debug.Log("KAniColllayerid" + name);
                }
            }
        }
        if (KAniCollHitlayerid < 0)
        {
            for (int i = 0; i < 32; i++)
            {
                if (((1 << i) & KAniCollHitlayer) > 0)
                {
                    KAniCollHitlayerid = i;
                    break;
                    //string name = LayerMask.LayerToName(i);
                    //Debug.Log("KAniCollHitlayerid" + name);
                }
            }
        }
    }
    void InitKAni()
    {
        MeshGroupGroupDataK kdata = group.data as MeshGroupGroupDataK;
        
        if (kdata != null && anicontroller == null)
        {
            string _name = this.defaultanim;
            if (System.IO.Path.DirectorySeparatorChar == '/')
            {
                _name = _name.Replace('\\', '/');
            }
            _name = System.IO.Path.GetFileNameWithoutExtension(_name);
            _name = System.IO.Path.GetFileNameWithoutExtension(_name).ToLower();
            anicontroller = new KAnim.SpriteAniController(GetKAnim(_name).GetElement(null));
            //Debug.Log("new kanicontroller");
        }
    }
    Dictionary<string, string> namemap = new Dictionary<string, string>();
    public KAnim.SpriteAni GetKAnim(string aniname)
    {

        MeshGroupGroupDataK kdata = group.data as MeshGroupGroupDataK;
        if (kdata != null)
        {
            if (namemap.ContainsKey(aniname))
            {
                return kdata.GetSpriteAni(namemap[aniname]);
            }
            else
            {
                foreach (var name in kdata.GetAllAniname())
                {
                    string _name = name;
                    if (System.IO.Path.DirectorySeparatorChar == '/')
                    {
                        _name = _name.Replace('\\', '/');
                    }
                    _name = System.IO.Path.GetFileNameWithoutExtension(_name);
                    _name = System.IO.Path.GetFileNameWithoutExtension(_name).ToLower();
                    //Debug.Log("_________Ani:" + _name + " aniname:" + aniname + ", file" + name);
                    if (_name == aniname)
                    {

                        namemap[aniname] = name;
                        return kdata.GetSpriteAni(name);
                    }
                    //if (name.ToLower().Contains(aniname))
                    //{
                    //    return kdata.GetSpriteAni(name);
                    //}
                }
            }

        }
        return null;
    }

    Dictionary<string, BoxCollider2D> collobj;
    BoxCollider2D GetSubCollObj(string name)
    {
        name = "coll:" + name;
        if (collobj.ContainsKey(name)) return collobj[name];

        Transform tran = this.transform.FindChild(name);
        if (tran == null)
        {
            GameObject obj = new GameObject();
            obj.name = name;
            obj.transform.parent = this.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.hideFlags = HideFlags.DontSave;
            collobj[name] = obj.AddComponent<BoxCollider2D>();
        }
        else
        {
            tran.localPosition = Vector3.zero;
            if (tran.gameObject.GetComponent<BoxCollider2D>() == null)
            {
                collobj[name] = tran.gameObject.AddComponent<BoxCollider2D>();
            }
            else
            {
                collobj[name] = tran.gameObject.GetComponent<BoxCollider2D>();
            }
        }

        return collobj[name];
    }
    public Bounds? GetKAniBound(string name)
    {

        KAnim.DrawFrame frame = anicontroller.GetFrame();
        foreach (var b in frame.bounds)
        {
            if (b.Key == name)
            {
                Rect? rect = frame.GetBounds(Vector2.zero, Vector2.one);
                Bounds _b = new Bounds(new Vector2((rect.Value.center.x) / 64.0f
                , -rect.Value.center.y / 64.0f)
                , new Vector2(b.Value.width / 2 / 32.0f, b.Value.height / 2 / 32.0f)
                );
                return _b;
            }
        }
        return null;
    }
    void GenKAniColl()
    {
        initlayid();
        if (collobj == null)
            collobj = new Dictionary<string, BoxCollider2D>();



        KAnim.DrawFrame frame = anicontroller.GetFrame();
        foreach (var b in collobj.Values)
        {
            b.gameObject.SetActive(false);
        }
        foreach (var b in frame.bounds)
        {
            if (b.Key.IndexOf("f:") >= 0) continue;
            if (b.Key.IndexOf("state:") >= 0) continue;

            //Rect? rect = frame.GetBounds(Vector2.zero, Vector2.one);

            BoxCollider2D box = null;
            if (b.Key == "" && KAniColllayerid > 0)
            {
                box = GetSubCollObj(b.Key);
                box.gameObject.layer = KAniColllayerid;
            }
            else if (b.Key == "hit" && KAniCollHitlayerid > 0)
            {
                box = GetSubCollObj(b.Key);
                box.gameObject.layer = KAniCollHitlayerid;
            }

            if (box != null)
            {
                box.gameObject.SetActive(true);


                Rect? _b = frame.GetBounds(Vector2.zero, Vector2.one, b.Key);
                box.center = new Vector2((_b.Value.center.x) / 64.0f
                    , -(_b.Value.center.y) / 64.0f);


                box.size = new Vector2(_b.Value.width / 2 / 32.0f, _b.Value.height / 2 / 32.0f);
            }

        }
    }
    public static event Action<string> _funcPlaySound;
    int LastKAniSoundPlayFrame = -1;
    void PlayKAniSound()
    {

        KAnim.DrawFrame frame = anicontroller.GetFrame();
        if (LastKAniSoundPlayFrame != frame.frameid)
        {

            foreach (var s in frame.sounds)
            {

                if (_funcPlaySound != null)
                {
                    _funcPlaySound(s);
                }
                else
                {
                    //Debug.Log("PlaySound:"+s);
                }
            }
        }
        LastKAniSoundPlayFrame = frame.frameid;
    }

}
