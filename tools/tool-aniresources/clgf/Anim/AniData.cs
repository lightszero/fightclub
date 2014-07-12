using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RectangleF = clgf.type.RectangleF;
using PointF = Microsoft.Xna.Framework.Vector2;
#if USECompression
    using System.IO.Compression;
#endif 
namespace clgf.Anim
{
    public interface IStreamObj
    {
        void Write(System.IO.Stream stream);
        void Read(System.IO.Stream stream);
    }
    public class Seed:IStreamObj
    {

        public string texname;
        public Vector2 size;
        public Vector2 orient;
        public static RectangleF CalcRotate(Vector2 size, Vector2 orient, float rotate)
        {
            Vector2[] src=new Vector2[]{
                new Vector2(-orient.X,-orient.Y),
                new Vector2(-orient.X,-orient.Y+size.Y),
                new Vector2(-orient.X+size.X ,-orient.Y),
                 new Vector2(-orient.X+size.X ,-orient.Y+size.Y),
            };
            Matrix mat = Matrix.CreateRotationZ(rotate);
            Vector2[] dest = new Vector2[4];
            Vector2.Transform(src, ref mat, dest);
            Vector2 v1 = dest[0];
            Vector2 v2 = dest[0];
            for (int i = 1; i < 4; i++)
            {
                v1.X =Math.Min(v1.X,dest[i].X);
                v1.Y = Math.Min(v1.Y, dest[i].Y);
                v2.X = Math.Max(v2.X, dest[i].X);
                v2.Y = Math.Max(v2.Y, dest[i].Y);
            }
            return new RectangleF(v1.X, v1.Y, v2.X - v1.X, v2.Y - v1.Y);
        }
        public Seed Copy()
        {
            Seed seed = new Seed();
            seed.texname = texname;
            seed.size = size;
            seed.orient = orient;
            return seed;
        }
        public void Write(System.IO.Stream s)
        {
            byte[] bb;
            //texname
            bb = System.Text.Encoding.UTF8.GetBytes(texname);
            s.WriteByte((byte)bb.Length);
            s.Write(bb, 0, bb.Length);

            bb = BitConverter.GetBytes(size.X);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(size.Y);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(orient.X);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(orient.Y);
            s.Write(bb, 0, bb.Length);
        }

        public void Read(System.IO.Stream s)
        {
            byte[] bb = new byte[255];
            int len = s.ReadByte();
            s.Read(bb, 0, len);
            texname = System.Text.Encoding.UTF8.GetString(bb, 0, len);
            s.Read(bb, 0, 4);
            size.X = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            size.Y = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            orient.X = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            orient.Y = BitConverter.ToSingle(bb, 0);
        }
        public override bool Equals(object obj)
        {
            Seed o = obj as Seed;
            if (o == null) return false;
           
            if ( System.IO.Path.GetFileNameWithoutExtension(o.texname) !=  System.IO.Path.GetFileNameWithoutExtension(this.texname)) return false;
            if (!o.size.Equals(this.size)) return false;
            if (!o.orient.Equals(this.orient)) return false;
            return true;
        }

    }
    public class Element:IStreamObj
    {
        public string seed="";
        public string seednow="";
        //public Seed seed =null;//new Seed();
        public Color color=new Color(1.0f,1.0f,1.0f,1.0f);
        public Color coloradd = new Color(0,0,0,0);
        public Vector2 pos=new Vector2();
        public Vector2 scale=new Vector2();
        public float rotate=0;
        public string sound="";
        public string tag="";
        public string seedasdummy = "";
        public Element Copy()
        {
            Element e = new Element();
            e.seed = seed;
            e.seednow = seednow;
            e.color = color;
            e.coloradd = coloradd;
            e.pos = pos;
            e.scale = scale;
            e.rotate = rotate;
            e.sound = sound;
            e.tag = tag;
            e.seedasdummy = seedasdummy;
            return e;
        }
        public void Write(System.IO.Stream s)
        {
            byte[] bb;
            //seed.Write(s);
            //string name = System.IO.Path.GetFileNameWithoutExtension(seed.texname);
            //int index = Anim.poolindex.IndexOf(name);
            bb = System.Text.Encoding.UTF8.GetBytes(seed);
            s.WriteByte((byte)bb.Length);
            s.Write(bb, 0, bb.Length);

            s.WriteByte(color.R);
            s.WriteByte(color.G);
            s.WriteByte(color.B);
            s.WriteByte(color.A);
            s.WriteByte(coloradd.R);
            s.WriteByte(coloradd.G);
            s.WriteByte(coloradd.B);
            s.WriteByte(coloradd.A);
            bb = BitConverter.GetBytes(pos.X);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(pos.Y);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(scale.X);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(scale.Y);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(rotate);
            s.Write(bb, 0, bb.Length);
            //sound
            bb = System.Text.Encoding.UTF8.GetBytes(sound);
            s.WriteByte((byte)bb.Length);
            s.Write(bb, 0, bb.Length);
            //tag
            bb = System.Text.Encoding.UTF8.GetBytes(tag);
            s.WriteByte((byte)bb.Length);
            s.Write(bb, 0, bb.Length);

                  //seedasdummy
            bb = System.Text.Encoding.UTF8.GetBytes(seedasdummy);
            s.WriteByte((byte)bb.Length);
            s.Write(bb, 0, bb.Length);
        }

