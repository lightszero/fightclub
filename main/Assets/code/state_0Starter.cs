using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class state_0Starter : IState
{
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

    void InitConsole()
    {
        AllocConsole();
        IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
        System.IO.FileStream fileStream = new System.IO.FileStream(stdHandle, System.IO.FileAccess.Write);
        System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(Console.OutputEncoding.CodePage);
        System.IO.StreamWriter standardOutput = new System.IO.StreamWriter(fileStream, encoding);
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }
    IStateMgr mgr;
    public void OnInit(IStateMgr mgr)
    {
        this.mgr = mgr;
        Debug.developerConsoleVisible = true;
        if (Application.isEditor == false)
        {
            InitConsole();
        }
        Debug.Log("state_0Starter:OnInit");
        System.Console.WriteLine("state_0Starter:OnInit");
#if UNITY_STANDALONE 

        //从磁盘加载

#endif
        //从网络加载
    }

    public void OnExit()
    {

    }
    float timer = 0;
    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 1.0f)
        {
            timer -= 1.0f;
            Debug.Log("state_0Starter:OnUpdate");
            System.Console.WriteLine("state_0Starter:OnUpdate");
        }

    }

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 500, 100), "editor=" + Application.isEditor);
    }
}
