using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace clgf.type
{
    public struct RectangleF
    {
        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Left
        {
            get
            {
                return X;
            }
        }
        public float Top
        {
            get
            {
                return Y;
            }
        }
        public float Right
        {
            get
            {
                return X+Width;
            }
        }
        public float Bottom
        {
            get
            {
                return Y+Height;
            }
        }
        public void Offset(float x, float y)
        {
            this.X += x;
            this.Y += y;
        }

        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            float x =a.X < b.X ? a.X : b.X;
            float y=a.Y < b.Y ? a.Y : b.Y;
            float r = a.Right>b.Right?a.Right:b.Right;
            float _b =a.Bottom>b.Bottom?a.Bottom:b.Bottom;
            return new RectangleF(x, y, r-x,_b-y);
             
        }
        public static implicit operator Rectangle(RectangleF s)
        {
            return new Rectangle((int)s.X, (int)s.Y, (int)s.Width, (int)s.Height);
        }
    }
}
