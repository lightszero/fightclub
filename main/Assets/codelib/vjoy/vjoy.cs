using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vjoy : MonoBehaviour
{

    // Use this for initialization
    Vector2 centerpos = new Vector2(0, 0);
    float joysize = 0;
    void Start()
    {

        float dpi = Screen.dpi;
        if (dpi == 0) dpi = defDPI;

        float size = dpi * defSIZE;
        joysize = size;

        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3, -size / 3 * 2.5f, size / 3, size / 3);
            i.id = KeyCode.J;
            i.buttonid = "Fire2";
            i.uv = CalcUI(joyback, 64 * 4, 0, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 0, 64, 64);
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3 * 2, -size / 3 * 2.5f, size / 3, size / 3);
            i.id = KeyCode.K;
            i.buttonid = "Fire1";
            i.uv = CalcUI(joyback, 64 * 4, 64, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 64, 64, 64);
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3 * 3, -size / 3 * 2.5f, size / 3, size / 3);
            i.id = KeyCode.L;
            i.buttonid = "Jump";
            i.uv = CalcUI(joyback, 64 * 4, 128, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 128, 64, 64);
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(size / 3 * -0.5f + size / 3, size / 3 * 0.5f, size / 3, size / 3);
            i.screenpos = Vector2.zero;
            i.id = KeyCode.Alpha1;
            i.buttonid = "Triger";
            i.uv = CalcUI(joyback, 64 * 4 + 128, 0, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 192, 0, 64, 64);
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(size / 3 * -0.5f + size / 3 * 2, size / 3 * 0.5f, size / 3, size / 3);
            i.screenpos = Vector2.zero;
            i.id = KeyCode.Alpha2;
            i.buttonid = "L";
            i.uv = CalcUI(joyback, 64 * 4 + 128, 64, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 192, 64, 64, 64);
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(size / 3 * -0.5f + size / 3 * 3, size / 3 * 0.5f, size / 3, size / 3);
            i.screenpos = Vector2.zero;
            i.id = KeyCode.Alpha3;
            i.buttonid = "R";
            i.uv = CalcUI(joyback, 64 * 4 + 128, 128, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 192, 128, 64, 64);
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3 , size / 3 * 0.5f, size / 3*2, size / 3);
            i.screenpos = new Vector2(1, 0);
            i.id = KeyCode.Escape;
            i.buttonid = "Menu";
            i.uv = CalcUI(joyback, 0, 256, 128, 64);
            i.uvdown = CalcUI(joyback, 128, 256, 128, 64);
            btnInfo.Add(i);
        }
    }
    bool wdown = false;
    bool sdown = false;
    bool adown = false;
    bool ddown = false;
    public static Vector2 joydir = new Vector2(0, 0);
    public bool bMenu = false;
    // Update is called once per frame
    Vector2 lasttouch = new Vector2(0, 0);
    bool bUseJoyExt = true;
    void Update()
    {
        wdown = false;
        sdown = false;
        adown = false;
        ddown = false;
        joydir.x = 0;
        joydir.y = 0;
        float axitx = Input.GetAxis("Horizontal");
        float axity = Input.GetAxis("Vertical");
        bool btriger = false;
        bool bL = false;
        bool bR = false;
        if (bUseJoyExt)
        {
            if (Input.GetAxis("DPadX") != 0) axitx = Input.GetAxis("DPadX");
            if (Input.GetAxis("DPadY") != 0) axity = Input.GetAxis("DPadY");
            if (Input.GetAxis("Triger") >0.8 ) btriger = true;
            bL = Input.GetButton("L");
            bR = Input.GetButton("R");
            bMenu = Input.GetButton("Menu");
        }
        
        if(!bMenu&& (Input.GetKey(KeyCode.Menu)||Input.GetKey(KeyCode.Escape)))
        {
            bMenu = true;
        }
        //float axitx2 = Input.GetAxis("6");
        //float axity2 = Input.GetAxis("7");
        joydir.x = axitx;
        joydir.y = axity;

        if (Input.GetKey(KeyCode.W))
        {
            wdown = true;

            joydir.y = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            sdown = true;
            joydir.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            adown = true;
            joydir.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            ddown = true;
            joydir.x = 1;
        }


        foreach (var bi in btnInfo)
        {
            bi.bdown = Input.GetKey(bi.id);
            if (!bi.bdown && string.IsNullOrEmpty(bi.buttonid) == false)
            {
                if (bi.buttonid == "Triger")
                {
                    bi.bdown = btriger;
                }
                else if(bi.buttonid =="L")
                {
                    bi.bdown = bL;
                }
                else if (bi.buttonid == "R")
                {
                    bi.bdown = bR;
                }
                else if(bi.buttonid=="Menu")
                {
                    if(bMenu)
                        bi.bdown = true ;
                    if(bi.bdown)
                        bMenu = true;
                }
                else
                {
                    bi.bdown = Input.GetButton(bi.buttonid);
                }
            }
        }

        bool bMapKey = true;
        if (joydir.x == 0 && joydir.y == 0 && Pos2Key)
        {
            bMapKey = true;//打开位置映射

        }
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved
                || Input.touches[i].phase == TouchPhase.Stationary)
            {
                var pos = Input.touches[i].position;

                lasttouch = pos;
                pos.y = Screen.height - pos.y;

                if (Vector2.Distance(pos, centerpos) < joysize)
                {
                    Vector2 dir = (pos - centerpos);
                    dir.y *= -1;
                    float len = dir.magnitude / joysize * 4;
                    dir.Normalize();
                    dir *= len;
                    joydir += dir;
                }
                foreach (var bi in btnInfo)
                {
                    Rect r = bi.dest;
                    r.x += Screen.width * bi.screenpos.x;
                    r.y += Screen.height * bi.screenpos.y;
                    if (bi.bdown == false && r.Contains(pos))
                        bi.bdown = true;
                }
            }
        }
