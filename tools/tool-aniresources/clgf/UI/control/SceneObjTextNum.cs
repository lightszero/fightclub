using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clgf.text;
using Microsoft.Xna.Framework;
using Size = clgf.type.Size;
using clgf.UI;
namespace clgf.Scene
{
    public class SceneObjTextNum:UIElement
    {
        public override SceneObj OnCopy()
        {
            SceneObjTextNum e = new SceneObjTextNum();
            //this
            e.color = this.color;
            e.drawfont1 = this.drawfont1;
            e.FixLength=this.FixLength;//将整数固定为几位

            e.colorZero = this.colorZero;
            e.valign = this.valign;//水平对齐
            e.halign = this.halign;//垂直对齐
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
        public void Init(IFont font)//专门显示整数的
        {
            drawfont1 = font;

        }
        public int FixLength=6;//将整数固定为几位

        public Color color = Color.White;
        public Color colorZero = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        IFont drawfont1;
        public uint Number
        {
            get
            {
                return uint.Parse(text);
            }
            set
            {
                text = value.ToString("D" + FixLength);
            }
        }
        string text="";
        public int valign = 0;//水平对齐
        public int halign = 0;//垂直对齐
        private int width;
        private int height;
        private int widthsplit;
        private int possplit;
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            if (needDraw == false) return;
            width = 0;
            height = 0;
            possplit = 0;
            widthsplit = 0;
            foreach (var c in text)
            {

                Size? size = null;

                size = drawfont1.GetCharSize(c);

                if (size != null)
                {

                    height = Math.Max(height, size.Value.Height);
                    if (c == '0' && width == widthsplit)
                    {
                        widthsplit += size.Value.Width;
                        possplit++;
                    }
                    width += size.Value.Width;
                }

            }
            if (valign == 0)
            {
                align.Y = 0;
            }
            if (valign == 1)
            {
                align.Y = (Size.Y - height) / 2;
            }
            if (valign == 2)
            {
                align.Y = (Size.Y - height);
            }
            if (halign == 0)
            {
                align.X = 0;
            }
            if (halign == 1)
            {
                align.X = (Size.X - width) / 2;
            }
            if (halign == 2)
            {
                align.X = (Size.X - width);
            }
        
        }

        Vector2 align = new Vector2(0, 0);
        List<Quadbuf.QuadBuf> bufs = new List<Quadbuf.QuadBuf>();
        public bool needDraw = false;
        public override bool needUpdate
        {
            get
            {
                return base.needUpdate;
            }
            set
            {
                needDraw = true;
                base.needUpdate = value;
            }
        }
        protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb,ref Point offect)
        {
            if (needDraw)
            {

                bufs.Clear();
                Vector2 _pos = scenepos - (Orient) * scenescale + align * scenescale;
                _pos.X += offect.X;
                _pos.Y += offect.Y;
                Vector2 _scale = scenescale;
                Vector2 _pos2 = _pos;
                _pos2.X += widthsplit * scenescale.X;
                if (drawfont1 != null)
                {
                    drawfont1.DrawStringToBuf(bufs, text.Substring(0, possplit), _pos, _scale, colorZero);
                    drawfont1.DrawStringToBuf(bufs, text.Substring(possplit), _pos2, _scale, color);
                }
            }
            Quadbuf.QuadBuf.Draw(sb, bufs);
        }
    }
}
