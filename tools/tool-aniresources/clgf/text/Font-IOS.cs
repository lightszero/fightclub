#if IOS
using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

using MonoTouch.CoreGraphics;
using Microsoft.Xna.Framework;
using clgf.type;

namespace clgf.text
{
	public class Font//GDI+的实现，不同平台需要不同实现
	{
		//static Graphics gfont = null;
		//static Bitmap gbmp = null;
		//System.Drawing.Font GdiFont = null;
		//static CGBitmapContext gbmp; 
		//static byte[] gdata;
		UIFont font =null;//UIFont.SystemFontOfSize(20);
		public int maxcharheight
		{
			get;
			private set;
		}
		public  Font(string fontname,int fontsize)
		{
			font =UIFont.FromName(fontname,fontsize);
			if(font==null)font=UIFont.SystemFontOfSize(fontsize);
			maxcharheight =(int)font.LineHeight;
			//if (gbmp == null)
			{
//				gdata = new byte[128 * 128 * 4];
//				var colorSpace = CGColorSpace.CreateDeviceRGB();
//				gbmp = new CGBitmapContext(gdata, 128, 128,
//				                                        8, 128 * 4, colorSpace, CGBitmapFlags.PremultipliedLast);
				//gbmp =new Bitmap(128,128);
				//gfont = Graphics.FromImage(gbmp);
				//gfont.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
				//gfont.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			}
			
			
			
			//GdiFont = new System.Drawing.Font(fontname, fontsize,GraphicsUnit.Pixel);
			//maxcharheight = GdiFont.Height;
			
			//设置文本输出质量
			//g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			//g.SmoothingMode = SmoothingMode.AntiAlias;
			//Font newFont = new Font("Times New Roman", 48);
			
		}
		
		public Size GetCharData(char c,out  UInt32[] cdata)
		{
			NSString str =new NSString(c.ToString());
			var size=str.StringSize(font);
			UIGraphics.BeginImageContextWithOptions(size,false,1.0f);
			UIGraphics.GetCurrentContext().SetFillColor(1.0f,1.0f,1.0f,1.0f);
			str.DrawString(new System.Drawing.PointF(0,0),font);
			UIImage img =UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			//
			int width=(int)size.Width;
			int height=(int)size.Height;
			cdata=new UInt32[width*height];
			var gdata=new byte[width*height*4];
			var colorSpace = CGColorSpace.CreateDeviceRGB();
			var gbmp = new CGBitmapContext(gdata, width, height,
			                           8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast);
			//gbmp.ClearRect(new RectangleF(0,0,width,height));
			gbmp.DrawImage(new System.Drawing.RectangleF(0,0,width,height),img.CGImage);
			gbmp.Dispose();
			colorSpace.Dispose();
			unsafe
			{
				fixed(byte* srcb = gdata)
					fixed (UInt32* dest=cdata)
				{
					UInt32* src=(UInt32*)srcb;
					for(int y=0;y<height;y++)
					{
						for(int x=0;x<width;x++)
						{
							dest[y*width+x]=src[y*width+x];
						}
					}
				}
			}

			return new Size(width,height);
		}

		public Size GetCharDataDC(char c, out  UInt32[] cdata,Color fill,Color border)
		{
			NSString str =new NSString(c.ToString());
			var size=str.StringSize(font);
			size.Width+=4;
			size.Height+=4;
			UIGraphics.BeginImageContextWithOptions(size,false,1.0f);
			UIGraphics.GetCurrentContext().SetFillColor((float)border.R/255.0f,(float)border.G/255.0f,(float)border.B/255.0f,(float)border.A/255.0f);
			str.DrawString(new System.Drawing.PointF(0,0),font);
			str.DrawString(new System.Drawing.PointF(0,1),font);
			str.DrawString(new System.Drawing.PointF(0,2),font);
			str.DrawString(new System.Drawing.PointF(0,3),font);
			str.DrawString(new System.Drawing.PointF(0,4),font);
			str.DrawString(new System.Drawing.PointF(1,0),font);
			str.DrawString(new System.Drawing.PointF(1,1),font);
			str.DrawString(new System.Drawing.PointF(1,2),font);
			str.DrawString(new System.Drawing.PointF(1,3),font);
			str.DrawString(new System.Drawing.PointF(1,4),font);
			str.DrawString(new System.Drawing.PointF(3,0),font);
			str.DrawString(new System.Drawing.PointF(3,1),font);
			str.DrawString(new System.Drawing.PointF(3,2),font);
			str.DrawString(new System.Drawing.PointF(3,3),font);
			str.DrawString(new System.Drawing.PointF(3,4),font);
			str.DrawString(new System.Drawing.PointF(4,0),font);
			str.DrawString(new System.Drawing.PointF(4,1),font);
			str.DrawString(new System.Drawing.PointF(4,2),font);
			str.DrawString(new System.Drawing.PointF(4,3),font);
			str.DrawString(new System.Drawing.PointF(4,4),font);
			str.DrawString(new System.Drawing.PointF(2,0),font);
			str.DrawString(new System.Drawing.PointF(2,1),font);
			//str.DrawString(new System.Drawing.PointF(2,2),font);
			str.DrawString(new System.Drawing.PointF(2,3),font);
			str.DrawString(new System.Drawing.PointF(2,4),font);
			UIGraphics.GetCurrentContext().SetFillColor((float)fill.R/255.0f,(float)fill.G/255.0f,(float)fill.B/255.0f,(float)fill.A/255.0f);
			str.DrawString(new System.Drawing.PointF(2,2),font);
			UIImage img =UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			
			//
			int width=(int)size.Width;
			int height=(int)size.Height;
			cdata=new UInt32[width*height];
			var gdata = new byte[width * height * 4];
			var colorSpace = CGColorSpace.CreateDeviceRGB();
			var gbmp = new CGBitmapContext(gdata, width, height,
			                           8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast);
			//gbmp.ClearRect(new RectangleF(0,0,width,height));
			gbmp.DrawImage(new System.Drawing.RectangleF(0,0,width,height),img.CGImage);
			gbmp.Dispose();
			colorSpace.Dispose();
			unsafe
			{
				fixed(byte* srcb = gdata)
					fixed (UInt32* dest=cdata)
				{
					UInt32* src=(UInt32*)srcb;
					for(int y=0;y<height;y++)
					{
						for(int x=0;x<width;x++)
						{
							dest[y*width+x]=src[y*width+x];
						}
					}
				}
			}
			
			return new Size(width,height);
		}
		
	}
}
#endif
