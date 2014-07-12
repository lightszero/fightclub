using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;

    public struct TextureBlock
    {
        public Texture2D tex
        {
            get
            {
                return __tex;
            }

        }
        public Rect uv
        {
            get
            {
                return __uv;
            }
        }
        private Texture2D __tex;
        private Rect __uv;
        public override string ToString()
        {
            return tex.name + ":" + __uv.xMin + "," + __uv.yMin + "," + __uv.xMax + "," + __uv.yMax;
        }
        private TextureBlock(Texture2D _tex, Rect _uv)
        {
            this.__tex = _tex;
            this.__uv = _uv;
        }
        public static TextureBlock Create(Texture2D tex, Rect uv)
        {
            TextureBlock tb = new TextureBlock(tex, uv);
            return tb;
        }
        public static TextureBlock Empty
        {
            get
            {
                return new TextureBlock(null,new Rect(0,0,0,0));
            }
            private set
            {
            }
        }
        public TextureBlock Create(Rect uv)
        {
            return new TextureBlock(__tex, uv);
        }
        
    }

    public class TextureBlockMgr
    {
        public class DetailBlock
        {
            public Dictionary<string, Rect> details = new Dictionary<string, Rect>();
        }
        static bool bInit =false;
        public static void InitOnce()
        {
            if(bInit)return;
			bInit=true;
            srcTexs.Clear();
            allBlocks.Clear();
            //Debug.Log("Init TextureBlockMgr.");
#if UNITY_EDITOR 
            string[] resources = System.IO.Directory.GetDirectories(".", "Resources", System.IO.SearchOption.AllDirectories);
            foreach (var r in resources)
            {
                //Debug.Log(r);
                if (r == null) continue;
                TextureBlockMgr.AddPath(r);
            }
			Save();
#else
			
			Load();
#endif
        }
		const string texsfilenameXML="texs";
		const string texspath="Assets/Resources";
		static void Load()
		{
            var texsdata = Resources.Load(texsfilenameXML, typeof(TextAsset)) as TextAsset;
            
			var texs=texsdata.text.Split(new char[]{ '\r','\n'},StringSplitOptions.RemoveEmptyEntries);
			foreach(var file in texs)
			{
                if (file.Contains("sheet"))
                {
                    var tex2 = Resources.Load(file, typeof(TextAsset)) as TextAsset;
                    var tdata = TextureAtlas.CreateFromStringXML(tex2.text);
                    //selectTex = Resources.Load(filename, typeof(Texture2D)) as Texture2D;

                    srcTexs[file] = null;
                    allBlocks[file] = new DetailBlock();
                    foreach (var s in tdata.sprites)
                    {
                        allBlocks[file].details[s.n.ToLower()] = new Rect((float)s.x / (float)tdata.width, (float)s.y / (float)tdata.height, (float)s.w / (float)tdata.width, (float)s.h / (float)tdata.height);
                    }
                }
                else
                {
                    var tex2 = Resources.Load(file+".packet.csv", typeof(TextAsset)) as TextAsset;
                    var tdata = TextureAtlas.CreateFromStringCSV(tex2.text);
                    //selectTex = Resources.Load(filename, typeof(Texture2D)) as Texture2D;

                    srcTexs[file] = null;
                    allBlocks[file] = new DetailBlock();
                    foreach (var s in tdata.sprites)
                    {
                        allBlocks[file].details[s.n.ToLower()] = new Rect((float)s.x / 8192.0f, (float)s.y / 8192.0f, (float)s.w / 8192.0f, (float)s.h / 8192.0f);
                    }
                }
				Debug.Log("add tex="+file);
			}
		}
		static void Save()
		{
            #if UNITY_EDITOR
			var names=	TextureBlockMgr.getAllTextureFileName();
			if(System.IO.Directory.Exists(texspath)==false)
			{
				System.IO.Directory.CreateDirectory(texspath);
			}
            System.IO.File.WriteAllLines(System.IO.Path.Combine(texspath, texsfilenameXML + ".txt"), names);
            #endif
		}
        public static void ReInit()
        {
            bInit = false;
            InitOnce();
        }
        public static void AddPath(string path)
        {
            #if UNITY_EDITOR
            //Debug.Log("AddPath TextureBlockMgr."+ path);
            var _files = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath( path), "sheet*.xml", System.IO.SearchOption.AllDirectories);
            foreach (var f in _files)
            {
                string file = f.ToLower();
                var i = file.ToLower().LastIndexOf("resources");
                file = file.Substring(i + "resources".Length + 1);
                var i2 = file.LastIndexOf('.');
                file = file.Substring(0, i2);   //must be。 remove ext name
                file = file.Replace('\\', '/'); //must be。 use / symble

                var tex2 = Resources.Load(file, typeof(TextAsset)) as TextAsset;
                var tdata = TextureAtlas.CreateFromStringXML(tex2.text);
                //selectTex = Resources.Load(filename, typeof(Texture2D)) as Texture2D;

                srcTexs[file] = null;
                allBlocks[file] = new DetailBlock();
                foreach (var s in tdata.sprites)
                {
                    allBlocks[file].details[s.n.ToLower()] = new Rect((float)s.x / (float)tdata.width, (float)s.y / (float)tdata.height, (float)s.w / (float)tdata.width, (float)s.h / (float)tdata.height);
                }
                Debug.Log("Init TextureBlockMgr." +f);
            }

            var _filescsv = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath(path), "*.csv.txt", System.IO.SearchOption.AllDirectories);
            foreach (var f in _filescsv)
            {
                string file = f.ToLower();
                var i = file.ToLower().LastIndexOf("resources");
                file = file.Substring(i + "resources".Length + 1);
                var i2 = file.LastIndexOf('.');
                file = file.Substring(0, i2);   //must be。 remove ext name
                file = file.Replace('\\', '/'); //must be。 use / symble


                var tex2 = Resources.Load(file, typeof(TextAsset)) as TextAsset;
                var tdata = TextureAtlas.CreateFromStringCSV(tex2.text);
                ////selectTex = Resources.Load(filename, typeof(Texture2D)) as Texture2D;

                var name = file.Split('.')[0];
                srcTexs[name] = null;
                allBlocks[name] = new DetailBlock();
                foreach (var s in tdata.sprites)
                {
                    allBlocks[name].details[s.n.ToLower()] = new Rect((float)s.x / 8192.0f, (float)s.y / 8192.0f, (float)s.w / 8192.0f, (float)s.h / 8192.0f);
                }
                //Debug.Log("csv=" + file);
            }
            #endif
        }
        public static string[] getAllTextureFileName()
        {
           List<string> strs =new List<string>(srcTexs.Keys);
           return strs.ToArray();
        }
        public static DetailBlock getDetail(string texfile)
        {
            if (allBlocks.ContainsKey(texfile.ToLower()))
            {

                return allBlocks[texfile.ToLower()];
            }
            return null;
        }
        static Dictionary<string, TextureBlock?> srcTexs = new Dictionary<string, TextureBlock?>();

        static Dictionary<string, DetailBlock> allBlocks = new Dictionary<string, DetailBlock>();
        static public TextureBlock? getTexture(string name)
        {
            var key =name.ToLower();
            if (srcTexs.ContainsKey(key))
            {
                if (srcTexs[key] == null)
                {
                    var tex =Resources.Load(name,typeof(Texture2D)) as Texture2D;
                    srcTexs[key] = TextureBlock.Create(tex,new Rect(0,0,1,1));
                }
                return srcTexs[name].Value;
            }
            foreach(var v in allBlocks)
            {
                if(v.Value.details.ContainsKey(key))
                {
                    if (srcTexs[v.Key] == null)
                    {
                        var tex = Resources.Load(v.Key, typeof(Texture2D)) as Texture2D;
                        srcTexs[v.Key] = TextureBlock.Create(tex, new Rect(0, 0, 1, 1));
                    }
                    var texs=     srcTexs[v.Key].Value.tex;
                    Rect suv = v.Value.details[key];
                    //suv.x /= texs.width;
                    //suv.y /= texs.height;
                    //suv.width /= texs.width;
                    //suv.height /= texs.height;
                    suv.y = 1 - suv.y - suv.height;
                    return srcTexs[v.Key].Value.Create(suv);
                }
            }
            return null;
        }
    }

