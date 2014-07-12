using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using Size = clgf.type.Size;
namespace clgf.text
{
    public class Font//GDI+的实现，不同平台需要不同实现
    {
        static Graphics gfont = null;
        static Bitmap gbmp = null;
        System.Drawing.Font GdiFont = null;
        public int maxcharheight
        {
            get;
            private set;
        }
        public  Font(string fontname,int fontsize)
        {
            if (gfont == null)
            {
                gbmp =new Bitmap(128,128);
                gfont = Graphics.FromImage(gbmp);
                gfont.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                gfont.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            }



            GdiFont = new System.Drawing.Font(fontname, fontsize, GraphicsUnit.Pixel);
            maxcharheight = GdiFont.Height;
      
            //设置文本输出质量
            //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //Font newFont = new Font("Times New Roman", 48);

        }

        public Size GetCharData(char c,out  UInt32[] cdata)
        {

            //try
            {
                Brush b = new SolidBrush(Color.White);
                gfont.Clear(Color.FromArgb(0));
                gfont.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;

                StringFormat sformat = new StringFormat();
                sformat.Alignment = StringAlignment.Near;
                sformat.LineAlignment = StringAlignment.Near;
                sformat.Trimming = StringTrimming.Character;
                sformat.FormatFlags = StringFormatFlags.NoClip;
                
                //var regions = gfont.MeasureCharacterRanges(a(c.ToString(), GdiFont, new RectangleF(0, 0, 64, 64), sformat);
                gfont.DrawString(c.ToString(), GdiFont, b, new PointF(0,0));
                var size=gfont.MeasureString(c.ToString(),GdiFont);
                //
                //    , clgf.type.Imaging.PixelFormat.Format32bppArgb);
                size.Width *= 72.0f / 96;
                int width = (int)size.Width;
                if (width < size.Width) width++;

                //width--;
                int height = maxcharheight;

                var bdata = gbmp.LockBits(new Rectangle(3, 0,width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                unsafe
                {
                    cdata =new UInt32[width*height];
                    fixed(UInt32* outp =cdata)
                    {
                        UInt32* src=(UInt32*) bdata.Scan0.ToPointer();
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                      
                                outp[y * width + x] = src[y * bdata.Stride / 4 + x];
                            }
                        }
                    }
                }
                gbmp.UnlockBits(bdata);
                return new Size(width, height);
                //gbmp.Save("c:\\abc\\1.png");
                //return size;

            }
            //catch
            //{
            //    cdata = null;
            //    return new Size(0, 0);
            //}
        }

        public Size GetCharDataDC(char c, out  UInt32[] cdata, Microsoft.Xna.Framework.Color fill, Microsoft.Xna.Framework.Color border)
        {
            try
            {
                Brush bF = new SolidBrush(Color.FromArgb((int)fill.PackedValue));
                Brush bB = new SolidBrush(Color.FromArgb((int)border.PackedValue));
                gfont.Clear(Color.FromArgb(0));
                gfont.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;

                StringFormat sformat = new StringFormat();
                sformat.Alignment = StringAlignment.Near;
                sformat.LineAlignment = StringAlignment.Near;
                sformat.Trimming = StringTrimming.Character;
                sformat.FormatFlags = StringFormatFlags.NoClip;

                //var regions = gfont.MeasureCharacterRanges(a(c.ToString(), GdiFont, new RectangleF(0, 0, 64, 64), sformat);
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(0, 0));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(0, 1));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(0, 2));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(2, 0));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(2, 1));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(2, 2));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(1, 0));
                gfont.DrawString(c.ToString(), GdiFont, bB, new PointF(1, 2));

                gfont.DrawString(c.ToString(), GdiFont, bF, new PointF(1, 1));
                var size = gfont.MeasureString(c.ToString(), GdiFont);
                //
                //    , clgf.type.Imaging.PixelFormat.Format32bppArgb);
                size.Width *= 72.0f / 96;
                int width = (int)size.Width;
                if (width < size.Width) width++;

                int height = maxcharheight;
                width += 2;
                height += 3;
                var bdata = gbmp.LockBits(new Rectangle(3, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                unsafe
                {
                    cdata = new UInt32[width * height];
                    fixed (UInt32* outp = cdata)
                    {
                        UInt32* src = (UInt32*)bdata.Scan0.ToPointer();
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {

                                outp[y * width + x] = src[y * bdata.Stride / 4 + x];
                            }
                        }
                    }
                }
                gbmp.UnlockBits(bdata);
                return new Size(width, height);
                //gbmp.Save("c:\\abc\\1.png");
                //return size;

            }
            catch
            {
                cdata = null;
                return new Size(0, 0);
            }
        }

    }
}
