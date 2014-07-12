using System;
using System.Collections.Generic;

using System.Text;

using UnityEngine;
#if USECompression
    using System.IO.Compression;
#endif 
namespace KAnim
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
        public static Rect CalcRotate(Vector2 size, Vector2 orient, float rotate)
        {
            Vector2[] src=new Vector2[]{
                new Vector2(-orient.x,-orient.y),
                new Vector2(-orient.x,-orient.y+size.y),
                new Vector2(-orient.x+size.x ,-orient.y),
                 new Vector2(-orient.x+size.x ,-orient.y+size.y),
            };

            Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero,Quaternion.AngleAxis(rotate/(float)Math.PI*180.0f,Vector3.forward),Vector3.one);  
            //CreateRotationZ(rotate);
            //math change

            Vector2[] dest = new Vector2[4];
            for (int i = 0; i < src.Length; i++)
            {
                dest[i] = mat.MultiplyPoint3x4(src[i]);
            }
            //Vector2.Transform(src, ref mat, dest);
            //math change

            Vector2 v1 = dest[0];
            Vector2 v2 = dest[0];
            for (int i = 1; i < 4; i++)
            {
                v1.x =Math.Min(v1.x,dest[i].x);
                v1.y = Math.Min(v1.y, dest[i].y);
                v2.x = Math.Max(v2.x, dest[i].x);
                v2.y = Math.Max(v2.y, dest[i].y);
            }
            return new Rect(v1.x, v1.y, v2.x - v1.x, v2.y - v1.y);
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

            bb = BitConverter.GetBytes(size.x);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(size.y);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(orient.x);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(orient.y);
            s.Write(bb, 0, bb.Length);
        }

        public void Read(System.IO.Stream s)
        {
            byte[] bb = new byte[255];
            int len = s.ReadByte();
            s.Read(bb, 0, len);
            texname = System.Text.Encoding.UTF8.GetString(bb, 0, len);
            s.Read(bb, 0, 4);
            size.x = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            size.y = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            orient.x = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            orient.y = BitConverter.ToSingle(bb, 0);
        }
    }
    public class Element:IStreamObj
    {
        public string seed="";
        public string seednow="";
        //public Seed seed =null;//new Seed();
        public Color32 color = new Color32(255, 255, 255,255);
        public Color32 coloradd = new Color32(0, 0, 0, 0);
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

            s.WriteByte(color.r);
            s.WriteByte(color.g);
            s.WriteByte(color.b);
            s.WriteByte(color.a);
            s.WriteByte(coloradd.r);
            s.WriteByte(coloradd.g);
            s.WriteByte(coloradd.b);
            s.WriteByte(coloradd.a);
            bb = BitConverter.GetBytes(pos.x);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(pos.y);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(scale.x);
            s.Write(bb, 0, bb.Length);
            bb = BitConverter.GetBytes(scale.y);
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
            

            color.r = (byte)s.ReadByte();
            color.g = (byte)s.ReadByte();
            color.b = (byte)s.ReadByte();
            color.a = (byte)s.ReadByte();
            coloradd.r = (byte)s.ReadByte();
            coloradd.g = (byte)s.ReadByte();
            coloradd.b = (byte)s.ReadByte();
            coloradd.a = (byte)s.ReadByte();
            s.Read(bb, 0, 4);
            pos.x = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            pos.y = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            scale.x = BitConverter.ToSingle(bb, 0);
            s.Read(bb, 0, 4);
            scale.y = BitConverter.ToSingle(bb, 0);
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
                string file = System.IO.Path.Combine(outpath, "seeds.bin");
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
                string file =System.IO.Path.Combine(outpath, i.Key + ".anim.bin");
                using (System.IO.Stream s = System.IO.File.Create(file))
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
                string file = System.IO.Path.Combine(inpath, "seeds.bin");

            }
            string[] files = null;
            {
                //using (System.IO.Stream s = TitleContainer.OpenStream(System.IO.Path.Combine( inpath , "anims.txt")))
                //{
                //    byte[] b = new byte[s.Length];
                //    s.Read(b, 0, b.Length);
                //    string txt = System.Text.Encoding.UTF8.GetString(b,0,b.Length);
                //    files = txt.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                //    while(!char.IsLetter(files[0][0]))
                //    {
                //        files[0]=files[0].Substring(1);
                //    }
                //}
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
                //using (System.IO.Stream ss = TitleContainer.OpenStream(fname))
                //{
                //    Anim ani = new Anim(spr);
                //    ani.Read(ss);
                //    string name = System.IO.Path.GetFileNameWithoutExtension(f);
                //    name = System.IO.Path.GetFileNameWithoutExtension(name);
                //    spr.anims[name] = ani;
                //}
            }
            return spr;
        }

        public static Sprite LoadUnity(string inpath, byte[] seeddata)
        {
            Sprite spr = new Sprite(inpath);

            if (inpath == null) return spr;
            using (System.IO.Stream os = new System.IO.MemoryStream(seeddata))
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
            //foreach (var ani in anis)
            {
                //using (System.IO.Stream ss = TitleContainer.OpenStream(fname))
                //{
                //    Anim ani = new Anim(spr);
                //    ani.Read(ss);
                //    string name = System.IO.Path.GetFileNameWithoutExtension(f);
                //    name = System.IO.Path.GetFileNameWithoutExtension(name);
                //    spr.anims[name] = ani;
                //}
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
