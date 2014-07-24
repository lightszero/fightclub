using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;


public class PacketDesc
{
    public string productName = "";
    public string version = "";
    public string versiondesc = "";
    public Dictionary<string, string> languages = new Dictionary<string, string>();
    public Dictionary<string, string> nameWithLanguage = new Dictionary<string, string>();
    public static PacketDesc FromString(string name, string text)
    {
        PacketDesc desc = new PacketDesc();
        {
            desc.productName = name;
            string[] lines = text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines)
            {
                var linfo = l.Split(new string[] { "=", "," }, StringSplitOptions.RemoveEmptyEntries);
                switch (linfo[0])
                {
                    case "ver":
                        desc.version = linfo[1];
                        break;
                    case "verinfo":
                        desc.versiondesc = linfo[1];
                        break;
                    case "language":
                        {
                            string tag = linfo[1];
                            string lname = linfo[2];
                            string lpname = linfo[3];
                            desc.languages[tag] = lname;
                            desc.nameWithLanguage[tag] = lpname;
                        }
                        break;

                }
            }
        }
        return desc;
    }
    public string ConvertToString()
    {
        string text = "";
        if (string.IsNullOrEmpty(version) == false)
            text += "ver=" + version + "\n";
        if (string.IsNullOrEmpty(versiondesc) == false)
            text += "verinfo=" + versiondesc + "\n";
        foreach (var tag in languages.Keys)
        {
            text += "language," + tag + "=" + languages[tag] + "," + nameWithLanguage[tag] + "\n";
        }
        return text;
    }
}
public class PacketList
{
    public void GetPacketListFromServer()
    {

        //应付Oray域名转向的方法
        //第一次请求 会返回一个Set-Cookie
        string cook = null;
        {
            System.Net.HttpWebRequest req = (HttpWebRequest)System.Net.HttpWebRequest.Create("http://w2.cltri.com");

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            cook = response.Headers["Set-Cookie"];
            Debug.Log("cook=" + cook);
            Debug.Log(response.ResponseUri + "(" + response.ContentLength + ")" + response.StatusCode + " " + response.StatusDescription);

            System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
            string text = sr.ReadToEnd();
            Debug.Log(text);
        }
        string[] cc = cook.Split(new string[] { ";", " " }, System.StringSplitOptions.RemoveEmptyEntries);
        string cpath = "";
        Dictionary<string, string> cookie = new Dictionary<string, string>();
        foreach (var c in cc)
        {
            Debug.Log("c=" + c);

            string[] v = c.Split('=');
            if (v[0] == "path")
            {
                cpath = v[1];
                continue;
            }
            cookie[v[0]] = v[1];
        }
        //第二次请求用此SetCookie的值再请求才能返回正确值
        {
            System.Net.HttpWebRequest req = (HttpWebRequest)System.Net.HttpWebRequest.Create("http://w2.cltri.com");
            req.CookieContainer = new CookieContainer();
            foreach (var c in cookie)
            {
                req.CookieContainer.Add(new Cookie(c.Key, c.Value, cpath, req.RequestUri.Host));
            }
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            cook = response.Headers["Set-Cookie"];
            Debug.Log(response.ResponseUri + "(" + response.ContentLength + ")" + response.StatusCode + " " + response.StatusDescription);

            foreach (string k in response.Headers)
            {
                Debug.Log("k=" + k + " v= " + response.Headers[k]);
            }
            System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
            string text = sr.ReadToEnd();
            Debug.Log(text);// .Substring(0,100));
        }
        //www = new WWW("http://w1.cltri.com");
    }

    public void InitPacketList(bool useServer)
    {

    }
    public string GetPackPathLocal()
    {
        string pakpath = "";
        if (Application.isEditor)
        {
            pakpath = System.IO.Path.GetFullPath("../game/paks");
        }
        else
        {
            pakpath = System.IO.Path.GetFullPath("paks");
        }
        Debug.Log("PackPathLocal = " + pakpath);

        return pakpath;
    }
    public IList<string> InitPacketListLocal()
    {
        IList<string> paks =new List<string>();
        string pakpath = GetPackPathLocal();

        string[] pakdesc = System.IO.Directory.GetFiles(pakpath, "packet.desc.txt", System.IO.SearchOption.AllDirectories);

        foreach (var p in pakdesc)
        {

            string path = System.IO.Path.GetDirectoryName(p);

            string pakname = path.Substring(pakpath.Length+1);

            paks.Add(pakname);

        }
        return paks;
    }
}
//public class Packet
//{
//    public Packet(string localurl)
//    {
//        ResmgrNative.Instance.SetCacheUrl()
//    }

//    public static IList<Packet> FindPacket_LocalPath()
//    {
//        return null;
//    }

//    //Desc desc;
//    public void LoadDesc(Action<Packet> onLoad)
//    {

//    }
//}

