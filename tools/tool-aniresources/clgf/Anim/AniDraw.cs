using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RectangleF = clgf.type.RectangleF;
using PointF = Microsoft.Xna.Framework.Vector2;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using clgf.Texture;

namespace clgf.Anim
{
    public class DrawElement
    {
        public clgf.Texture.TexturePacket.TextureBlock block;
        public Vector2 seedsize;
        public Vector2 seedorient;
        public Color color = new Color();
        public Vector2 pos = new Vector2();
        public Vector2 scale = new Vector2();
        public float rotate = 0;
        public RectangleF bounds;
        public string tag;
        public void Fill(clgf.Anim.Anim anim, clgf.Anim.Element elem, TextureMgr tmgr, string texpath)
        {
            //if (string.IsNullOrEmpty(elem.seednow))
            //{
            //    return;
            //}
            if (anim.parent.seeds.ContainsKey(elem.seednow) == false) return;

            var seed = anim.parent.seeds[elem.seednow];
            //string filename = System.IO.Path.GetFileNameWithoutExtension(elem.seed.texname);
            block = tmgr.GetTexture(System.IO.Path.Combine(texpath, seed.texname));
            seedorient = seed.orient;
            seedsize = seed.size;
            color = elem.color;
            pos = elem.pos;
            scale = elem.scale;

            rotate = elem.rotate;
            bounds = Seed.CalcRotate(seed.size * scale, seed.orient * scale, rotate);
            bounds.Offset(pos.X, pos.Y);
            tag = elem.tag;
            if (tag == null) tag = "";
            if (tag.IndexOf("dummy:") == 0)
            {
                isdummy = true;
                tag = tag.Substring(6);
            }
        }

        public bool isdummy = false;
    }
    public class Dummy
    {
        public string name;
        public Vector2 pos = new Vector2();
        public float rotate = 0;
        public string seed;
        public DrawElement elem;
    }
    public class DrawFrame
    {
        public Point size
        {
            private set;
            get;
        }
        public DrawFrame(Point _size)
        {
            size = _size;
        }
        public Dictionary<string, RectangleF> bounds = new Dictionary<string, RectangleF>();
        public List<DrawElement> drawElements = new List<DrawElement>();
        public List<Dummy> dummys = new List<Dummy>();
        public List<string> sounds = new List<string>();
        public RectangleF? GetBounds(Vector2 pos, Vector2 scale, string name = "")
        {
            if (bounds.ContainsKey(name) == false) return null;

            RectangleF rect = bounds[name];
            rect.X *= scale.X;
            rect.Y *= scale.Y;
            rect.Width *= scale.X;
            rect.Height *= scale.Y;

            float x = pos.X + (-size.X / 2) * scale.X;
            float y = pos.Y + (-size.Y) * scale.Y;
            rect.Offset(x, y);
            if (rect.Width < 0)
            {
                rect.X += rect.Width;
                rect.Width *= -1;
            }
            if (rect.Height < 0)
            {
                rect.Y += rect.Height;
                rect.Height *= -1;
            }
            return rect;
        }
        public void DrawWithTag(List<string> tag, SpriteBatch sb, Vector2 pos, Vector2 scale, Color color)
        {
            //scale.X *= -1;
            foreach (var d in drawElements)
            {
                if (tag.Contains(d.tag) == false) continue;
                DrawElement(size, d, sb, pos, scale, color);
            }
        }
        public static void DrawElement(Point size, DrawElement d, SpriteBatch sb, Vector2 pos, Vector2 scale, Color color)
        {
            if (d.block == null) return;

            float width = (d.seedsize.X * d.scale.X * scale.X);
            float height = d.seedsize.Y * d.scale.Y * scale.Y;
            float x = pos.X + (d.pos.X - size.X / 2) * scale.X;
            float y = pos.Y + (d.pos.Y - size.Y) * scale.Y;
            if (scale.X != 1) width += 1;
            if (scale.Y != 1) height += 1;

            Rectangle dest = new Rectangle((int)x, (int)y, (int)width, (int)height);


            //dest.X = (int)pos.X;
            //dest.Y = (int)pos.Y;
            Rectangle src = new Rectangle((int)(d.block.uv.X * d.block.parent.texture.Width),
                                (int)(d.block.uv.Y * d.block.parent.texture.Height),
                                (int)(d.block.uv.Width * d.block.parent.texture.Width),
                                (int)(d.block.uv.Height * d.block.parent.texture.Height));
            Vector2 orient = d.seedorient;
            orient.X *= src.Width / d.seedsize.X;
            orient.Y *= src.Height / d.seedsize.Y;
            //orient.X = (int)orient.X;
            //orient.Y = (int)orient.Y;
            //if (limitdest != null)
            //{
            //    Rectangle sdest = dest;
            //    sdest.Offset((int)-orient.X, (int)-orient.Y);
            //    if (limitdest.Value.Intersects(sdest) == false) continue;
            //}

#if XNA

#else
                //monogame orient 实现有bug...
                //orient.X *= (float)d.block.parent.texture.Width /src.Width;
                //orient.Y *=  (float)d.block.parent.texture.Height/src.Height;

#endif
            float rotate = d.rotate * Math.Sign(scale.X) * Math.Sign(scale.Y);
            float r = (float)(color.R / 255.0f * d.color.R / 255.0f);
            float g = (float)(color.G / 255.0f * d.color.G / 255.0f);
            float b = (float)(color.B / 255.0f * d.color.B / 255.0f);
            float a = (float)(color.A / 255.0f * d.color.A / 255.0f);
            sb.Draw(d.block.parent.texture, dest, src, new Color(r, g, b, a), rotate, orient, SpriteEffects.None, 0);
        }
        public void Draw(SpriteBatch sb, Vector2 pos, Vector2 scale, Color color)
        {
            //scale.X *= -1;
            foreach (var d in drawElements)
            {
                DrawElement(size, d, sb, pos, scale, color);
                //sb.Draw(d.block.parent.texture, new Rectangle((int)(pos.X - 2), (int)(pos.Y - 2), 4, 4), Color.Red);
            }
        }

