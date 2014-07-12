using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vjoybtn : MonoBehaviour
{
    void Start()
    {
        
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(0.1f, 0.6f, 0.2f, 0.2f);
            i.id = KeyCode.J;
            i.uv = CalcUI(joyback, 64 * 4, 0, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 0, 64, 64);
            i.useHalfScreen =true;
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.rightSide = true;
            i.dest = new Rect(0.1f, 0.6f, 0.2f, 0.2f);
            i.id = KeyCode.K;
            i.uv = CalcUI(joyback, 64 * 4, 64, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 64, 64, 64);
            i.useHalfScreen = true;
            btnInfo.Add(i);
        }

    }
    // Update is called once per frame
    Vector2 lasttouch = new Vector2(0, 0);
    void Update()
    {

        foreach (var bi in btnInfo)
        {
            bi.bdown = Input.GetKey(bi.id);
        }

        //if (Input.touchCount > 0)
        //{
        //    Debug.Log("have touch." + Input.touchCount);
        //}
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved
                || Input.touches[i].phase == TouchPhase.Stationary)
            {
               
                var pos = Input.touches[i].position;
#if UNITY_WP8
                pos.y =Screen.height -pos.y;
#endif
                //Debug.Log("have touch. [" +i+"]"+pos);
                lasttouch = pos;
                foreach (var bi in btnInfo)
                {
                    Rect r = bi.GetScreenRectTouch();
                    if (bi.bdown == false && r.Contains(pos))
                        bi.bdown = true;
                }
            }
        }
#if UNITY_FLASH
#else
        //需要的时候得模拟一下
        //Debug.Log(Input.multiTouchEnabled+"|"+ Input.mousePresent);
        if (Input.touchCount == 0)
        {
            Vector2 pos = Input.mousePosition;
            lasttouch = pos;
            pos.y = Screen.height - pos.y;
            if (Input.GetMouseButton(0))
            {
                foreach (var bi in btnInfo)
                {

                    if (Input.GetMouseButton(0))
                    {
                        Rect r = bi.GetScreenRectTouch();
                        if (bi.bdown == false && r.Contains(pos))
                            bi.bdown = true;
                        //bi.bdown = bi.dest.Contains(pos);
                    }

                }
            }
        }
#endif
    }
    //NewBehaviourScript b;
    public Texture2D joyback;
    Rect CalcUI(Texture2D tex, int x, int y, int width, int height)
    {
        if(tex==null)return new Rect(0,0,1,1);
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
        //open rightSide ,
        public bool rightSide = false;
        public Rect dest;
        public bool useHalfScreen=false;
        public Rect uv;
        public Rect uvdown;
        public bool bdown = false;
        public Rect GetScreenRect()
        {
            Rect rout = new Rect();
            rout.x = (float)Screen.width * dest.x;
            rout.y = (float)Screen.height * dest.y;
            rout.width = (float)Screen.height * dest.width;
            rout.height = (float)Screen.height * dest.height;
            if (rightSide)
                rout.x = Screen.width - rout.x - rout.width;

            return rout;
        }
        public Rect GetScreenRectTouch()
        {
            if (useHalfScreen)
            {
                if (rightSide)
                {
                    return new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
                }
                else
                {
                    return new Rect(0, 0, Screen.width / 2, Screen.height);
                }
            }
            else
            {
                return GetScreenRect();
            }
        }
    }
    public static List<BtnInfo> btnInfo = new List<BtnInfo>();


    float timer = 0.0f;
    void OnGUI()
    {
        if (joyback == null) return;
               
        foreach (var bi in btnInfo)
        {
            Rect r = bi.GetScreenRect();

            if (bi.bdown)
            {
                GUI.DrawTextureWithTexCoords(r, joyback, bi.uvdown);
            }
            else
            {
                GUI.DrawTextureWithTexCoords(r, joyback, bi.uv);
            }
        }

    }
}
