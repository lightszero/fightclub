using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
public class MeshGroupMgr
{
    public static void Clear()
    {
        mapGroupGroup.Clear();
        bInit = false;
        
    }
    public static Dictionary<string, MeshGroupGroup> mapGroupGroup = new Dictionary<string, MeshGroupGroup>();
    public static MeshGroup GetMeshGroup(string filename, string aniname)
    {
        if (mapGroupGroup.ContainsKey(filename))
        {
            
            return mapGroupGroup[filename].GetMeshGroup(aniname);
        }
        else
        {
            //判断是HA角色 还是K角色
            var data = MeshGroupGroupDataHA.Create(filename);
            if (data == null)
                data = MeshGroupGroupDataK.Create(filename);
            if (data != null)
            {
                mapGroupGroup[filename] = new MeshGroupGroup(data);
                return mapGroupGroup[filename].GetMeshGroup(aniname);
            }
            else
            {
                Debug.Log("error:" + filename);
            }

        }

        return null;
    }
    public static bool BufMeshGroup(string filename)
    {
        if (mapGroupGroup.ContainsKey(filename)==false)
        {
            //判断是HA角色 还是K角色
            var data = MeshGroupGroupDataHA.Create(filename);
            if (data == null)
                data = MeshGroupGroupDataK.Create(filename);
            if (data != null)
            {
                mapGroupGroup[filename] = new MeshGroupGroup(data);
               
            }
            else
            {
                Debug.Log("error:" + filename);
                return false;
            }

        }
        return mapGroupGroup[filename].BufMeshGroup();
        
    }
    static bool bInit = false;
    public static void InitOnce()
    {
        if (bInit) return;

        #if UNITY_EDITOR
            Save();
        #else
        Load();
        #endif
    }

    static void Load()
    {
    }
    static void Save()
    {
        #if UNITY_EDITOR
        var _files = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath("."), "*.Puppet.xml", System.IO.SearchOption.AllDirectories);
        foreach (var f in _files)
        {
            string file = f;
            var i = file.ToLower().LastIndexOf("resources");
            file = file.Substring(i + "resources".Length + 1);
            var i2 = file.LastIndexOf('.');
            file = file.Substring(0, i2);   //must be。 remove ext name
            file = file.Replace('\\', '/'); //must be。 use / symble
            var data = MeshGroupGroupDataHA.Create(file);
            if (data != null)
            {
                mapGroupGroup[file] = new MeshGroupGroup(data);
            }
        }

        var _files2 = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath("."), "seeds.bin.bytes", System.IO.SearchOption.AllDirectories);
        Debug.Log("MeshGroupMgr:Save"+ _files2.Length);
        foreach (var f in _files2)
        {
            string file = f;

            var i = file.ToLower().LastIndexOf("resources");
            file = file.Substring(i + "resources".Length + 1);
            var i2 = file.LastIndexOf('.');
            file = file.Substring(0, i2);   //must be。 remove ext name
            file = System.IO.Path.GetDirectoryName(file);

            file = file.Replace('\\', '/'); //must be。 use / symble

            var data = MeshGroupGroupDataK.Create(file);
            if (data != null)
            {
                mapGroupGroup[file] = new MeshGroupGroup(data);
            }
        }
#endif
    }
}