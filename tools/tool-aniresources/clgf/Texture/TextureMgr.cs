using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using RectangleF = clgf.type.RectangleF;
using Nuclex.Game.Packing;


namespace clgf.Texture
{


    public class TexturePacket
    {
        public Texture2D texture
        {
            get;
            private set;
        }
        //public class SaveInfo
        //{
        //    public string rectname;
        //    public RectangleF rect;
        //}
        public void Save(System.IO.Stream s)
        {
            System.IO.StreamWriter ww = new System.IO.StreamWriter(s,Encoding.UTF8);
            //List<SaveInfo> save = new List<SaveInfo>();
            foreach (var b in blocks)
            {
                int x =(int)(b.Value.X*8192);
                int y = (int)(b.Value.Y * 8192);
                int w = (int)(b.Value.Width * 8192);
                int h = (int)(b.Value.Height * 8192);
                ww.WriteLine(b.Key + "," + x + "," + y + "," + w + "," + h);
                //SaveInfo si = new SaveInfo();
                //si.rectname = b.Key;
                //si.rect = b.Value;
                //save.Add(si);
            }
            ww.Flush();
            //System.Xml.Serialization.XmlSerializer xmls = new System.Xml.Serialization.XmlSerializer(save.GetType());
            //xmls.Serialize(s, save);
        }
        private Dictionary<string, RectangleF> blocks = new Dictionary<string, RectangleF>();
        public  TexturePacket(Texture2D _tex, Dictionary<string, RectangleF> _blocks)
        {
            blocks = _blocks;
            ReLoad(_tex);
        }
        public void ReLoad(Texture2D _tex)
        {
            texture = _tex;
            refZero = false;
        }
        public TextureBlock GetTextureBlock(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new TextureBlock(this, new RectangleF(0, 0, 1.0f, 1.0f));
            }
            else if(blocks.ContainsKey(name))
            {
                return new TextureBlock(this, blocks[name]);
            }
            return null;
        }
        private int refCount = 0;
        internal  void AddRef()
        {
            refCount++;
        }
        internal void RemoveRef()
        {
            refCount--;
            if (refCount <= 0)
            {
                refZero = true;
                //texture.Dispose();
                texture = null;
            }
        }
        public bool refZero
        {
            get;
            private set;
        }

        //贴图装箱算法
        public static IList<TexturePacket> PacketTexture(GraphicsDevice gdevice,IDictionary<string,Texture2D> sources)
        {
            List<TexturePacket> list = new List<TexturePacket>();

            int packetsize = 512;
            RectanglePacker packer = new ArevaloRectanglePacker(packetsize, packetsize);
            Texture2D tex = new Texture2D(gdevice, packer.PackingAreaWidth, packer.PackingAreaHeight, false, SurfaceFormat.Color);
            Dictionary<string, RectangleF> rects = new Dictionary<string, RectangleF>();
            foreach (var i in sources)
            {
               UInt32[] data=new UInt32[i.Value.Width*i.Value.Height];
               i.Value.GetData<UInt32>(data);
               int twidth = i.Value.Width;
               int theight = i.Value.Height;
                bool bfixx=false;
                bool bfixy = false;
                if((twidth== packetsize/2||twidth==packetsize/4))
                {
                }
                else
                {
                    bfixx=true;
                    twidth+=2;
                }
                if (theight==packetsize/2||theight==packetsize/4)
                {

                }
                else
                {
                    theight += 2;
                    bfixy = true;
                }
                Point pos;
            
                if (packer.TryPack(twidth, theight, out pos))
                {
                    //right
                }
                else
                {
                    TexturePacket p = new TexturePacket(tex, rects);
                    list.Add(p);
                    packer = new ArevaloRectanglePacker(packetsize, packetsize);
                    tex = new Texture2D(gdevice, packer.PackingAreaWidth, packer.PackingAreaHeight, false, SurfaceFormat.Color); 
                    rects = new Dictionary<string, RectangleF>();

                    packer.TryPack(i.Value.Width, i.Value.Height, out pos);
                }
                if (bfixx) pos.X += 1;
                if(bfixy)    pos.Y+=1;

                tex.SetData<UInt32>(0, new Rectangle(pos.X, pos.Y, i.Value.Width, i.Value.Height), data, 0, data.Length);
                rects.Add(i.Key, new RectangleF(
                    (float)(pos.X) / (float)packer.PackingAreaWidth,
                    (float)(pos.Y) / (float)packer.PackingAreaHeight, 
                    (float)i.Value.Width / (float)packer.PackingAreaWidth, 
                    (float)i.Value.Height / (float)packer.PackingAreaHeight
                    )); 
            }
            {
                TexturePacket p = new TexturePacket(tex, rects);
                list.Add(p);
            }
            return list;
        }
        public class TextureBlock
        {
            public TexturePacket parent
            {
                get;
                private set;
            }
            public RectangleF uv
            {
                get;
                private set;
            }
            public TextureBlock(TexturePacket _p,RectangleF _uv)
            {
                parent = _p;
                parent.AddRef();
                uv = _uv;
            }
            ~TextureBlock()
            {
                parent.RemoveRef();
            }
            public void Draw(Vector2 orient,Vector2 pos,float rotate,Vector2 size)
            {
            }
        }
    }
    public class TextureMgr
    {
        GraphicsDevice device;
        ContentManager content;
        string rootpath;
        private Dictionary<string, TexturePacket> mapNamedTex=new Dictionary<string,TexturePacket>();   //定义贴图
        private Dictionary<string, string> mapFileInTex=new Dictionary<string,string>();//定义文件在那个贴图中
        //private Dictionary<string, List<string>> groups;//贴图组管理

        /// <summary>
        /// 得到一张贴图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public TexturePacket.TextureBlock GetTexture(string name)
        {
            string filename;
            string blockname=name;
            if (mapFileInTex.ContainsKey(name))
            {
                filename = mapFileInTex[name];
            }
            else
            {
                filename = name;
                blockname = "";
            }
            if (mapNamedTex.ContainsKey(filename))
            {
                if (mapNamedTex[filename].texture == null)
                {
                    mapNamedTex[filename].ReLoad(LoadTexFromFile(filename));
                }
                return mapNamedTex[filename].GetTextureBlock(blockname);
            }
            else
            {
                Texture2D tex =null;
                try
                {
                     tex = LoadTexFromFile(filename);
                }
                catch(Exception err)
                {
                }
                if (tex == null)
                {
                    return null;
                }
                mapNamedTex[filename] = new TexturePacket(tex, null);
                return mapNamedTex[filename].GetTextureBlock(blockname);
            }

        }
        public TexturePacket.TextureBlock AddTextureQuick(Texture2D tex)
        {
            string texname="noname__";
            int i=0;
            string pname =texname+i.ToString();
            while (mapNamedTex.ContainsKey(pname) == true)
            {
                i++;
            }
            Dictionary<string, RectangleF> ps = new Dictionary<string, RectangleF>();
            mapNamedTex[pname] = new TexturePacket(tex, null);
            return mapNamedTex[pname].GetTextureBlock(null);

        }
        private Texture2D LoadTexFromFile(string filename)
        {
//#if XNA

            string fname=System.IO.Path.Combine(rootpath,filename);
            try
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(fname ))
                {
                    Texture2D tex = Texture2D.FromStream(device, s);
                    return tex;
                }
            }
            catch
            {
            }
            try
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(fname + ".png"))
                {
                    Texture2D tex = Texture2D.FromStream(device, s);
                    return tex;
                }
            }
            catch
            {
            }
            try
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(fname + ".jpg"))
                {
                    Texture2D tex = Texture2D.FromStream(device, s);
                    return tex;
                }
            }
            catch
            {
            }
            try
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(fname + ".jpeg"))
                {
                    Texture2D tex = Texture2D.FromStream(device, s);
                    return tex;
                }
            }
            catch
            {
            }
            return null;
