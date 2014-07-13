using UnityEngine;
using System.Collections;
using CLUI;

public class com_main : MonoBehaviour, IStateMgr
{

    // Use this for initialization
    void Start()
    {
        var _skin = Resources.Load("GUI/GUISkin001_100percent/GUISkin001") as GUISkin;
        SetSkin(_skin);
        nodeUIRoot = new CLUI_Node_Empty ("root");
        nodeUIRoot.rectScale = CLUI_Border.ScaleFill;

        this.ChangeState(new state_0Starter());
    }

    // Update is called once per frame
    void Update()
    {
        nodeUIRoot.DoUpdate();

        curstate.OnUpdate();
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
        nodeUIRoot.DoGUI();

        curstate.OnGUI();
    }

    IState curstate = null;

    public void ChangeState(IState state)
    {
        if (curstate != null)
            curstate.OnExit();

        curstate = state;
        if (curstate != null)
            curstate.OnInit(this);
    }

    public CLUI_Node nodeUIRoot
    {
        get;
        private set;
    }
    GUISkin skin;
    bool bskinUpdate = false;
    public void SetSkin(GUISkin _skin)
    {
        if (skin != _skin)
        {
            bskinUpdate = true;
            skin = _skin;
        }
    }
    int srcfontsize_label = 20;
    int srcfontsize_button = 20;
    int srcfontsize_textarea = 20;
    int srcfontsize_textfield = 20;
    int lastScreenHeight;
    public void ResetFontSize()
    {
        float s = (float)Screen.height / 480.0f;
        skin.label.fontSize = (int)(srcfontsize_label * s);
        skin.button.fontSize = (int)(srcfontsize_button * s);
        //Debug.Log(skin.button.fontSize);
        //GUI.skin = skin;
    }


}
public interface IStateMgr
{
    void ChangeState(IState state);
    CLUI_Node nodeUIRoot
    {
        get;
    }
}
public interface IState
{
    void OnInit(IStateMgr mgr);
    void OnExit();
    void OnUpdate();
    void OnGUI();
}
