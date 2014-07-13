using UnityEngine;
using System.Collections;
using CLUI;

public class state_1PacketList : IState
{


    IStateMgr mgr;
    public void OnInit(IStateMgr mgr)
    {
        this.mgr = mgr;

        var back = new CLUI.CLUI_Node_Box("back");
        back.rectScale = CLUI_Border.ScaleFill;
        mgr.nodeUIRoot.AddNode(back);
        var btn01=new CLUI.CLUI_Node_Button("btn01");
        btn01.rectScale = new CLUI_Border(0.1f, 0.1f, 0.5f, 0.2f);
        btn01.text = "hello";
        mgr.nodeUIRoot.AddNode(btn01);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnGUI()
    {

    }
}
