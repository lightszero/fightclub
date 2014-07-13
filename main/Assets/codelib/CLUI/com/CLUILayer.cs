using UnityEngine;
using System.Collections;
using CLUI;

public class CLUILayer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        nodeRoot = new CLUI_Node_Box("root");
        nodeRoot.rectScale = CLUI_Border.ScaleFill;
    }
    // Update is called once per frame
    void Update()
    {
        nodeRoot.DoUpdate();
    }
    public static CLUI_Node nodeRoot
    {
        get;
        private set;
    }
    static GUISkin skin;
    static bool bskinUpdate = false;
    public static void SetSkin(GUISkin _skin)
    {
        if (skin != _skin)
        {
            bskinUpdate = true;
            skin = _skin;
        }
    }
    static int srcfontsize_label=20;
    static int srcfontsize_button=20;
    static int srcfontsize_textarea = 20;
    static int srcfontsize_textfield = 20;
    static int lastScreenHeight;
    public static void ResetFontSize()
    {
        float s = (float)Screen.height / 480.0f;
        skin.label.fontSize =(int) (srcfontsize_label * s);
        skin.button.fontSize =(int)( srcfontsize_button*s);
        //Debug.Log(skin.button.fontSize);
        //GUI.skin = skin;
    }
    void OnGUI()
    {
        if (Screen.height != lastScreenHeight)
        {
            ResetFontSize();
            lastScreenHeight = Screen.height;
        }


        if (bskinUpdate)
            GUI.skin = skin;
        nodeRoot.DoGUI();
    }
}
