using Android.Graphics;
using clgf.type;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace clgf.text
{
    public class Font //不同平台需要不同实现
    {
        //Bitmap bitmap;
        Paint paint;
        //Canvas canvas;
        int fontsize = 0;
        public Font(string fontname, int fontsize)
        {
            this.fontsize=fontsize;
            //if (bitmap == null)
            {
                //float height = (fontsize * 1.25f);
                //int ih = (int)height;
                //if ((height - ih) > 0) ih++;

                //bitmap = Bitmap.CreateBitmap(fontsize*2, ih, Bitmap.Config.Argb8888);
                //canvas = new Canvas(bitmap);
                paint = new Paint();
                paint.Color = Android.Graphics.Color.White;
                //paint.SetTypeface(Typeface.
                paint.TextSize = fontsize;
                //canvas.Save();
            }
        }

        public Size GetCharData(char c, out  UInt32[] cdata)
        {
            float height=(fontsize * 1.25f);
            int ih = (int)height;
            if ((height - ih) > 0) ih++;
            float width = paint.MeasureText(c.ToString());
            int iw = (int)width;
            if ((width - iw) > 0) iw++;

            //canvas.Restore();
            Bitmap bitmap = Bitmap.CreateBitmap(iw, ih, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawColor(Android.Graphics.Color.Transparent);
            canvas.DrawText(c.ToString(), 0, fontsize, paint);



            cdata = new UInt32[iw * ih];
            int[] data = new int[iw * ih];
            bitmap.GetPixels(data, 0, iw, 0, 0, iw, ih);
            for (int i = 0; i < data.Length; i++)
            {
                cdata[i] = (uint)data[i];
            }
            return new Size(iw, ih);
        }

        public Size GetCharDataDC(char c, out  UInt32[] cdata, Microsoft.Xna.Framework.Color fill, Microsoft.Xna.Framework.Color border)
        {
            float height = (fontsize * 1.25f);
            int ih = (int)height;
            if ((height - ih) > 0) ih++;
            float width = paint.MeasureText(c.ToString());
            int iw = (int)width;
            if ((width - iw) > 0) iw++;
            iw += 2;

            //canvas.Restore();
            Bitmap bitmap = Bitmap.CreateBitmap(iw, ih, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawColor(Android.Graphics.Color.Transparent);
            var pb = new Paint(paint);
            pb.Color = new Android.Graphics.Color(border.R, border.G, border.B, border.A);
            canvas.DrawText(c.ToString(), 0, fontsize-1, pb);
            canvas.DrawText(c.ToString(), 1, fontsize-1, pb);
            canvas.DrawText(c.ToString(), 2, fontsize-1, pb);
            canvas.DrawText(c.ToString(), 0, fontsize, pb);
            //canvas.DrawText(c.ToString(), 1, fontsize, pb);
            canvas.DrawText(c.ToString(), 2, fontsize, pb);
            canvas.DrawText(c.ToString(), 0, fontsize + 1, pb);
            canvas.DrawText(c.ToString(), 1, fontsize + 1, pb);
            canvas.DrawText(c.ToString(), 2, fontsize + 1, pb);
            var pf = new Paint(paint);
            pf.Color = new Android.Graphics.Color(fill.R, fill.G, fill.B, fill.A);
            canvas.DrawText(c.ToString(), 1, fontsize, pf);


            cdata = new UInt32[iw * ih];
            int[] data = new int[iw * ih];
            bitmap.GetPixels(data, 0, iw, 0, 0, iw, ih);
            for (int i = 0; i < data.Length; i++)
            {
                cdata[i] = (uint)data[i];
            }
            return new Size(iw, ih);
        }
    }
}
