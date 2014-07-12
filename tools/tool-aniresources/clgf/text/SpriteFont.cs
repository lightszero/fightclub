using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Size = clgf.type.Size;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.Game.Packing;
using clgf.Anim;
using clgf.Texture;
namespace clgf.text
{
    public interface IFont
    {
        Size? GetCharSize(char c);
        void DrawString(SpriteBatch sb, string str, Vector2 pos, Vector2 scale, Color color);
        void DrawStringToBuf(IList<Quadbuf.QuadBuf> bufs, string str, Vector2 pos, Vector2 Scale, Color color);
    }
    struct charpos
        {
            public charpos(Rectangle _src, int _id)
            {
                src = _src;
                texid = _id;
            }
                public Rectangle src;
                public int texid;
        }
    public class SpriteFontSimple:IFont
    {
        public SpriteFontSimple(GraphicsDevice gdevice, Font font)
        {
            _font = font;
            _device = gdevice;
            //if (_font.maxcharheight <= 16)
            {
                _tex.Add(new Texture2D(gdevice, 512, 512));
                packer = new SimpleRectanglePacker(512, 512);
            }
            //else
            //{
            //    _tex.Add(new Texture2D(gdevice, 1024, 1024));
            //    packer = new SimpleRectanglePacker(1024, 1024);
            //}
        }
        Font _font;
        List<Texture2D> _tex = new List<Texture2D>();
        Dictionary<char, charpos> charposs = new Dictionary<char, charpos>();
        SimpleRectanglePacker packer;
        GraphicsDevice _device;

        void FillChar(char c)
        {
            if (charposs.ContainsKey(c)) return;
            UInt32[] data;
            var size=
            _font.GetCharData(c,out data);
            Point p;
            var b=packer.TryPack(size.Width, size.Height, out p);
            Texture2D tex;
            if (b)
            {
                charposs[c] = new charpos(
                    new Rectangle(p.X, p.Y, size.Width, size.Height)
                    , _tex.Count - 1
                    );

            }
            else
            {
                //if (_font.maxcharheight <= 16)
                {
                    _tex.Add(new Texture2D(_device, 512, 512));
                    packer = new SimpleRectanglePacker(512, 512);
                }
                //else
                //{
                //    _tex.Add(new Texture2D(_device, 1024, 1024));
                //    packer = new SimpleRectanglePacker(1024, 1024);
                //}
                packer.TryPack(size.Width, size.Height, out p);
                charposs[c] = new charpos(
                    new Rectangle(p.X, p.Y, size.Width, size.Height)
                    , _tex.Count - 1
                    );
            }
            tex = _tex[_tex.Count - 1];
            tex.SetData<UInt32>(0, charposs[c].src, data, 0, data.Length);
        }
        public Size? GetCharSize(char c)
        {
            FillChar(c);
            return new Size(charposs[c].src.Width,charposs[c].src.Height);
        }
        public void DrawString(SpriteBatch sb, string str,Vector2 pos,Vector2 Scale,Color color)
        {
            sb.GraphicsDevice.Textures[0] = null;
            foreach (var c in str)
            {
                FillChar(c);
                
            }
            float x = 0;
            foreach (var c in str)
            {
                var __pos =charposs[c];
                float height = __pos.src.Height;
                float width = __pos.src.Width;
                height *= Scale.Y;
                width *= Scale.X;
                Rectangle dest = new Rectangle((int)(pos.X + x), (int)pos.Y, (int)width, (int)height);
                sb.Draw(_tex[__pos.texid],dest, __pos.src, color);
                x +=width;
            }
            
        }
        public void DrawStringToBuf(IList<Quadbuf.QuadBuf> bufs, string str, Vector2 pos, Vector2 Scale, Color color)
        {
            foreach (var c in str)
            {
                FillChar(c);

            }
            float x = 0;
            foreach (var c in str)
            {
                var __pos = charposs[c];
                float height = __pos.src.Height;
                float width = __pos.src.Width;
                height *= Scale.Y;
                width *= Scale.X;
                Rectangle dest = new Rectangle((int)(pos.X + x), (int)pos.Y, (int)width, (int)height);
                bufs.Add(new Quadbuf.QuadBuf(_tex[__pos.texid], dest, __pos.src, color));
                x += width;
            }
            
        }
    }
    public class SpriteFontDoubleColor:IFont
    {
        public SpriteFontDoubleColor(GraphicsDevice gdevice,Font font,Color fillcolor,Color bordercolor)
        {
            _font = font;
            _device = gdevice;
            _fillcolor = fillcolor;
            _bordercolor = bordercolor;
            //if (_font.maxcharheight <= 16)
            {
                _tex.Add(new Texture2D(gdevice, 512, 512));
                packer = new SimpleRectanglePacker(512, 512);
            }
            //else
            //{
            //    _tex.Add( new Texture2D(gdevice, 1024, 1024));
            //    packer = new SimpleRectanglePacker(1024, 1024);
            //}
        }
        Color _fillcolor;
        Color _bordercolor;
        Font _font;
        List<Texture2D> _tex = new List<Texture2D>();
        Dictionary<char, charpos> charposs = new Dictionary<char, charpos>();
        SimpleRectanglePacker packer;
        GraphicsDevice _device;

