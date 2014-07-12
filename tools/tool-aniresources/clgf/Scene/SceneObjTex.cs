using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RectangleF=clgf.type.RectangleF;
using clgf.Anim;
namespace clgf.Scene
{
    public class SceneObjTex : SceneObj
    {
        public override SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        public Texture2D tex;
        public RectangleF srcuv;
        public Vector2 seedsize = new Vector2(0, 0);
        public Vector2 seedorient = new Vector2(0, 0);
        public Color color = Color.White;
        public float rotate = 0;

        public void InitWithSeed(Seed seed, Texture.TextureMgr tmgr)
        {
            var block = tmgr.GetTexture(seed.texname);
            tex = block.parent.texture;
            srcuv = block.uv;
            seedsize = seed.size;
            seedorient = seed.orient;
        }
        public void InitWithTexture(Texture2D texture)
        {
            tex = texture;
            srcuv = new RectangleF(0, 0, 1.0f, 1.0f);
            seedsize.X = (float)tex.Width * srcuv.Width;
            seedsize.Y = (float)tex.Height * srcuv.Height;
            seedorient = new Vector2(0, 0);
        }
        public bool bTrans = true;
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, ref Point offect)
        {
            if (color.A == 0) return;
            if (bTrans == false)
            {
                sb.End();
				sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque,SamplerState.PointClamp,DepthStencilState.None,RasterizerState.CullNone);
            }
            float width = (seedsize.X * this.scenescale.X);
            float height = seedsize.Y * this.scenescale.Y;
            float x = this.scenepos.X+offect.X;
            float y = this.scenepos.Y+offect.Y;
            Rectangle dest = new Rectangle((int)x, (int)y, (int)width, (int)height);

            Rectangle src = new Rectangle((int)(srcuv.X * tex.Width),
                                     (int)(srcuv.Y * tex.Height),
                                     (int)(srcuv.Width * tex.Width),
                                     (int)(srcuv.Height * tex.Height));
            Vector2 orient = seedorient;
            orient.X *= src.Width / seedsize.X;
            orient.Y *= src.Height / seedsize.Y;

#if XNA

#else
                //monogame orient 实现有bug...
                //orient.X *= (float)tex.Width /src.Width;
                //orient.Y *=  (float)tex.Height/src.Height;

#endif
            //float rotate = rotate * Math.Sign(scenescale.X) * Math.Sign(scenescale.Y);
            sb.Draw(tex, dest, src, color, rotate, orient, SpriteEffects.None, 0);
            //sb.Draw(d.block.parent.texture, new Rectangle((int)(pos.X - 2), (int)(pos.Y - 2), 4, 4), Color.Red);
            if (bTrans == false)
            {
                sb.End();
				sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied,SamplerState.LinearClamp,DepthStencilState.None,RasterizerState.CullNone);
            }
        }
    }

}
