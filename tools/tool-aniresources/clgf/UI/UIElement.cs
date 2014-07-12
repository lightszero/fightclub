using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using clgf.type;

namespace clgf.UI
{
    public class UIElement : Scene.SceneObj
    {

        public override Scene.SceneObj OnCopy()
        {
            UIElement e = new UIElement();
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

        //触摸
        //return flase 继续
        //return true 吞掉消息
        public bool Touch(TouchCollection touchs)
        {
            if (Children != null)
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    UIElement uie = Children[i] as UIElement;
                    if (uie == null) continue;

                    if (uie.Touch(touchs)) return true;
                }
            return OnTouch(touchs);
        }

        protected virtual bool OnTouch(TouchCollection touchs)
        {
            return false;
        }


        //UI组件标准配置，有Size 和 Orient 才好对齐
        public virtual string Text
        {
            get;
            set;
        }
        public Vector2 Size = new Vector2(100, 100);
        public Vector2 Orient = new Vector2(0, 0);

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)(scenepos.X - Orient.X * scenescale.X),
                    (int)(scenepos.Y - Orient.Y * scenescale.Y),
                    (int)(Size.X * scenescale.X), (int)(Size.Y * scenescale.Y));
            }
        }

        public RectangleF RectF
        {
            get
            {
                return new RectangleF((scenepos.X - Orient.X * scenescale.X),
                    (scenepos.Y - Orient.Y * scenescale.Y),
                    (Size.X * scenescale.X), (Size.Y * scenescale.Y));
            }
        }
        public virtual void OnUpdateLayout()
        {
            this.needUpdate = true;
        }
        public void UpdateLayout()
        {
            OnUpdateLayout();
            if (Children != null)
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    UIElement uie = Children[i] as UIElement;
                    if (uie == null) continue;

                    uie.UpdateLayout();
                }
        }
        //对齐
        public int posHAlign//左右对齐，0居左，1居中，2居右 
        {
            get;
            protected set;
        }
        public int posVAlign//左右对齐，0居左，1居中，2居右 
        {
            get;
            protected set;
        }
        public void UpdateAlign(int halign, int valign)
        {
            this.posHAlign = halign;
            this.posVAlign = valign;
        }
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
                float sx = 1.0f;
                if (posHAlign == 3)
                {
                    sx = (float)p.Rect.Width / (float)this.Rect.Width;

                }
                float sy = 1.0f;
                if (posVAlign == 3)
                {
                    sy = (float)p.Rect.Height / (float)this.Rect.Height;

                }
                scenescale = new Vector2(scenescale.X * sx, scenescale.Y * sy);
            }
        }
        //裁剪
        public void SetClipRender(bool v)
        {
            ClipRender = v;
        }
        protected bool ClipRender = false;
        public override void Draw(SpriteBatch sb, ref Point offect)
        {
            if (!ClipRender)
            {
                base.Draw(sb, ref offect);
                return;
            }
            OnDraw(sb, ref offect);
            sb.End();
            Rectangle old = sb.GraphicsDevice.Viewport.Bounds;
            Rectangle rf = new Rectangle((int)(scenepos.X - Orient.X * scenescale.X + offect.X), (int)(scenepos.Y - Orient.Y * scenescale.Y + offect.Y), (int)(Size.X * scenescale.X), (int)(Size.Y * scenescale.Y));
            if (rf.Right > old.Right)
            {
                rf.Width = old.Right - rf.Left; 
            }
            if (rf.Bottom > old.Bottom)
            {
                rf.Height = old.Bottom - rf.Top;
            }

            Point off2 = offect;
            try
            {
              sb.GraphicsDevice.Viewport = new Viewport(rf);
              off2.X -= rf.X;
              off2.Y -= rf.Y;
            }
            catch
            {
            }
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);

            if (Children != null)
            {

                foreach (var c in Children)
                {
                    c.Draw(sb, ref off2);
                }
                sb.End();
            }
            try
            {
                sb.GraphicsDevice.Viewport = new Viewport(old);
            }
            catch
            {
            }
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
        }
    }
}