        public void Read(System.IO.Stream s)
        {
            byte[] bb = new byte[255];
            //seed = new Seed();
            //seed.Read(s);
            int slen =s.ReadByte();
            s.Read(bb, 0, slen);
            seed = System.Text.Encoding.UTF8.GetString(bb, 0, slen);
            seednow = seed;
            //int index = s.ReadByte();
            //seed = Anim.saveseedpool[Anim.poolindex[index]];
            

            color.R = (byte)s.ReadByte();
            color.G = (byte)s.ReadByte();
            color.B = (byte)s.ReadByte();
            color.A = (byte)s.ReadByte();
            coloradd.R = (byte)s.ReadByte();
            coloradd.G = (byte)s.ReadByte();
            coloradd.B = (byte)s.ReadByte();
            coloradd.A = (byte)s.ReadByte();
            s.Read(bb, 0, 4);
            pos.X = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            pos.Y = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            scale.X = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            scale.Y = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            rotate = BitConverter.ToSingle(bb, 0);

            int len = s.ReadByte();
            s.Read(bb, 0, len);
            sound=System.Text.Encoding.UTF8.GetString(bb, 0, len);

            len = s.ReadByte();
            s.Read(bb, 0, len);
            tag = System.Text.Encoding.UTF8.GetString(bb, 0, len);

            len = s.ReadByte();
            s.Read(bb, 0, len);
            seedasdummy = System.Text.Encoding.UTF8.GetString(bb, 0, len);
        }
    }
    public class Frame:IStreamObj
    {
        //public Vector2 footpos;
        //public Vector2 scale;
        public List<Element> elems = new List<Element>();
        public List<string> frametags = new List<string>();
        public void AddTag(string tag)
        {
            if (tag.Contains("state:"))
            {
                frametags.Add(tag.Substring(6));
            }
            else if(tag.Contains("f:"))
            {
                frametags.Add(tag.Substring(2));
            }
        }
        public IList<Element> GetElementWithSeedName(string name)
        {
            List<Element> list = new List<Element>();
            foreach (var i in elems)
            {
                if (i.seed.Contains(name))
                    list.Add(i.Copy());
            }
            return list;
        }
        public Frame Copy()
        {
            Frame f = new Frame();
            foreach (var e in elems)
            {
                f.elems.Add(e.Copy());
            }
            foreach (var t in frametags)
            {
                f.frametags.Add(t);
            }
            return f;
        }
        public void Write(System.IO.Stream s)
        {
            byte[] bb;
            bb = BitConverter.GetBytes((Int16)elems.Count);
            s.Write(bb, 0, bb.Length);
            for (int i = 0; i < elems.Count; i++)
            {
                elems[i].Write(s);
            }

            bb = BitConverter.GetBytes((Int16)frametags.Count);
            s.Write(bb, 0, bb.Length);
            for (int i = 0; i < frametags.Count; i++)
            {
                bb = System.Text.Encoding.UTF8.GetBytes(frametags[i]);
                s.WriteByte((byte)bb.Length);
                s.Write(bb, 0, bb.Length);
            }
        }

        public void Read(System.IO.Stream s)
        {
            elems.Clear();
            byte[] bb = new byte[255];
            s.Read(bb,0,2);
            int length = BitConverter.ToInt16(bb, 0);
            for (int i = 0; i < length; i++)
            {
                Element e = new Element();
                e.Read(s);
                elems.Add(e);
            }

            s.Read(bb, 0, 2);
            length = BitConverter.ToInt16(bb, 0);
            for (int i = 0; i < length; i++)
            {
                var len = s.ReadByte();
                s.Read(bb, 0, len);
                var tag = System.Text.Encoding.UTF8.GetString(bb, 0, len);
                frametags.Add(tag);
            }

        
        }
    }
    public class Anim:IStreamObj
    {
        public Sprite parent
        {
            get;
            private set;
        }
        public Anim(Sprite _parent)
        {
            parent=_parent;
        }
        public int fps = 24;
        public Point size;
        public List<Frame> frames=new List<Frame>();

