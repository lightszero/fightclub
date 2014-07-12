using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace clgf.UI
{
    public class UI_Btn : UI.UIElement
    {
        public override clgf.Scene.SceneObj OnCopy()
        {
            UI_Btn e = new UI_Btn();
            //this
            e.btntag = this.btntag;
            e.imgNormal = this.imgNormal;
            e.imgTouch = this.imgTouch;

            //uielement
            e.Text = this.Text;
            e.Orient = this.Orient;
            e.Size = this.Size;
            e.posHAlign = this.posHAlign;
            e.posVAlign = this.posVAlign;
            e.ClipRender = this.ClipRender;
            //scene
            e._pos = this._pos;
            e._scale = this._scale;
            e.needUpdate = true;



            return e;
        }
        public Scene.SceneObj imgNormal;
        public Scene.SceneObj imgTouch;

        bool bTouch = false;
        public string btntag = "";
        public delegate void delebtnevent(string tag);
        public event delebtnevent onTouchDown;
        public event delebtnevent onTouchUp;
        public event delebtnevent onTouchLeft;

        protected override bool OnTouch(Microsoft.Xna.Framework.Input.Touch.TouchCollection touchs)
        {
            Vector2 lt = this.scenepos - (Orient) * this.scenescale;
            Vector2 rb = lt + this.scenescale * Size;
            bool ntouch = false;
            bool ntouchup = false;
            foreach (var t in touchs)
            {
                if (t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed || t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Moved)
                {
                    if (lt.X <= t.Position.X && t.Position.X <= rb.X
                        && lt.Y <= t.Position.Y && t.Position.Y <= rb.Y)
                    {
                        ntouch = true;
                        break;
                    }
                }
                if (t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Released || t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Invalid)
                {
                    if (lt.X <= t.Position.X && t.Position.X <= rb.X
                        && lt.Y <= t.Position.Y && t.Position.Y <= rb.Y)
                    {
                        ntouchup = true;
                        break;
                    }
                }
            }
            if (ntouch == true && bTouch == false)
            {
                //按下
                bTouch = true;
                if (onTouchDown != null)
                    onTouchDown(btntag);
                return true;
            }
            if (ntouchup == true && bTouch == true)
            {
                //弹起
                bTouch = false;
                if (onTouchUp != null)
                    onTouchUp(btntag);
                return true;
            }
            if (ntouch == false && ntouchup == false)
            {
                if (onTouchLeft != null)
                    onTouchLeft(btntag);
            }
            bTouch = ntouch;
            imgTouch.needUpdate = true;
            if (imgNormal.Children != null)
                foreach (var i in imgNormal.Children)
                {
                    i.needUpdate = true;
                }
            imgNormal.needUpdate = true;
            if (imgTouch.Children != null)
                foreach (var i in imgTouch.Children)
                {
                    i.needUpdate = true;
                }
            return false;
        }
        public override void OnUpdateLayout()
        {
            imgTouch.needUpdate = true;
            imgNormal.needUpdate = true;
            if (imgTouch is UIElement)
                (imgTouch as UIElement).UpdateLayout();
            if (imgNormal is UIElement)
                (imgNormal as UIElement).UpdateLayout();
        }
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            //Vector2 lt = this.scenepos - (orient) * this.scenescale;
            //Vector2 rb = lt + this.scenescale * size;
           
            //imgNormal.scale = scenescale;
            //if (bTouch)
            {
                imgTouch.SetParent(this);

                imgTouch.Update(delta);
            }
            //else
            {
                imgNormal.SetParent(this);
                imgNormal.Update(delta);
            }

            //imgTouch.pos = this.scenepos ;
            //imgTouch.scale = scenescale;

        }
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, ref Point offect)
        {
            if (bTouch)
            {
                //imgTouch.Update(0);
                imgTouch.Draw(sb, ref offect);
            }
            else
            {

                imgNormal.Draw(sb, ref offect);

            }
        }
    }
}
