using System;
using System.Collections.Generic;

using System.Text;


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

