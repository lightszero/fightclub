using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace clgf.UI.panel
{
    //辅助定位的面板
    //面板在撑满时不改变缩放比率,而是修改自己的Size
    public class UI_Panel:UIElement
    {
        public override Scene.SceneObj OnCopy()
        {
            UI_Panel e = new UI_Panel();
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
        public int borderleft = 1;
        public int bordertop = 1;
        public int borderright = 1;
        public int borderbottom = 1;
        public void SetBorder(int w)
        {
            borderleft =
            bordertop =
            borderright =
            borderbottom = w;
        }
        public void SetBorder(int l, int t, int r, int b)
        {
            borderleft = l;
            bordertop = t;
            borderright = r;
            borderbottom = b;
        }
        //，3=第四种 fill,撑满
        
        protected override void OnUpdate(float delta)
        {
            UIElement p = parent as UIElement;
            if (p != null && (posHAlign > 0 || posVAlign > 0))
            {
                float xadd = 0;
                if (posHAlign < 3)
                {

                    int width = p.Rect.Width - this.Rect.Width;
                    xadd = (width / 2.0f * posHAlign);
                }
                float yadd = 0;
                if (posVAlign < 3)
                {
                    int height = p.Rect.Height - this.Rect.Height;
                    yadd = (height / 2.0f * posVAlign);
                }
                scenepos = new Vector2(scenepos.X + xadd, scenepos.Y + yadd);
                Vector2 npos = this.pos;
                float ny = this.pos.Y;
                if (posHAlign == 3)
                {
                    npos.X = borderleft / scenescale.X / scale.X;
                    this.Size.X = (p.RectF.Width - borderright - borderleft) / scenescale.X/scale.X;
                }
                if (posVAlign == 3)
                {
                    npos.Y = bordertop / scenescale.Y / scale.Y;
                    this.Size.Y = (p.RectF.Height -borderbottom - bordertop) / scenescale.Y/scale.Y;
                }
                this.pos = npos;
            }
        }
    }
}
