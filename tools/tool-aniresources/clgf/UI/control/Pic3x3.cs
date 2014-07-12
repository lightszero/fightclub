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
    public class Pic3x3 : UIElement
    {
        public override Scene.SceneObj OnCopy()
        {
            Pic3x3 e = new Pic3x3();
            //this
            e.drawblock = this.drawblock;
            e.borderleft = this.borderleft;
            e.bordertop = this.bordertop;
            e.borderright = this.borderright;
            e.borderbottom = this.borderbottom;
            e.color = this.color;
            //e.seedorient = this.seedorient;
            //e.seedsize = this.seedsize;
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
        public int borderleft = 1;
        public int bordertop = 1;
        public int borderright = 1;
        public int borderbottom = 1;

        //public Vector2 seedorient = new Vector2(0, 0);
        //public Vector2 seedsize = new Vector2(0, 0);
        public void InitWithSeed(Seed seed, Texture.TextureMgr tmgr)
        {
            //seedorient = seed.orient;
            //seedsize = seed.size;
            this.Size = seed.size;
            this.Orient = seed.orient;
            drawblock = tmgr.GetTexture(seed.texname);
        }
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
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            Vector2 lt = this.scenepos;
            lt.X -= (  Orient.X) * scenescale.X;
            lt.Y -= ( Orient.Y) * scenescale.Y;
            Vector2 rb = lt;
            rb.X += Size.X * scenescale.X;
            rb.Y += Size.Y * scenescale.Y;
            var tex = drawblock.parent.texture;
            Rectangle src = new Rectangle((int)(drawblock.uv.X * tex.Width),
                (int)(drawblock.uv.Y * tex.Height),
                (int)(drawblock.uv.Width * tex.Width),
                (int)(drawblock.uv.Height * tex.Height));
            Rectangle dest = new Rectangle((int)lt.X, (int)lt.Y, (int)(rb.X - lt.X), (int)(rb.Y - lt.Y));
            bufs.Clear();

            //top
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.X, dest.Y, borderleft, bordertop), new Rectangle(src.X, src.Y, borderleft, bordertop), Color.White));
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.X + borderleft, dest.Y, dest.Width - borderleft - borderright, bordertop), new Rectangle(src.X + borderleft, src.Y, src.Width - borderleft - borderright, bordertop), Color.White));
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.Right - borderright, dest.Y, borderright, bordertop), new Rectangle(src.Right - borderright, src.Y, borderright, bordertop), Color.White));

            //mid
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.X, dest.Y + bordertop, borderleft, dest.Height - bordertop - borderbottom), new Rectangle(src.X, src.Y + bordertop, borderleft, src.Height - bordertop - borderbottom), Color.White));
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.X + borderleft, dest.Y + bordertop, dest.Width - borderleft - borderright, dest.Height - borderleft - borderright), new Rectangle(src.X + borderleft, src.Y + bordertop, src.Width - borderleft - borderright, src.Height - bordertop - borderbottom), Color.White));
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.Right - borderright, dest.Y + bordertop, borderright, dest.Height - bordertop - borderbottom), new Rectangle(src.Right - borderright, src.Y + bordertop, borderright, src.Height - bordertop - borderbottom), Color.White));

            //bottom
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.X, dest.Bottom - borderbottom, borderleft, borderbottom), new Rectangle(src.X, src.Bottom - borderbottom, borderleft, borderbottom), Color.White));
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.X + borderleft, dest.Bottom - borderbottom, dest.Width - borderleft - borderright, borderbottom), new Rectangle(src.X + borderleft, src.Bottom - borderbottom, src.Width - borderleft - borderright, borderbottom), Color.White));
            bufs.Add(new QuadBuf(tex, new Rectangle(dest.Right - borderright, dest.Bottom - borderbottom, borderright, borderbottom), new Rectangle(src.Right - borderright, src.Bottom - borderbottom, borderright, borderbottom), Color.White));
        }
        List<QuadBuf> bufs = new List<QuadBuf>();
        Microsoft.Xna.Framework.Point offect;
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, ref Microsoft.Xna.Framework.Point offect)
        {
            this.offect = offect;
            QuadBuf.Draw(sb, bufs, offect,color);
        }
    }
}
