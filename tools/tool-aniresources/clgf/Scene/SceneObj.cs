using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using clgf.Anim;

namespace clgf.Scene
{
    public class SceneObj
    {
        public SceneObj()
        {
            needUpdate = true;
            _pos = new Vector2(0, 0);
            _scale = new Vector2(1, 1);
        }
        public virtual SceneObj OnCopy()
        {
            SceneObj o = new SceneObj();
            o._pos = this._pos;
            o._scale = this._scale;
            o.needUpdate = true;
            return o;
        }
        public SceneObj Copy()
        {
            SceneObj e = OnCopy();
            if (this.Children != null)
            {
                foreach (var i in this.Children)
                {
                    e.AddObject( i.Copy());
                }
            }
            return e;
        }
        protected Vector2 _pos;
        public Vector2 pos
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                needUpdate = true;
            }
        }
        protected Vector2 _scale;
        public Vector2 scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                needUpdate = true;
            }
        }
        public Vector2 scenepos
        {
            get;
            protected set;
        }
            //= new Vector2(0, 0);
        public Vector2 scenescale
        {
            get;
            protected set;
        }
            //= new Vector2(1, 1);


        public SceneObj parent
        {
            get;
            protected set;
        }
        public void SetParent(SceneObj obj)
        {
            parent = obj;
        }
        protected virtual void OnDraw(SpriteBatch sb, ref Point offect)
        {
        }
        protected virtual void OnDrawShadow(SpriteBatch sb, ref Point offect)
        {
        }
        public virtual void Draw(SpriteBatch sb, ref Point offect)
        {
            OnDraw(sb, ref offect);
			if(Children!=null)
            foreach (var c in Children)
            {
                c.Draw(sb,ref offect);
            }

        }
        public void DrawShadow(SpriteBatch sb,ref Point  offect)
        {
            OnDrawShadow(sb, ref offect);
			if(Children!=null)
            foreach (var c in Children)
            {
                c.DrawShadow(sb,ref offect);
            }
        }
        protected virtual void OnUpdate(float delta)
        {
        }
        public Vector2 CalcScenePos(Vector2 pos)
        {
            return pos * parent.scenescale + parent.scenepos;
        }
        public virtual bool needUpdate
        {
            get;
            set;
        }
        public void NeedUpdateAll()
        {
            this.needUpdate = true;
            if(Children!=null)
            foreach (var c in Children)
            {
                c.NeedUpdateAll();
            }
        }
        public void Update(float delta)
        {

            if (needUpdate)
            {
                if (parent != null)
                {
                    this.scenepos = this.pos * parent.scenescale + parent.scenepos;// (parent.scenepos+this.pos) * parent.scenescale ;//+ this.pos;
                    this.scenescale = parent.scenescale * this.scale;
                }
                else
                {
                    this.scenepos = pos;
                    this.scenescale = scale;
                }
                //needUpdate = false;
            }
            OnUpdate(delta);
			if(Children!=null)
            foreach (var c in Children)
            {
                c.Update(delta);
            }
        
        }
        public List<SceneObj> Children
        {
            get;
            private set;
        }
        public void AddObject(SceneObj obj)
        {
			if(Children==null)Children = new List<SceneObj>();
            Children.Add(obj);
            obj.parent = this;
        }
        public void RemoveObject(SceneObj obj)
        {
            Children.Remove(obj);
        }
    }
}