        //private Rectangle? limitdest = null;
        //public void LimitRect(Rectangle? dest=null)
        //{
        //    limitdest = dest;
        //}


    }
    public class AniElement
    {

        public AniElement(SpriteAni ani, int fstart, int fend, bool bloop)
        {
            this.ani = ani;
            framestart = fstart;
            frameend = fend;
            loop = bloop;
            life = frameend - framestart + 1;
        }
        public void FixLife()
        {
            life = frameend - framestart + 1;
        }
        public override string ToString()
        {
            if (loop)
            {
                return "(<-" + framestart + "--" + frameend + "->)";
            }
            else
            {
                return "(-" + framestart + "->" + frameend + "-)";
            }
        }
        public SpriteAni ani;
        public int life;//生命周期
        public bool loop;//是否循环
        public int framestart;//开始帧
        public int frameend;   //结束帧

        public object Clone()
        {
            AniElement newobj = new AniElement(this.ani, this.framestart, this.frameend, this.loop);
            newobj.life = this.life;
            return newobj;

        }
    }
    public class SpriteAni //绘制用的动画实例
    {
        public float fps
        {
            get;
            private set;
        }
        public DrawFrame GetFrameByID(int fid)
        {
            return frames[fid];
        }
        public int framecount
        {
            get
            {
                return frames.Count;
            }
        }

        //public DrawFrame GetFrameByAdvTime(float delta)
        //{
        //    int lastid = (int)(timer * fps);
        //    if (play)
        //    {
        //        timer += delta;
        //    }
        //    int newid = (int)(timer * fps);
        //    while (newid - lastid > 1)
        //    {
        //        timer -= 1.0f / fps;//限制不跳帧
        //        newid --;
        //    }



