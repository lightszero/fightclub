using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace clgf.Quadbuf
{
    public class QuadBuf
    {
        public Rectangle dest;
        public Rectangle src;
        public Texture2D tex;
        public Color color;
        public QuadBuf(Texture2D tex, Rectangle dest, Rectangle src, Color color)
        {
            this.tex = tex;
            this.dest = dest;
            this.src = src;
            this.color = color;
        }

        public static void Draw(SpriteBatch sb, IList<QuadBuf> bufs)
        {
            foreach (var b in bufs)
            {
                sb.Draw(b.tex, b.dest, b.src, b.color);
            }
        }

        public static void Draw(SpriteBatch sb, IList<QuadBuf> bufs,Point offect)
        {
            foreach (var b in bufs)
            {
                int x = b.dest.X + offect.X;
                int y = b.dest.Y + offect.Y;

                sb.Draw(b.tex, new Rectangle(x, y, b.dest.Width, b.dest.Height), b.src, b.color);
            }
        }
        public static void Draw(SpriteBatch sb, IList<QuadBuf> bufs,Vector2 scale, Point offect)
        {
            foreach (var b in bufs)
            {
                float x = b.dest.X * scale.X + offect.X;
                float y = b.dest.Y * scale.Y + offect.Y;
                float w = b.dest.Width * scale.X;
                float h = b.dest.Height * scale.Y;
                sb.Draw(b.tex,new Rectangle((int)x,(int)y,(int)w,(int)h), b.src, b.color);
            }
        }
        public static void Draw(SpriteBatch sb, IList<QuadBuf> bufs,Color color)
        {
            foreach (var b in bufs)
            {
                sb.Draw(b.tex, b.dest, b.src, color);
            }
        }

        public static void Draw(SpriteBatch sb, IList<QuadBuf> bufs, Point offect, Color color)
        {
            foreach (var b in bufs)
            {
                int x = b.dest.X + offect.X;
                int y = b.dest.Y + offect.Y;

                sb.Draw(b.tex, new Rectangle(x, y, b.dest.Width, b.dest.Height), b.src, color);
            }
        }
        public static void Draw(SpriteBatch sb, IList<QuadBuf> bufs, Vector2 scale, Point offect, Color color)
        {
            foreach (var b in bufs)
            {
                float x = b.dest.X * scale.X + offect.X;
                float y = b.dest.Y * scale.Y + offect.Y;
                float w = b.dest.Width * scale.X;
                float h = b.dest.Height * scale.Y;
                sb.Draw(b.tex, new Rectangle((int)x, (int)y, (int)w, (int)h), b.src, color);
            }
        }

    }
}
