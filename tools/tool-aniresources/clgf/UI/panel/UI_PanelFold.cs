using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clgf.type;
using Microsoft.Xna.Framework;
using clgf.Scene;

namespace clgf.UI.panel
{
    public class UI_PanelFold:UI_Panel //会改变尺寸的面板
    {
        public override Scene.SceneObj OnCopy()
        {
            UI_PanelFold e = new UI_PanelFold(this.foldElement.Size,this.unfoldElement.Size);
            //this
            e.anim = this.anim;
            e.fold = this.fold;

            if (this.Children==null|| !this.Children.Contains(foldElement))
            {
                e.foldElement= foldElement.Copy() as UI_Panel;
            }
            if (this.Children == null || !this.Children.Contains(unfoldElement))
            {
                e.unfoldElement = unfoldElement.Copy() as UI_Panel;
            }       
            //panel
            e.borderleft = this.borderleft;
            e.bordertop = this.bordertop;
            e.borderright = this.borderright;
            e.borderbottom = this.borderbottom;
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
        public UI_PanelFold(Vector2 min, Vector2 max)
        {
            foldElement = new UI_Panel();
            foldElement.Size = min;
            foldElement.UpdateAlign(this.posHAlign, this.posVAlign);
            unfoldElement = new UI_Panel();
            unfoldElement.Size = max;
            unfoldElement.UpdateAlign(this.posHAlign, this.posVAlign);
        }
        public void SetSize(Vector2 min, Vector2 max)
        {
            foldElement.Size = min;
            unfoldElement.Size = max;
        }
        bool anim = false;
        bool fold = true;
        public void Fold()
        {
            fold = true;
            this.RemoveObject(foldElement);
            this.RemoveObject(unfoldElement);
            anim = true;
        }
        public void UnFold()
        {
            fold = false;
            this.RemoveObject(foldElement);
            this.RemoveObject(unfoldElement);
            anim = true;
        }
        public void OnFolded()
        {
            this.AddObject(foldElement);
            foldElement.needUpdate = true;
        }
        public void OnUnFolded()
        {
            this.AddObject(unfoldElement);
            foldElement.needUpdate = true;
        }
        //Children 正常组，一直显示
        UI_Panel foldElement; //折起时显示的部分

        UI_Panel unfoldElement;//打开时显示的部分

        public enum ObjShowStyle
        {
            ShowAllTime,
            ShowInFold,
            ShowInUnFold,
        }
        public void AddObject(SceneObj obj, ObjShowStyle s)
        {
            if (s == ObjShowStyle.ShowAllTime)
            {
                this.AddObject(obj);
            }
            else if (s == ObjShowStyle.ShowInFold)
            {
                foldElement.AddObject(obj);
            }
            else if (s == ObjShowStyle.ShowInUnFold)
            {
                unfoldElement.AddObject(obj);
            }
        }
        protected override void OnUpdate(float delta)
        {//动画展开与折叠
            base.OnUpdate(delta);
            foldElement.UpdateAlign(this.posHAlign, this.posVAlign);
            unfoldElement.UpdateAlign(this.posHAlign, this.posVAlign);
            if (anim)
            {
                Vector2 tsize;
                if (fold) tsize = foldElement.Size;
                else tsize = unfoldElement.Size;


                if(false)
                {
                    Size = tsize;
                    anim = false;
                    if (fold) OnFolded();
                    else OnUnFolded();
                    return;
                }

                float ydist = 0;
                if (this.posVAlign == 0)
                {
                    float ydir = tsize.Y - Size.Y;
                    float ylen = Math.Abs(ydir);
                    ydir =Math.Sign(ydir);
                    float y =  delta * 200 * 3;
                    if (y > ylen) y = ylen;
                    Size.Y += y*ydir;
                    ydist = tsize.Y - Size.Y;
                }
                float xdist = 0;
                if (this.posHAlign == 0)
                {
                    float xdir = tsize.X - Size.X;
                    float xlen = Math.Abs(xdir);
                    xdir = Math.Sign(xdir);
                    float x = delta * 200 * 3;
                    if (x > xlen) x = xlen;
                    Size.X += x * xdir;
                    xdist = tsize.X - Size.X;
                }

                if (ydist==0&&xdist==0)
                {
                    anim = false;
                    if (fold) OnFolded();
                    else OnUnFolded();
                }

            }

        }
    }
}
