using UnityEngine;
using System.Collections;

public class state_1PacketList : IState
{


    IStateMgr mgr;
    public void OnInit(IStateMgr mgr)
    {
        this.mgr = mgr;
#if UNITY_STANDALONE

        //从磁盘加载

#endif
        //从网络加载
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
