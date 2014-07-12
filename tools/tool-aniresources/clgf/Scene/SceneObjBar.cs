using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace clgf.Scene
{
    public class SceneObjBar:SceneObj
    {
        public override SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        SceneObj front;
        SceneObj back;
        Vector2 orcfrontscale;
        Vector2 orcbackscale;
        float _value = 0;
        float _valueback = 0;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public void SetPic(SceneObj front, SceneObj back)
        {
            this.front = front;
            orcfrontscale = front.scale;
            this.front.pos = new Vector2(0, 0);
            this.back = back;
            orcbackscale =back.scale;
            this.back.pos =new Vector2(0,0);

            this.AddObject(back);
            this.AddObject(front);
        }
        
        protected override void OnUpdate(float delta)
        {
            if (_value < _valueback)//扣血模式
            {
                _valueback -= 0.3f * delta;
                if (_valueback < _value) _valueback = _value;
                Vector2 s1 = orcfrontscale;
                s1.X *= _value;
                this.front.scale = s1;
                Vector2 s2 = orcbackscale;
                s2.X*=  _valueback;
                this.back.scale = s2;
            }
            else//增长模式
            {
                _valueback += 0.3f * delta;
                if (_valueback > _value) _valueback = _value;
                Vector2 s1 = orcfrontscale;
                s1.X *= _valueback;
                this.front.scale = s1;
                Vector2 s2 = orcbackscale;
                s2.X *= _valueback;
                this.back.scale = s2;
            }
            base.OnUpdate(delta);
            front.Update(delta);
            back.Update(delta);
        }
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb,ref Point offect)
        {
            base.OnDraw(sb,ref offect);
            back.Draw(sb,ref offect);
            front.Draw(sb,ref offect);
        }
    }
}
