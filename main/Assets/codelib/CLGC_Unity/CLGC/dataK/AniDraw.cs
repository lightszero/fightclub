using System;
using System.Collections.Generic;

using System.Text;

using UnityEngine;

namespace KAnim
{
    public class Dummy
    {
        public string name;
        public Vector2 pos = new Vector2();
        public float rotate = 0;
    }
    public class DrawFrame
    {
        public Point size
        {
            private set;
            get;
        }
        public int frameid
        {
            get;
            private set;
        }
        public string aniname
        {
            get;
            private set;
        }
        public DrawFrame(Point _size,int frameid,string aniname)
        {
            size = _size;
            this.frameid = frameid;
            this.aniname = aniname;
        }
        public Dictionary<string, Rect> bounds = new Dictionary<string, Rect>();
        //public List<DrawElement> drawElements = new List<DrawElement>();
		public List<Dummy> dummys = new List<Dummy>();
        public List<string> sounds = new List<string>();
        public Rect? GetBounds(Vector2 pos, Vector2 scale, string name = "")
        {
            if (bounds.ContainsKey(name) == false) return null;

            Rect rect = bounds[name];
            rect.x *= scale.x;
            rect.y *= scale.y;
            rect.width *= scale.x;
            rect.height *= scale.y;

            float x = pos.x + (-size.X / 2) * scale.x;
            float y = pos.y + (-size.Y) * scale.y;
            //rect.Offset(x, y);
            rect.x += x;
            rect.y += y;
            if (rect.width < 0)
            {
                rect.x += rect.width;
                rect.width *= -1;
            }
            if (rect.height < 0)
            {
                rect.y += rect.height;
                rect.height *= -1;
            }
            return rect;
        }

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
        public static Rect Rect_Union(Rect a, Rect b)
        {
            return Rect.MinMaxRect(a.x < b.x ? a.x : b.x, a.y < b.y ? a.y : b.y,
                a.xMax > b.xMax ? a.xMax : b.xMax, a.yMax > b.yMax ? a.yMax : b.yMax);
        }
        public static Rect Calc_Bound(KAnim.Anim anim,KAnim.Element elem)
        {
            
            //if(anim.parent.seeds.ContainsKey(elem.seednow)==false)
            //{
            //    Debug.LogError("failed Calc_Bound:" + elem.seednow);
            //}
            var seed = anim.parent.seeds[elem.seednow];
            //string filename = System.IO.Path.GetFileNameWithoutExtension(elem.seed.texname);
            //block = tmgr.GetTexture(System.IO.Path.Combine(texpath, seed.texname));
            var seedorient = seed.orient;
            //seedsize = seed.size;
            //color = elem.color;
            var pos = elem.pos;
            var scale = elem.scale;

            var rotate = elem.rotate;
            Vector2 seed_size = seed.size;
            seed_size.x *= scale.x;
            seed_size.y *= scale.y;
            Vector2 seed_orient = seed.orient;
            seed_orient.x *= scale.x;
            seed_orient.y *= scale.y;
            var bounds = Seed.CalcRotate(seed_size, seed_orient, rotate);
            //bounds.Offset(pos.X, pos.Y);
            bounds.x += pos.x;
            bounds.y += pos.y;
            return bounds;
        }
        public static SpriteAni CreateAni(Anim data,string aniname)
        {
            SpriteAni ani = new SpriteAni();
            ani.fps = data.fps;
            while (ani.frames.Count < data.frames.Count)
            {
                ani.frames.Add(new DrawFrame(data.size, ani.frames.Count,aniname));
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
                    //DrawElement e = new DrawElement();

                    //在这里要反转层顺序
                    Element se = data.frames[i].elems[j];
                    if(string.IsNullOrEmpty(se.sound)==false)
                    {
                        ani.frames[i].sounds.Add(System.IO.Path.GetFileNameWithoutExtension(se.sound));
                    }


                    //e.Fill(data, se);
                    bool e_isdummy = (se.tag.IndexOf("dummy:") == 0);
                    if (e_isdummy == true || string.IsNullOrEmpty(se.seedasdummy) == false)
                    {
                        Dummy dm = new Dummy();
                        dm.pos = se.pos;
                        dm.rotate = se.rotate;
                       
                        if (string.IsNullOrEmpty(se.seedasdummy) == false)
                            dm.name = se.seedasdummy;
                        else
                            dm.name = se.tag.Substring(6);
                        ani.frames[i].dummys.Add(dm);
                    }
                    if(!e_isdummy)
                    {
                        Rect e_bounds = Calc_Bound(data, se);
	                    //ani.frames[i].drawElements.Add(e);

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
	                            ani.frames[i].bounds[""] = e_bounds;
	                        }
	                        else
	                        {
	                            Rect src = ani.frames[i].bounds[""];
	                            ani.frames[i].bounds[""] = Rect_Union(src, e_bounds);
	                        }
	                    }
	                    if (ani.frames[i].bounds.ContainsKey(tag) == false)
	                    {
	                        ani.frames[i].bounds[tag] = e_bounds;
	                    }
	                    else
	                    {
	                        Rect src = ani.frames[i].bounds[tag];
	                        ani.frames[i].bounds[tag] = Rect_Union(src, e_bounds);
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
            this.skipframe = true;
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
