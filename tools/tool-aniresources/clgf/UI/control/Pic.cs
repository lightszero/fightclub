using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clgf.Scene;
using clgf.type;
using Microsoft.Xna.Framework;
using clgf.Anim;
using clgf.Quadbuf;

namespace clgf.UI
{
    public class Pic : UIElement
    {
        public override Scene.SceneObj OnCopy()
        {
            Pic e = new Pic();
            //this
            e.drawblock = this.drawblock;
            e.color = this.color;
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
        Texture.TexturePacket.TextureBlock drawblock;
        public Color color = Color.White;
        public void InitWithSeed(Seed seed, Texture.TextureMgr tmgr)
        {
            this.Orient = seed.orient;
            drawblock = tmgr.GetTexture(seed.texname);
            Size = seed.size;
        }

        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            Vector2 lt = this.scenepos;
            lt.X -= (Orient.X) * scenescale.X;
            lt.Y -= (Orient.Y) * scenescale.Y;
            Vector2 rb = lt;
            rb.X += Size.X * scenescale.X;
            rb.Y += Size.Y * scenescale.Y;
            var tex = drawblock.parent.texture;

            dest = new Rectangle((int)lt.X, (int)lt.Y, (int)(rb.X - lt.X), (int)(rb.Y - lt.Y));
        }
        Rectangle dest;
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, ref Microsoft.Xna.Framework.Point offect)
        {
            var tex = drawblock.parent.texture;
            Rectangle src = new Rectangle((int)(drawblock.uv.X * tex.Width),
     (int)(drawblock.uv.Y * tex.Height),
     (int)(drawblock.uv.Width * tex.Width),
     (int)(drawblock.uv.Height * tex.Height));
            Rectangle sdest = dest;
            sdest.X += offect.X;
            sdest.Y += offect.Y;
            sb.Draw(tex, sdest, src, color);

        }
    }
}
