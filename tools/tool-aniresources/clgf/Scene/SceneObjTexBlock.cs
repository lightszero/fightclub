using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using clgf.Anim;

namespace clgf.Scene
{
    public class SceneObjTexBlock:SceneObj
    {
        public override SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        public clgf.Texture.TexturePacket.TextureBlock block;
        public Vector2 seedsize=new Vector2(0,0);
        public Vector2 seedorient=new Vector2(0,0);
        public Color color = Color.White;
        public float rotate = 0;

        public void InitWithSeed(Seed seed,Texture.TextureMgr tmgr)
        {
            block=tmgr.GetTexture(seed.texname);
            seedsize=seed.size;
            seedorient =seed.orient;
        }
        public void InitWithTextureBlock(Texture.TexturePacket.TextureBlock tb)
        {
            block = tb;
            seedsize.X = (float)tb.parent.texture.Width * tb.uv.Width;
            seedsize.Y = (float)tb.parent.texture.Height * tb.uv.Height;
            seedorient = new Vector2(0, 0);
        }
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb,ref Point offect)
        {
            if (color.A == 0) return;
            float width = seedsize.X * this.scenescale.X;
            float height = seedsize.Y * this.scenescale.Y;
            float x = this.scenepos.X;
            float y = this.scenepos.Y;
            Rectangle dest = new Rectangle((int)x, (int)y, (int)width, (int)height);

           Rectangle src = new Rectangle((int)(block.uv.X * block.parent.texture.Width),
                                    (int)(block.uv.Y * block.parent.texture.Height),
                                    (int)(block.uv.Width * block.parent.texture.Width),
                                    (int)(block.uv.Height *block.parent.texture.Height));
                Vector2 orient = seedorient;
                orient.X *= src.Width / seedsize.X;
                orient.Y *= src.Height / seedsize.Y;

#if XNA

#else
                //monogame orient 实现有bug...
                //orient.X *= (float)block.parent.texture.Width /src.Width;
                //orient.Y *=  (float)block.parent.texture.Height/src.Height;

#endif
                //float rotate = rotate * Math.Sign(scenescale.X) * Math.Sign(scenescale.Y);
                sb.Draw(block.parent.texture, dest, src, color, rotate, orient, SpriteEffects.None, 0);
                //sb.Draw(d.block.parent.texture, new Rectangle((int)(pos.X - 2), (int)(pos.Y - 2), 4, 4), Color.Red);
        }
        
    }
}