#if UNITY_FLASH
#else

        //需要的时候得模拟一下
        if (Input.multiTouchEnabled == false)
        {
            Vector2 pos = Input.mousePosition;
            lasttouch = pos;
            pos.y = Screen.height - pos.y;
            if (Input.GetMouseButton(0))
            {
                if (Vector2.Distance(pos, centerpos) < joysize)
                {
                    Vector2 dir = (pos - centerpos);
                    dir.y *= -1;
                    float len = dir.magnitude / joysize * 4;
                    dir.Normalize();
                    dir *= len;
                    joydir += dir;
                }
                foreach (var bi in btnInfo)
                {

                    if (Input.GetMouseButton(0))
                    {
                        Rect r = bi.dest;
                        r.x += Screen.width * bi.screenpos.x;
                        r.y += Screen.height * bi.screenpos.y;
                        if (bi.bdown == false && r.Contains(pos))
                            bi.bdown = true;
                        //bi.bdown = bi.dest.Contains(pos);
                    }

                }
            }
        }
#endif
        if (joydir.magnitude > 1.0f)
        {
            joydir.Normalize();
        }
        if (bMapKey)
        {
            if (-joydir.y < -0.5f) wdown = true;
            if (-joydir.y > 0.5f) sdown = true;
            if (joydir.x < -0.5f) adown = true;
            if (joydir.x > 0.5f) ddown = true;
        }
    }
    //NewBehaviourScript b;
    public Texture2D joyback;
    public int defDPI = 120;            //当检测不到屏幕dpi时使用的DPI
    public float defSIZE = 1.0f;        //摇杆大小、英寸
    public bool Pos2Key = true;         //是否将位置映射回按键
    Rect CalcUI(Texture2D tex, int x, int y, int width, int height)
    {
        Rect rect = new Rect();
        rect.x = (float)(x + 0.5f) / (float)tex.width;
        rect.y = 1.0f - (float)(y + height - 0.5f) / (float)tex.height;
        rect.width = (float)(width - 0.5f) / (float)tex.width;
        rect.height = (float)(height - 1.0f) / (float)tex.height;
        return rect;


    }
    void DrawRect(Vector2 pos, float size, int x, int y, int width, int height)
    {
        Rect src = CalcUI(joyback, x, y, width, height);
        GUI.DrawTextureWithTexCoords(new Rect(pos.x - size / 2, pos.y - size / 2, size, size), joyback, src);
    }

    public class BtnInfo
    {
        public KeyCode id;
        public string buttonid;
        public Vector2 screenpos = Vector2.one;
        public Rect dest;
        public Rect uv;
        public Rect uvdown;
        public bool bdown = false;
    }
    public static List<BtnInfo> btnInfo = new List<BtnInfo>();
    void OnGUI()
    {

        float dpi = Screen.dpi;
        if (dpi == 0) dpi = defDPI;

        float centrex = 0;
        float centrey = 0;
        float size = dpi * defSIZE;
        {
            float left = size / 3.0f / 2;
            float top = Screen.height - size / 3.0f / 2 - size;
            centrex = left + size * 0.5f;
            centrey = top + size * 0.5f;
            centerpos.x = centrex;
            centerpos.y = centrey;
            //back
            //GUI.DrawTextureWithTexCoords(new Rect(left, top, size, size), joyback, CalcUI(joyback,0, 0, 64*3,64*3));
            DrawRect(new Vector2(centrex, centrey), size, 0, 0, 64 * 3, 64 * 3);
        }


        //w
        DrawRect(new Vector2(centrex, centrey - size / 3), size / 3, wdown ? 64 : 0, 64 * 3, 64, 64);

        //s
        DrawRect(new Vector2(centrex, centrey + size / 3), size / 3, sdown ? 64 * 2 + 64 : 64 * 2, 64 * 3, 64, 64);

        //a
        DrawRect(new Vector2(centrex - size / 3, centrey), size / 3, adown ? 128 * 2 + 64 : 128 * 2, 64 * 3, 64, 64);

        //d
        DrawRect(new Vector2(centrex + size / 3, centrey), size / 3, ddown ? 192 * 2 + 64 : 192 * 2, 64 * 3, 64, 64);


        //point
        DrawRect(new Vector2(centrex + joydir.x * size / 4, centrey - joydir.y * size / 4), size / 3, 64 * 3, 64, 64, 64);



        //GUI.Label(new Rect(0, 0, Screen.width / 2, 100), "中文试验DPI=" + dpi + " W=" + Screen.width + " h=" + Screen.height + " btn=" + btndown + " lasttouch=" + lasttouch);

        foreach (var bi in btnInfo)
        {
            Rect r = bi.dest;
            r.x = Screen.width * bi.screenpos.x + r.x;
            r.y = Screen.height * bi.screenpos.y + r.y;
            if (bi.bdown)
            {
                GUI.DrawTextureWithTexCoords(r, joyback, bi.uvdown);
            }
            else
            {
                GUI.DrawTextureWithTexCoords(r, joyback, bi.uv);
            }
        }
        //this.transform.localToWorldMatrix
    }
}