        void FillChar(char c)
        {
            if (charposs.ContainsKey(c)) return;
            UInt32[] data;
            Size size=
            _font.GetCharDataDC(c,out data,_fillcolor,_bordercolor);
            Point p;
            var b=packer.TryPack(size.Width, size.Height, out p);
            Texture2D tex;
            if (b)
            {
                charposs[c] = new charpos(
                    new Rectangle(p.X, p.Y, size.Width, size.Height)
                    , _tex.Count - 1
                    );

            }
            else
            {
                //if (_font.maxcharheight <= 16)
                {
                    _tex.Add(new Texture2D(_device, 512, 512));
                    packer = new SimpleRectanglePacker(512, 512);
                }
                //else
                //{
                //    _tex.Add(new Texture2D(_device, 1024, 1024));
                //    packer = new SimpleRectanglePacker(1024, 1024);
                //}
                packer.TryPack(size.Width, size.Height, out p);
                charposs[c] = new charpos(
                    new Rectangle(p.X, p.Y, size.Width, size.Height)
                    , _tex.Count - 1
                    );
            }
            tex = _tex[_tex.Count - 1];
            tex.SetData<UInt32>(0, charposs[c].src, data, 0, data.Length);
        }
        public Size? GetCharSize(char c)
        {
            FillChar(c);
            return new Size(charposs[c].src.Width,charposs[c].src.Height);
        }
        public void DrawString(SpriteBatch sb, string str,Vector2 pos,Vector2 scale, Color color)
        {
            sb.GraphicsDevice.Textures[0] = null;
            foreach (var c in str)
            {
                FillChar(c);
                
            }
            //int x = 0;
            //foreach (var c in str)
            //{
            //    var __pos =charposs[c];
            //    sb.Draw(_tex[__pos.texid], new Vector2(pos.X + x, pos.Y), __pos.src, Color.White);
            //    x += __pos.src.Width;
            //}
            float x = 0;
            foreach (var c in str)
            {
                var __pos = charposs[c];
                float height = __pos.src.Height;
                float width = __pos.src.Width;
                height *= scale.Y;
                width *= scale.X;
                Rectangle dest = new Rectangle((int)(pos.X + x), (int)pos.Y, (int)width, (int)height);
                sb.Draw(_tex[__pos.texid], dest, __pos.src, color);
                x += width;
            }
        }


        public void DrawStringToBuf(IList<Quadbuf.QuadBuf> bufs, string str, Vector2 pos, Vector2 scale, Color color)
        {
          
            //int x = 0;
            foreach (var c in str)
            {
                FillChar(c);

            }
            float x = 0;
            foreach (var c in str)
            {
                var __pos = charposs[c];
                float height = __pos.src.Height;
                float width = __pos.src.Width;
                height *= scale.Y;
                width *= scale.X;
                Rectangle dest = new Rectangle((int)(pos.X + x), (int)pos.Y, (int)width, (int)height);
                bufs.Add(new Quadbuf.QuadBuf(_tex[__pos.texid], dest, __pos.src, color));
                x += width;
            }
        }
    }