        //    return frames[newid];
        //}
        public static SpriteAni CreateAni(Anim data, TextureMgr tmgr, string texpath = "")
        {
            SpriteAni ani = new SpriteAni();
            ani.fps = data.fps;
            while (ani.frames.Count < data.frames.Count)
            {
                ani.frames.Add(new DrawFrame(data.size));
            }
            int lastframe = 0;

            for (int i = 0; i < data.frames.Count; i++)
            {
                //while (ani.frames[i].drawElements.Count < data.frames[i].elems.Count)
                //{
                //    ani.frames[i].drawElements.Add(new DrawElement());
                //}
                #region fill frame
                for (int j = data.frames[i].elems.Count - 1; j >= 0; j--)
                {
                    if (string.IsNullOrEmpty(data.frames[i].elems[j].seednow))
                        continue;
                    DrawElement e = new DrawElement();
                    //在这里要反转层顺序


                    Element se = data.frames[i].elems[j];
                    if (string.IsNullOrEmpty(se.sound) == false)
                    {
                        ani.frames[i].sounds.Add(se.sound);
                        Console.WriteLine("find sound:" + se.sound);
                    }
                    e.Fill(data, se, tmgr, texpath);
                    if (e.isdummy == true || string.IsNullOrWhiteSpace(se.seedasdummy) == false)
                    {
                        Dummy dm = new Dummy();
                        dm.pos = e.pos;
                        dm.rotate = e.rotate;
                        dm.name = e.tag;
                        if (string.IsNullOrWhiteSpace(se.seedasdummy) == false)
                            dm.name = se.seedasdummy;
                        dm.seed = se.seed;
                        dm.elem = e;
                        ani.frames[i].dummys.Add(dm);
                    }
                    if (!e.isdummy)
                    {
                        ani.frames[i].drawElements.Add(e);

                        string tag = "";
                        if (string.IsNullOrEmpty(se.tag) == false)
                        {
                            tag = se.tag;
                        }
                        else
                        {
                            tag = "";
                        }
                        if (tag != "")
                        {
                            if (ani.frames[i].bounds.ContainsKey("") == false)
                            {
                                ani.frames[i].bounds[""] = e.bounds;
                            }
                            else
                            {
                                RectangleF src = ani.frames[i].bounds[""];
                                ani.frames[i].bounds[""] = RectangleF.Union(src, e.bounds);
                            }
                        }
                        if (ani.frames[i].bounds.ContainsKey(tag) == false)
                        {
                            ani.frames[i].bounds[tag] = e.bounds;
                        }
                        else
                        {
                            RectangleF src = ani.frames[i].bounds[tag];
                            ani.frames[i].bounds[tag] = RectangleF.Union(src, e.bounds);
                        }
                    }
                }
                #endregion
                if (i == 0 && data.frames[0].frametags.Count == 0)
                {
                    ani.elements["in"] = new AniElement(ani, 0, data.frames.Count - 1, false);
                }
                if (i == data.frames.Count - 1 && data.frames[i].frametags.Count == 0 && lastframe > 0)
                {
                    ani.elements["out"] = new AniElement(ani, lastframe + 1, data.frames.Count - 1, false);
                }
                if (data.frames[i].frametags.Count > 0)
                {
                    lastframe = i;
                    if (ani.elements.ContainsKey("in"))
                    {
                        if (ani.elements["in"].frameend == data.frames.Count - 1)
                        {
                            ani.elements["in"].frameend = i - 1;
                            ani.elements["in"].FixLife();
                        }
                    }
                }
                foreach (var tag in data.frames[i].frametags)
                {

                    if (ani.elements.ContainsKey(tag))
                    {
                        ani.elements[tag].frameend = i;
                        ani.elements[tag].FixLife();
                    }
                    else
                    {
                        ani.elements[tag] = new AniElement(ani, i, i, true);

                    }
                }

            }
            string[] ttags = new string[ani.framecount];
            foreach (var e in ani.elements)
            {
                for (int i = e.Value.framestart; i <= e.Value.frameend; i++)
                {
                    ttags[i] = e.Key;
                }
            }
            string begintag = "";
            int begini = 0;
            for (int i = 0; i < ani.framecount; i++)
            {
                if (string.IsNullOrEmpty(ttags[i]) == false)
                {
                    if (begintag != ttags[i])
                    {
                        if (begintag == "")
                        {

                        }
                        else
                        {
                            string endtag = ttags[i];
                            if (begini + 1 <= i - 1)
                                ani.elements[begintag + "-" + endtag] = new AniElement(ani, begini + 1, i - 1, false);
                        }
                    }
                    begintag = ttags[i];
                    begini = i;
                }

            }
            ani.allelement = new AniElement(ani, 0, ani.framecount - 1, true);
            return ani;
        }
        private List<DrawFrame> frames = new List<DrawFrame>();


