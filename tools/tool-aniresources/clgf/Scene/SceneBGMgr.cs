using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clgf.Anim;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace clgf.Scene
{

    public class BGBlock //最基本的块
    {
        public BGBlock(SpriteAniController _ani)
        {
            ani = _ani;
        }
        public SpriteAniController ani
        {
            get;
            private set;
        }
        public float startstep = 0;
        public Rectangle rect;
        public void Draw(SpriteBatch sb,Vector2 scenepos,Vector2 scenescale)
        {
            var f = ani.GetFrame();
            scenepos.X+=(f.size.X / 2+rect.X)*scenescale.X;
            scenepos.Y += rect.Y*scenescale.Y;
            f.Draw(sb, scenepos, scenescale, Color.White);
        }
    }
    public interface IBGLayer//一层卷轴 
    {
        BGBlock GetNext();
        float scrollspeed 
        {
            get;
            set;
        }
    }
    public class BGLayerSingleAni:IBGLayer
    {
        BGBlock block = null;
        public BGLayerSingleAni(SpriteAniController ani)
        {
            block=new BGBlock(ani);
            scrollspeed = 1.0f;
        }
        public BGBlock GetNext()
        {
            return new BGBlock(block.ani);
        }
        public float scrollspeed 
        {
            get;
            set;
        }
    }

    public class BGArea:SceneObj //一个区块 负责自己的多层卷轴，可以单独使用一个区块工作
    {
        public override SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        public Vector2 size;
        public BGArea( Vector2 _size)
        {
            size = _size;
        }
        class LayerStatus
        {
            public IBGLayer layer;
            public List<BGBlock> blocks = new List<BGBlock>();
            public void Update(BGArea area,float delta)
            {
                float leftvalue = (area.pos.X + area.stepX) * layer.scrollspeed;
                float rightvalue = leftvalue + area.size.X;
                float scrolldec = (layer.scrollspeed - 1);
                float newspeed = 1 + scrolldec /4;
                if (newspeed < 0) newspeed = 0;
                float Y = area.scenepos.Y + area.drawoffect.Y + (-area.pos.Y + area.stepY) * newspeed;
                //删除
                List<BGBlock> del=new List<BGBlock>();
                foreach (var b in blocks)
                {
                    if (b.startstep + b.rect.Width < leftvalue-350)
                        del.Add(b);
                }
                foreach (var b in del)
                {
                    blocks.Remove(b);
                }
                //移动
                List<SpriteAniController> anis = new List<SpriteAniController>();
                float maxright = leftvalue;
                foreach (var b in blocks)
                {
                    b.rect.Width = b.ani.GetFrame().size.X;
                    b.rect.Height = (int)area.size.Y;
                    b.rect.X = (int)(area.scenepos.X + area.drawoffect.X+(b.startstep - leftvalue));
                    b.rect.Y = (int)Y;
                    maxright = Math.Max(maxright, b.startstep+b.rect.Width);
                    if (anis.Contains(b.ani) == false)
                    {
                        anis.Add(b.ani);
                    }
                }
                foreach (var a in anis)
                {
                    a.AdvTime(delta);
                }
                //如果不够，再加上
                if (maxright < rightvalue)
                {
                    var b = layer.GetNext();
                    b.startstep = maxright;
                    blocks.Add(b);
                    b.rect.Width = b.ani.GetFrame().size.X;
                    b.rect.Height = (int)area.size.Y;
                    b.rect.X = (int)(area.scenepos.X + area.drawoffect .X+ (b.startstep - leftvalue));
                    b.rect.Y = (int)Y;
                }
            }
        }
        Dictionary<int,LayerStatus> layers = new Dictionary<int,LayerStatus>();
        public float stepX
        {
            get;
            set;
        }
        public float stepY
        {
            get;
            set;
        }
        public void SetLayer(int i, IBGLayer lay)
        {
            layers[i] = new LayerStatus();
            layers[i].layer=lay;
        }
        protected override void OnUpdate(float delta)
        {
            foreach (var l in layers)
            {
                //float leftvalue = this.scenepos.X + step;
                l.Value.Update(this,delta);
            }
        }
        Point drawoffect = new Point(0, 0);
        protected override void OnDraw(SpriteBatch sb,ref Point offect)
        {
            drawoffect = offect;
            foreach (var l in layers)
            {
                foreach (var b in l.Value.blocks)
                {
                    Vector2 v = this.scenepos;
                    v.Y += this.size.Y*this.scenescale.Y +offect.Y;
                    v.X += offect.X;
                    b.Draw(sb,v,this.scenescale);
                    
                }
            }
        }
    }
}