//#else
//            return content.Load<Texture2D>(filename);
//#endif
        }
        public void Init(GraphicsDevice _device, ContentManager _content)
        {
            device = _device;
            content = _content;
            rootpath = content.RootDirectory;
#if WIN32
            string[] files = System.IO.Directory.GetFiles(rootpath, "*.packet.csv.txt", System.IO.SearchOption.AllDirectories);
            System.IO.File.WriteAllLines(System.IO.Path.Combine(rootpath, "packets.txt"), files);
#else
            string[] files = null;
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(System.IO.Path.Combine( rootpath , "packets.txt")))
                {
                    byte[] buf =new byte[65536];
                    int buflen = s.Read(buf, 0, 65536);

                    //byte[] b = new byte[s.Length];
                    //s.Read(b, 0, b.Length);
                    string txt = System.Text.Encoding.UTF8.GetString(buf, 0, buflen);
                    files = txt.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    while(!char.IsLetter(files[0][0]))
                    {
                        files[0]=files[0].Substring(1);
                    }
                }
            }
#endif
            foreach (var _f in files)
            {
				string f=_f;
				if(System.IO.Path.DirectorySeparatorChar=='/')
				{
					f=f.Replace('\\','/');
				}
                string texname = System.IO.Path.GetDirectoryName(f);
                texname = texname.Substring(rootpath.Length + 1);
                string name = System.IO.Path.GetFileNameWithoutExtension(f);
                name = System.IO.Path.GetFileNameWithoutExtension(name);
                LoadPacketInfo(f, System.IO.Path.Combine(texname, name));
            }
            //LoadResourceDefine(_rootpath\*.texpacker.xml);
        }
        void LoadPacketInfo(string filename,string texname)
        {
            Dictionary<string, RectangleF> _blocks = new Dictionary<string, RectangleF>();
            string[] lines = null;
            try
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(filename))
                {
                    byte[] buf = new byte[65536];
                    int buflen = (int)s.Read(buf, 0, 65536);
                    //byte[] b = new byte[s.Length];
                    //s.Read(b, 0, b.Length);
                    string t = System.Text.Encoding.UTF8.GetString(buf, 0, buflen);
                    lines = t.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                }
            }
            catch
            {
                return;
            }
            //string[] lines2 = System.IO.File.ReadAllLines(filename,Encoding.UTF8);
            foreach (var l in lines)
            {
                var ws = l.Split(new char[] { ',' });
                float x = float.Parse(ws[1])/8192.0f;
                float y = float.Parse(ws[2])/8192.0f;
                float w = float.Parse(ws[3])/8192.0f;
                float h = float.Parse(ws[4])/8192.0f;
                string _file = ws[0];
                while (!char.IsLetter(_file[0]))
                {
                    _file = _file.Substring(1);
                }
                _blocks.Add(_file, new RectangleF(x, y, w, h));
                mapFileInTex[_file] = texname;
            }
            mapNamedTex[texname] = new TexturePacket(LoadTexFromFile(texname), _blocks);
        }
        public void ClearPacketInfo()
        {
            mapFileInTex.Clear();
            mapNamedTex.Clear();
        }
    }
}
