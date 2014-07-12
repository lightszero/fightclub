using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using clgf.Anim;
using Microsoft.Xna.Framework.Graphics;
using Size = clgf.type.Size;
using clgf.UI;

namespace clgf.Scene
{
    //横向卷轴场景
    public class Scene:IDisposable
    {
        public Scene()
        {
            background = new SceneObj();
            sceneroot = new SceneObj();
            uilayer = new SceneObj_UIContainer(this);
            backgroundColor = Color.AliceBlue;
        }
        Rectangle _ViewPort;
        public Rectangle ViewPort
        {

            get
            {
                return _ViewPort;
            }
        }
        public Size destDrawArea
        {
            private set;
            get;

        }
        void UpdateDrawInfo()
        {
            sceneroot.pos = new Vector2(0, 0);

            sceneroot.scale = new Vector2((float)this.destDrawArea.Width / (float)this.ViewPort.Width
                , (float)this.destDrawArea.Height / (float)this.ViewPort.Height);

            //sceneroot.pos *= sceneroot.scale;

            background.pos = sceneroot.pos;
            background.scale = sceneroot.scale;

            uilayer.pos = sceneroot.pos;
            uilayer.scale = sceneroot.scale;
        }
        //SceneBGMgr bgmgr = new SceneBGMgr();//背景管理器
        //public HScrollScene(SceneBGMgr bgcreator)
        //{
        //    bgmgr = bgcreator;
        //}
        /// <summary>
        /// 初始化场景
        /// </summary>
        /// <param name="destdrawarea">目标绘制区域，全屏</param>
        /// <param name="ViewPort">实际显示区域</param>
        public void Init(Size _destDrawArea, Rectangle _ViewPort)
        {
            destDrawArea = _destDrawArea;
            this._ViewPort = _ViewPort;
            UpdateDrawInfo();
        }
        public void Resize(Size size,bool fixviewport=false)
        {
            destDrawArea = size;
            if (fixviewport)
            {
                _ViewPort.Width = (int)((float)_ViewPort.Height / (float)size.Height * (float)size.Width);
            }
            UpdateDrawInfo();
            NeedUpdateAll(this.sceneroot);
            NeedUpdateAll(this.uilayer);
        }
        public void NeedUpdateAll(SceneObj sobj)
        {
            sobj.needUpdate = true;
            if(sobj.Children!=null)
            foreach (var o in sobj.Children)
            {
                NeedUpdateAll(o);
            }
        }
        public void SetCamPos(int x, int y)
        {
            _ViewPort.X = x;
            _ViewPort.Y = y;
        }
        public void SetViewPortSize(int x, int y)
        {
            _ViewPort.Width = x;
            _ViewPort.Height = y;
        }
        public void Dispose()
        {
            background = null;
            sceneroot = null;
            uilayer = null;
            if(shadowrt!=null)
                shadowrt.Dispose();
            shadowrt = null;
        }
        public SceneObj background
        {
            get;
            private set;
        }
        public SceneObj sceneroot
        {
            get;
            private set;
        }
        public SceneObj_UIContainer uilayer
        {
            get;
            private set;
        }
        //绘制场景

        public RenderTarget2D shadowrt = null;
        public Color backgroundColor
        {
            get;
             set;
        }
        public bool drawShadow=false;
        Point drawOffect;
        Point uiOffect = new Point(0, 0);
        public void Draw(GraphicsDevice gdevice, SpriteBatch sb,RenderTarget2D scenert)
        {
            if (drawShadow)
            {
                if (shadowrt == null || shadowrt.Width != destDrawArea.Width / 4 || shadowrt.Height != destDrawArea.Height)
                {
                    shadowrt = new RenderTarget2D(gdevice, destDrawArea.Width / 4, destDrawArea.Height, false, SurfaceFormat.Color, DepthFormat.None);
                }
                gdevice.SetRenderTarget(shadowrt);
                {
                    gdevice.Clear(new Color(0, 0, 0, 0f));
					sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    sceneroot.DrawShadow(sb, ref drawOffect);
                    sb.End();
                }
            }
            gdevice.SetRenderTarget(scenert);
            gdevice.Clear(backgroundColor);
			sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            background.Draw(sb, ref drawOffect);
            sb.End();

			sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            if (drawShadow)
            {
                sb.Draw(shadowrt, new Rectangle(0, 0, destDrawArea.Width, destDrawArea.Height), new Color(1.0f, 1.0f, 1.0f, 0.5f));//shadow
              }
            sceneroot.Draw(sb, ref drawOffect);
            sb.End();

			sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);

            uilayer.Draw(sb, ref uiOffect);
            sb.End();
           
        }

        public void Update(float delta)
        {
           

            drawOffect = new Point((int)(-this.ViewPort.Left * sceneroot.scale.X),(int)( -this.ViewPort.Top * sceneroot.scale.Y));


            

            background.Update(delta);
            sceneroot.Update(delta);



            uilayer.Update(delta);
            uilayer.UpdateTouch();
        }
    }
}