        public Anim Copy(Sprite _parent)
        {
            Anim anim = new Anim(_parent);
            anim.fps = fps;
            anim.size = size;
            foreach (var f in frames)
            {
                anim.frames.Add(f.Copy());
            }
            return anim;
        }
        //public static Dictionary<string, Seed> saveseedpool = new Dictionary<string, Seed>();
        //public static List<string> poolindex = new List<string>();
        public void Write(System.IO.Stream os)
        {
            //saveseedpool.Clear();
            //poolindex.Clear();
            //foreach (var f in frames)
            //{
            //    foreach (var e in f.elems)
            //    {
            //        string name=System.IO.Path.GetFileNameWithoutExtension(e.seed.texname);
            //        if (saveseedpool.ContainsKey(name) == false)
            //        {
            //            saveseedpool[name] = e.seed;
            //            poolindex.Add(name);
            //        }
            //    }
            //}
#if USECompression
            using (GZipStream s = new GZipStream(os, CompressionMode.Compress))
#else
            var s=os;
#endif
            {
                //s.WriteByte((byte)saveseedpool.Count);
                //foreach (var k in poolindex)
                //{
                //    saveseedpool[k].Write(s);
                //}
                s.WriteByte((byte)fps);
                byte[] bb;
                bb = BitConverter.GetBytes((Int16)size.X);
                s.Write(bb, 0, bb.Length);
                bb = BitConverter.GetBytes((Int16)size.Y);
                s.Write(bb, 0, bb.Length);
                bb = BitConverter.GetBytes((Int16)frames.Count);
                s.Write(bb, 0, bb.Length);
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].Write(s);
                }
            }
            //saveseedpool.Clear();
            //poolindex.Clear();
        }

        public void Read(System.IO.Stream os)
        {
            //saveseedpool.Clear();
            //poolindex.Clear();
#if USECompression
            using(GZipStream s=new GZipStream(os,CompressionMode.Decompress))
#else
            var s = os;
#endif
            {
                //int pcount = s.ReadByte();
                //for (int i = 0; i < pcount; i++)
                //{
                //    Seed _s = new Seed();
                //    _s.Read(s);
                //    string name=System.IO.Path.GetFileNameWithoutExtension(_s.texname);
                //    poolindex.Add(name);
                //    saveseedpool[name]=_s;
                //}
                this.fps=s.ReadByte();
                byte[] bb = new byte[255];
                s.Read(bb, 0, 2);
                this.size.X = BitConverter.ToInt16(bb, 0);
                s.Read(bb, 0, 2);
                this.size.Y = BitConverter.ToInt16(bb, 0);

                frames.Clear();
                 s.Read(bb, 0, 2);
                int fcount = BitConverter.ToInt16(bb, 0);
                for (int i = 0; i < fcount; i++)
                {
                    Frame f = new Frame();
                    f.Read(s);
                    frames.Add(f);
                }
            }
            //saveseedpool.Clear();
            //poolindex.Clear();
        }
    }


    public sealed class Sprite
    {
        private Sprite(string name)
        {
            this.name = name;
        }
        public string name
        {
            get;
            private set;
        }
        public Dictionary<string, Anim> anims = new Dictionary<string, Anim>();
        public Dictionary<string, Seed> seeds = new Dictionary<string, Seed>();

        public Sprite Copy()
        {
            Sprite sp = new Sprite(this.name);
            foreach (var i in seeds)
            {
                sp.seeds[i.Key] = i.Value.Copy();
            }
            foreach (var anim in anims)
            {
                sp.anims[anim.Key] = anim.Value.Copy(sp);
            }
            return sp;
        }
        public void Save(string outpath)
        {
#if WIN32
            {
                string file = System.IO.Path.Combine(outpath, "seeds.bin.bytes");
                if (System.IO.Directory.Exists(outpath) == false)
                {
                    System.IO.Directory.CreateDirectory(outpath);
                }
                using (System.IO.Stream os = System.IO.File.Create(file))
                {
#if USECompression
                    using (GZipStream s = new GZipStream(os, CompressionMode.Compress))
#else 
                    var s = os;
#endif
                    {
                        UInt16 len=(UInt16) seeds.Count;
                        s.Write(BitConverter.GetBytes(len), 0, 2);
                        foreach (var i in seeds)
                        {
                            byte[] sb = System.Text.Encoding.UTF8.GetBytes(i.Key);
                            s.WriteByte((byte)sb.Length);
                            s.Write(sb, 0, sb.Length);
                            i.Value.Write(s);
                        }
                    }
                }
            }
            string flist = "";
            foreach (var i in anims)
            {
                string file = System.IO.Path.Combine(outpath, i.Key + ".anim.bin");
                using (System.IO.Stream s = System.IO.File.Create(file+".bytes"))
                {
                    i.Value.Write(s);
                }
                flist += file + "\r\n";
            }
            System.IO.File.WriteAllText(System.IO.Path.Combine(outpath, "anims.txt"), flist);
#endif
        }
        public static Sprite Load(string inpath)
        {
            Sprite spr = new Sprite(inpath);

            if (inpath == null) return spr;
            spr.seeds.Clear();//LoadSeeds
            {
                if (System.IO.Path.DirectorySeparatorChar == '\\')
                    inpath = inpath.Replace("/", "\\");
                else
                    inpath = inpath.Replace("\\", "/");
                string file = System.IO.Path.Combine(inpath, "seeds.bin.bytes");
                using (System.IO.Stream os = TitleContainer.OpenStream(file))
                {
#if USECompression
                    using (GZipStream s = new GZipStream(os, CompressionMode.Decompress))
#else
                    var s = os;
#endif
                    {
                        byte[] bb = new byte[256];

                        s.Read(bb, 0, 2);
                        UInt16 len = BitConverter.ToUInt16(bb, 0);
                        for (int i = 0; i < len; i++)
                        {
                            int slen = s.ReadByte();
                            s.Read(bb, 0, slen);
                            Seed seed = new Seed();
                            seed.Read(s);
                            string name = System.Text.Encoding.UTF8.GetString(bb, 0, slen);
                            spr.seeds[name] = seed;
                        }
                    }
                }
            }
            string[] files = null;
            {
                using (System.IO.Stream s = TitleContainer.OpenStream(System.IO.Path.Combine( inpath , "anims.txt")))
                {
                    byte[] buf = new byte[65536];
                    int buflen = (int)s.Read(buf, 0, 65536);
                    //byte[] b = new byte[s.Length];
                    //s.Read(b, 0, b.Length);
                    string txt = System.Text.Encoding.UTF8.GetString(buf,0,buflen);
                    files = txt.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    while(!char.IsLetter(files[0][0]))
                    {
                        files[0]=files[0].Substring(1);
                    }
                }
            }
            //string[] files = System.IO.Directory.GetFiles(inpath, "*.anim.bin");
            foreach (var _f in files)
            {
				string f =_f;
				if(System.IO.Path.DirectorySeparatorChar=='/')
				{
					f=f.Replace('\\','/');
				}
				string fname =System.IO.Path.GetFileName(f);
				fname = System.IO.Path.Combine(inpath,fname);
                using (System.IO.Stream ss = TitleContainer.OpenStream(fname))
                {
                    Anim ani = new Anim(spr);
                    ani.Read(ss);
                    string name = System.IO.Path.GetFileNameWithoutExtension(f);
                    name = System.IO.Path.GetFileNameWithoutExtension(name);
                    spr.anims[name] = ani;
                }
            }
            return spr;
        }


        public void SeedHide(string name)
        {
            this.name += "|H|" + name;
            foreach (var ani in anims.Values)
            {
                foreach (var f in ani.frames)
                {
                    foreach (var e in f.elems)
                    {
                        if (e.seed.Contains(name))
                        {
                            e.seednow="";
                        }
                    }
                }
            }
        }
        public void SeedRestore(string name)
        {
            this.name += "|R|" + name;
            foreach (var ani in anims.Values)
            {
                foreach (var f in ani.frames)
                {
                    foreach (var e in f.elems)
                    {
                        if (e.seed.Contains(name))
                        {
                            e.seednow = e.seed;
                        }
                    }
                }
            }
        }
        public void SeedReplace(string src, string dest)
        {
            this.name += "|R|" + src+"|"+dest;
            foreach (var ani in anims.Values)
            {
                foreach (var f in ani.frames)
                {
                    foreach (var e in f.elems)
                    {
                        if (e.seed.Contains(src))
                        {
                            e.seednow = dest;
                        }
                    }
                }
            }
        }
    }
}
