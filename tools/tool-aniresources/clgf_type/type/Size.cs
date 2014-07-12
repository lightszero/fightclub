using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clgf.type
{
    public struct Size
    {
        public Size(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width;
        public int Height;
    }
}