    public class SpriteFontWithSeed : IFont
    {
        string baseseed;
        int fontsize;
        Sprite sprite;
        TextureMgr tmgr;
        string texpath;
        public SpriteFontWithSeed(int fontsize, Sprite sprite ,string baseseed,TextureMgr tmgr, string texpath)
        {
            this.fontsize = fontsize;
            this.baseseed = baseseed;
            this.sprite = sprite;
            this.tmgr = tmgr;
            this.texpath = texpath;
        }
        public struct tpos
        {
            public tpos(Seed seed, clgf.Texture.TexturePacket.TextureBlock block)
            {

                this.block = block;
                this.size = new Size((int)seed.size.X, (int)seed.size.Y);

                if (seed.size.X - size.Width > 0) this.size.Width++;
                if (seed.size.Y - size.Height > 0) this.size.Height++;
                this.orientY = seed.orient.Y;
            }
            public clgf.Texture.TexturePacket.TextureBlock block;
            public float orientY;
            public Size size;
            public Size GetSize()
            {
                int i = (int)orientY;
                if (orientY - i > 0) i++;
                return new Size(size.Width, i);
            }
        }
        Dictionary<char, tpos> blocks = new Dictionary<char, tpos>();
        public Size? GetCharSize(char c)
        {
            if (blocks.ContainsKey(c)) return blocks[c].GetSize();
            string key = baseseed + " (" + c + ")";
            string key2 = baseseed + "(" + c + ")";
            Seed seed = null;
            if (sprite.seeds.ContainsKey(key))
            {
                 seed = sprite.seeds[key];
            }
            else if (sprite.seeds.ContainsKey(key2))
            {
                seed = sprite.seeds[key2];
            }
            if (seed == null) return null;
            blocks[c] = new tpos(seed, tmgr.GetTexture(System.IO.Path.Combine(texpath, seed.texname)));
            return null;
        }

        public void DrawString(SpriteBatch sb, string str, Vector2 pos, Vector2 scale, Color color)
        {
            float x = 0;
            foreach (var c in str)
            {
                Size? size= GetCharSize(c);
                if (size == null) continue;
                float width = blocks[c].size.Width * scale.X;
                float height = blocks[c].size.Height * scale.Y;
                float yadd = fontsize- blocks[c].orientY;
                Rectangle dest = new Rectangle((int)(pos.X + x), (int)(pos.Y + yadd), (int)width, (int)height);
                var tex=blocks[c].block.parent.texture;
                Rectangle src = new Rectangle((int)(blocks[c].block.uv.X * tex.Width), (int)(blocks[c].block.uv.Y * tex.Height), (int)(blocks[c].block.uv.Width * tex.Width), (int)(blocks[c].block.uv.Height * tex.Height));
                sb.Draw(tex, dest,src , color);
                x += width;
            }
        }


        public void DrawStringToBuf(IList<Quadbuf.QuadBuf> bufs, string str, Vector2 pos, Vector2 scale, Color color)
        {

            float x = 0;
            foreach (var c in str)
            {
                Size? size = GetCharSize(c);
                if (size == null) continue;
                float width = blocks[c].size.Width * scale.X;
                float height = blocks[c].size.Height * scale.Y;
                float yadd = fontsize - blocks[c].orientY;
                Rectangle dest = new Rectangle((int)(pos.X + x), (int)(pos.Y + yadd), (int)width, (int)height);
                var tex = blocks[c].block.parent.texture;
                Rectangle src = new Rectangle((int)(blocks[c].block.uv.X * tex.Width), (int)(blocks[c].block.uv.Y * tex.Height), (int)(blocks[c].block.uv.Width * tex.Width), (int)(blocks[c].block.uv.Height * tex.Height));
                bufs.Add(new Quadbuf.QuadBuf(tex, dest, src, color));
                x += width;
            }
        }
    }
}
