using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using CLUI;

public class state_0Starter : IState
{
    #region console
#if UNITY_STANDALONE

    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();
    [DllImport("kernel32.dll",
        EntryPoint = "GetStdHandle",
        SetLastError = true,
        CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    private const int STD_OUTPUT_HANDLE = -11;
    /// <summary>
    /// 释放控制台
    /// </summary>
    /// <returns></returns>
    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();
#endif

    void InitConsole()
    {
#if UNITY_STANDALONE

        FreeConsole();
        AllocConsole();
        IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
        System.IO.FileStream fileStream = new System.IO.FileStream(safeFileHandle, System.IO.FileAccess.Write);
        System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(Console.OutputEncoding.CodePage);
        System.IO.StreamWriter standardOutput = new System.IO.StreamWriter(fileStream, encoding);
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
        Application.RegisterLogCallbackThreaded((text, trace, type) =>
            {
                if (type == LogType.Error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (type == LogType.Warning)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine(text);
            });
#endif

    }
    #endregion
    IStateMgr mgr;
    CLUI_Node_Texture back;
    public void OnInit(IStateMgr mgr)
    {
        this.mgr = mgr;
        //if (Application.isEditor == false)
        {
            InitConsole();
        }
        back =new CLUI_Node_Texture("back");
        back.rectScale = CLUI_Border.ScaleFill;
        back.texture = Resources.Load<Texture2D>("back");
        mgr.nodeUIRoot.AddNode(back);
        Debug.LogWarning("state_0Starter::OnInit");
        //从网络加载
    }

    public void OnExit()
    {
        Debug.Log("state_0Starter::Exit");
        mgr.nodeUIRoot.ClearNode();
    }
    float timer = 0;
    public void OnUpdate()
    {
        timer += Time.deltaTime;
        float alpha = (2.0f - timer) *0.5f;
        back.color.a = alpha;
        if (timer > 2.0f)
        {
            timer -= 1.0f;
            mgr.ChangeState(new state_1PacketList());
        }

    }

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 500, 100), "editor=" + Application.isEditor);
    }
}
