using UnityEngine;
using System.Collections;

public class com_main : MonoBehaviour, IStateMgr
{

    // Use this for initialization
    void Start()
    {
        this.ChangeState(new state_0Starter());
    }

    // Update is called once per frame
    void Update()
    {
        curstate.OnUpdate();
    }
    void OnGUI()
    {
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
}
public interface IStateMgr
{
    void ChangeState(IState state);
}
public interface IState
{
    void OnInit(IStateMgr mgr);
    void OnExit();
    void OnUpdate();
    void OnGUI();
}