        public Dictionary<string, AniElement> elements = new Dictionary<string, AniElement>();
        AniElement allelement;
        public AniElement GetElement(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return allelement;
            }
            else
            {
                return elements[name];
            }
        }
    }

    public class SpriteAniController //动画播放器
    {
        public SpriteAniController(AniElement state)
        {
            //this.data = data;
            this.element_state = state;
            this._data = state.ani;
            if (element_state != null)
            {
                curfstart = element_state.framestart;
                curfend = element_state.frameend;
                cureloop = element_state.loop;
                _data = element_state.ani;
                curframeid = 0;
                timer = 0;
            }
            this.bplay = true;
            instate = true;
        }
        private float timer = 0;

        public bool bplay
        {
            get;
            set;
        }
        public bool skipframe
        {
            get;
            set;
        }
        SpriteAni _data;
        public bool instate
        {
            get;
            private set;
        }
        public float GetCurElementLength()
        {
            return (float)(curfend - curfstart + 1) / 24.0f;
        }
        public void AdvTime(float delta)
        {
            int lastid = (int)(timer * _data.fps);
            if (bplay)
            {
                timer += delta;
            }
            curframeid = (int)(timer * _data.fps);
            if (curframeid == lastid && curframeid <= (curfend - curfstart)) return;
            if (skipframe)
            {

                while (curframeid - lastid > 1)
                {
                    timer -= 1.0f / _data.fps;//限制不跳帧
                    curframeid--;

                }
            }
            curelife -= (curframeid - lastid);
            if (curelife <= 0)
            {
                curelife = 0;
                if (element_queue.Count > 0)
                {
                    var e = element_queue.Dequeue();
                    if (monitor != null)
                    {
                        if (monitor.ContainsKey(e))
                        {
                            monitor[e]();
                            monitor.Remove(e);
                        }
                    }
                    curfstart = e.framestart;
                    curfend = e.frameend;
                    cureloop = e.loop;
                    curelife = e.life;
                    curframeid = 0;
                    timer = 0;
                    _data = e.ani;
                    instate = false;
                }
                else
                {
                    if (element_state == null)
                    {
                        cureloop = false;

                    }
                    else
                    {
                        if (monitor != null)
                        {
                            if (monitor.ContainsKey(element_state))
                            {
                                monitor[element_state]();
                                monitor.Remove(element_state);
                            }
                        }
                        curfstart = element_state.framestart;
                        curfend = element_state.frameend;
                        cureloop = element_state.loop;
                        _data = element_state.ani;

                    }
                    instate = true;
                }
            }
            if (cureloop)
            {
                while (curframeid >= (curfend - curfstart + 1))
                {
                    curframeid = curframeid - (curfend - curfstart + 1);
                    timer -= (curfend - curfstart + 1) / _data.fps;
                }
            }
            else
            {
                if (curframeid >= (curfend - curfstart + 1))
                {
                    curframeid = (curfend - curfstart + 1) - 1;
                }
            }
        }
        public DrawFrame GetFrame()
        {
            return _data.GetFrameByID(curframeid + curfstart);
        }

        int curfstart;
        int curfend;
        private int curframeid = 0;
        int curelife = 0;
        bool cureloop;
        public enum playtag
        {
            play_immediate,//立即
            play_afterthiselement,//插入在当前片段后面
            play_wait,//插入在所有片段后面            
        }
        public Queue<AniElement> element_queue = new Queue<AniElement>();
        public AniElement element_state = null;
        public Dictionary<AniElement, Action> monitor = null;
        public void Play(AniElement elem, playtag tag, Action whenplay = null)
        {
            if (tag == playtag.play_immediate)
            {
                element_queue.Clear();
                curfstart = elem.framestart;
                curfend = elem.frameend;
                curelife = elem.life;
                cureloop = elem.loop;
                curframeid = 0;
                _data = elem.ani;
                timer = 0;
                instate = false;
                if (whenplay != null)
                    whenplay();
            }
            if (tag == playtag.play_afterthiselement)
            {
                element_queue.Clear();
                element_queue.Enqueue(elem);
                if (whenplay != null)
                {
                    if (monitor == null) monitor = new Dictionary<AniElement, Action>();
                    monitor[elem] = whenplay;
                }
            }
            if (tag == playtag.play_wait)
            {
                element_queue.Enqueue(elem);
                if (whenplay != null)
                {
                    if (monitor == null) monitor = new Dictionary<AniElement, Action>();
                    monitor[elem] = whenplay;
                }
            }
        }
        public void State(AniElement elem, bool bimmediate = true, Action whenplay = null)
        {
            element_state = elem;
            if (whenplay != null)
            {
                if (monitor == null) monitor = new Dictionary<AniElement, Action>();
                monitor[elem] = whenplay;
            }
            if (bimmediate && elem != null && element_queue.Count == 0)
            {
                curfstart = element_state.framestart;
                curfend = element_state.frameend;
                cureloop = element_state.loop;
                _data = element_state.ani;
                curframeid = 0;
                timer = 0;
                instate = true;
            }
        }

    }


}
