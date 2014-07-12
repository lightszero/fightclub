using System;
using System.Collections.Generic;

using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace ha_ani_tool.hadata
{
    public class Node
    {
        [XmlAttribute]
        public string name;

        public class Frame : IEditAble
        {
            public Frame()
            {
                scaleX = 1.0f;
                scaleY = 1.0f;
                alpha = 255;
                rotation = 0;
                translationX = 0;
                translationY = 0;
            }
            [XmlAttribute]
            public float time;
            [XmlAttribute]
            public float rotation
            {
                get;
                set;
            }
            [XmlAttribute]
            public float translationX
            {
                get;
                set;
            }
            [XmlAttribute]
            public float translationY
            {
                get;
                set;
            }

            [XmlAttribute]
            public float scaleX
            {
                get;
                set;
            }
            [XmlAttribute]
            public float scaleY
            {
                get;
                set;
            }

            [XmlAttribute]
            public int alpha
            {
                get;
                set;
            }
            [XmlAttribute]
            public byte tintRed
            {
                get;
                set;
            }
            [XmlAttribute]
            public byte tintGreen
            {
                get;
                set;
            }
            [XmlAttribute]
            public byte tintBlue
            {
                get;
                set;
            }
            public Frame Copy()
            {
                Frame f = new Frame();
                f.rotation = this.rotation;
                f.scaleX = this.scaleX;
                f.scaleY = this.scaleY;
                f.time = this.time;
                f.translationX = this.translationX;
                f.translationY = this.translationY;
                f.alpha = this.alpha;
                f.tintRed = this.tintRed;
                f.tintGreen = this.tintGreen;
                f.tintBlue = this.tintBlue;
                return f;
            }
            public void Fill(Frame frame)
            {
                this.rotation = frame.rotation;
                this.scaleX = frame.scaleX;
                this.scaleY = frame.scaleY;
                this.time = frame.time;
                this.translationX = frame.translationX;
                this.translationY = frame.translationY;
                this.alpha = frame.alpha;
                this.tintRed = frame.tintRed;
                this.tintGreen = frame.tintGreen;
                this.tintBlue = frame.tintBlue;
            }
            public void linear(Frame a, Frame b, float v)
            {
                v = 1.0f - v;
                this.rotation = a.rotation * v + b.rotation * (1.0f - v);
                this.scaleX = a.scaleX * v + b.scaleX * (1.0f - v);
                this.scaleY = a.scaleY * v + b.scaleY * (1.0f - v);
                this.time = a.time * v + b.time * (1.0f - v);
                this.translationX = a.translationX * v + b.translationX * (1.0f - v);
                this.translationY = a.translationY * v + b.translationY * (1.0f - v);
                Color ca = new Color(a.tintRed, a.tintGreen, a.tintBlue, a.alpha);
                Color cb = new Color(b.tintRed, b.tintGreen, b.tintBlue, b.alpha);
                var oc=ca*v+cb*(1.0f-v);
               // Vector4 vc = ca.ToVector4() * v + cb.ToVector4() * (1.0f - v);
                //Color oc=  new Color(vc);
                this.alpha =(byte)( oc.a * 255);
                this.tintRed =(byte)(  oc.r*255);
                this.tintGreen =(byte)(  oc.g*255);
                this.tintBlue = (byte)( oc.b*255);


            }
            public bool IsEquals(Frame f)
            {
                    if (f.rotation != this.rotation) return false;
                    if (f.scaleX != this.scaleX) return false;
                    if (f.rotation != this.rotation) return false;
                    if (f.scaleX != this.scaleX) return false;
                    if (f.scaleY != this.scaleY) return false;
                    //if (f.time = this.time;
                    if (f.translationX != this.translationX) return false;
                    if (f.translationY != this.translationY) return false;
                    if (f.alpha != this.alpha) return false;
                    if (f.tintRed != this.tintRed) return false;
                    if (f.tintGreen != this.tintGreen) return false;
                    if (f.tintBlue != this.tintBlue) return false;
                    return true;
            }
            public void Move(int x, int y)
            {
                this.translationY -= y;
                this.translationX += x;
                //Save();
                //Value v = new Value(this);
                //Save();
            }
            public void Scale(float x, float y)
            {
                this.scaleX += x;
                this.scaleY += y;
            }
            public void Rotate(float value)
            {
                //Save();
                this.rotation += value;
                //Save();
            }
            class Value
            {
                public Value(Frame f)
                {
                    x = f.translationX;
                    y = f.translationY;
                    rotate = f.rotation;
                    sx = f.scaleX;
                    sy = f.scaleY;
                }
                public float x;
                public float y;
                public float rotate;
                public float sx;
                public float sy;
                public override bool Equals(object obj)
                {
                    Value v = obj as Value;
                    if (v == null) return false;
                    if (v.x == x && v.y == y && v.rotate == rotate && v.sx == sx && v.sy == sy) return true;

                    return false;
                }
				public override int GetHashCode ()
				{
					return base.GetHashCode ();
				}
            }
            [XmlIgnore]
            List<Value> History = new List<Value>();
            [XmlIgnore]
            int stepid = 0;
            public void Save()
            {
                while (History.Count > (stepid + 1) && stepid > 0)
                {
                    History.RemoveAt(stepid + 1);
                }

                Value v = new Value(this);
                if (History.Count == 0)
                {
                    History.Add(v);

                }
                else
                {
                    if (v != History[History.Count - 1])
                    {
                        History.Add(v);
                    }
                }
                while (History.Count > 10)
                {
                    History.RemoveAt(0);
                }
                stepid = History.Count - 1;
            }
            public void UnDo()
            {
                stepid--;
                if (stepid < 0) stepid = 0;
                if (History.Count <= stepid)
                {
                    return;
                }
                this.translationX = History[stepid].x;
                this.translationY = History[stepid].y;
                this.rotation = History[stepid].rotate;
                this.scaleX = History[stepid].sx;
                this.scaleY = History[stepid].sy;
            }

            public void ReDo()
            {
                stepid++;
                if (stepid >= History.Count) stepid = History.Count - 1;
                if (History.Count <= stepid)
                {
                    return;
                }
                this.translationX = History[stepid].x;
                this.translationY = History[stepid].y;
                this.rotation = History[stepid].rotate;
                this.scaleX = History[stepid].sx;
                this.scaleY = History[stepid].sy;
            }
        }
        [XmlElement("frame")]
        public List<Frame> frames = new List<Frame>();

    }


    [XmlRoot("anim")]
    //name='Atk01'>
    public class puppetanimdata
    {
        [XmlAttribute]
        public string name;



        [XmlElement("node")]
        public List<Node> nodes = new List<Node>();

        [XmlIgnore]
        public string _FileName;
        [XmlIgnore]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value.Replace(".puppetanim", ".PuppetAnim");
            }
        }
        public static puppetanimdata Load(string filename)
        {
            XmlSerializer s = new XmlSerializer(typeof(puppetanimdata));
            try
            {
                if(System.IO.File.Exists(filename)==false)
                {
                    
                    UnityEngine.Debug.Log(filename + "不存在");
                    return null;
                }
                using (System.IO.Stream stream = System.IO.File.OpenRead(filename))
                {
                    var r = s.Deserialize(stream) as puppetanimdata;
                    if (r != null)
                    {
                        r.FileName = filename;
                        return r;
                    }
                }
            }
            catch(Exception err)
            {
                UnityEngine.Debug.Log(filename + ":" + err.ToString());
            }
            return null;
        }
        public static puppetanimdata LoadFromString(string xml)
        {
            XmlSerializer s = new XmlSerializer(typeof(puppetanimdata));
            try
            {

                using (System.IO.StringReader stream =new System.IO.StringReader(xml))
                {
                    var r = s.Deserialize(stream) as puppetanimdata;
                    if (r != null)
                    {
                        r.FileName = "whatever";
                        return r;
                    }
                }
            }
            catch (Exception err)
            {
                UnityEngine.Debug.Log("whatever" + ":" + err.ToString());
            }
            return null;
        }
        public void Save()
        {
            XmlSerializer s = new XmlSerializer(typeof(puppetanimdata));
            try
            {

                using (System.IO.Stream stream = System.IO.File.Open(FileName, System.IO.FileMode.Create))
                {
                    s.Serialize(stream, this);
                }
            }
            catch
            {
            }

        }
    }
    public class AniRuntime
    {
        public int MaxFrame = 0;
        public Dictionary<int, Dictionary<string, Node.Frame>> frames = new Dictionary<int, Dictionary<string, Node.Frame>>();
        public static AniRuntime CreateFrom(puppetanimdata data)
        {
            AniRuntime ar = new AniRuntime();
            foreach (var node in data.nodes)
            {
                foreach (var frame in node.frames)
                {
                    int iframe = (int)Math.Round(frame.time * 24);
                    if (iframe > ar.MaxFrame) ar.MaxFrame = iframe;
                    if (ar.frames.ContainsKey(iframe) == false)
                    {
                        for (int i = 0; i <= iframe; i++)
                        {
                            if (ar.frames.ContainsKey(i) == false)
                            {
                                ar.frames[i] = new Dictionary<string, Node.Frame>();
                            }
                        }

                    }
                    ar.frames[iframe][node.name] = frame;
                }
            }
            foreach (var node in data.nodes)
            {
                for (int i = 0; i <= ar.MaxFrame; i++)
                {
                    if (!ar.frames[i].ContainsKey(node.name) && i > 0)
                    {
                        if (ar.frames[i - 1].ContainsKey(node.name))
                            ar.frames[i][node.name] = ar.frames[i - 1][node.name];
                    }
                }
            }
            return ar;
        }
        public void Restore(puppetanimdata data)
        {
            data.nodes.Clear();
            foreach (var f in this.frames)
            {
                float time = (float)f.Key / 24.0f;
                foreach (var b in f.Value)
                {
                    b.Value.time = time;
                    //bool bHaveBone =false;
                    Node bone = null;
                    foreach (var n in data.nodes)
                    {
                        if (n.name == b.Key)
                        {
                            bone = n;// ne = true;
                            break;
                        }
                    }

                    if (bone == null)
                    {
                        bone = new Node();
                        bone.name = b.Key;

                        data.nodes.Add(bone);
                    }
                    bone.frames.Add(b.Value);

                }
            }
        }
        public void ResetFrameCount(int count)
        {
            if (count < MaxFrame)
            {
                for (int i = count + 1; i <= MaxFrame; i++)
                {
                    if (frames.ContainsKey(i))
                    {
                        frames.Remove(i);
                    }
                }
            }
            if (count > MaxFrame)
            {
                for (int i = MaxFrame + 1; i <= count; i++)
                {
                    frames[i] = new Dictionary<string, Node.Frame>();
                    foreach (var p in frames[i - 1])
                    {
                        frames[i].Add(p.Key, p.Value.Copy());
                    }
                }
            }
            MaxFrame = count;
        }
    }
}
