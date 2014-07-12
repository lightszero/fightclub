using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using clgf.Anim;
namespace clgf.Scene
{
    public class SceneObjChar : SceneObj
    {
        public override SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        public virtual Sprite sprite
        {
            get;
            set;
        }
       
        public SpriteAniController controller
        {
            get;
            private set;
        }
        //DrawFrame drawframe = null;
        protected Texture.TextureMgr tmgr;
        public void Init(Sprite _sprite, string initani, Texture.TextureMgr _tmgr)
        {
            tmgr = _tmgr;
            sprite = _sprite;
            var drawani = SpriteAniMgr.CreateAni(sprite,initani,tmgr);
            controller = new SpriteAniController(drawani.GetElement(null));
        }
        public void SetAniState(string ani, string state=null,bool  bimmediate=false, Action whenplay=null)
        {
            controller.State(SpriteAniMgr.CreateAni(sprite, ani, tmgr).GetElement(state), bimmediate, whenplay);
        }
        public void PlayAni(string ani, string elem=null, SpriteAniController.playtag tag = SpriteAniController.playtag.play_immediate,Action whenplay=null)
        {
            controller.Play(SpriteAniMgr.CreateAni(sprite, ani, tmgr).GetElement(elem), tag,whenplay);
        }
        protected override void OnUpdate(float delta)
        {
            if (controller == null) return;
            //drawframe = 
            controller.AdvTime(delta);
        }
        protected override void OnDraw(SpriteBatch sb, ref Point offect)
        {
            if (controller == null) return;
            controller.GetFrame().Draw(sb,new Vector2( scenepos.X+offect.X,scenepos.Y+offect.Y), scenescale, Color.White);
        }

        protected override void OnDrawShadow(SpriteBatch sb, ref Point offect)
        {
            if (shadowline == null) return;
            if (controller == null) return;
            Vector2 _pos = new Vector2(this.pos.X, this.shadowline.Value);
            _pos = CalcScenePos(_pos);
            _pos.X += offect.X;
            _pos.Y += offect.Y;
            Vector2 scale = scenescale;
            scale.Y *= -0.25f;// 0.25f;
            scale.X *= 0.25f;
            float y = (this.shadowline.Value - this.pos.Y) / this.shadowline.Value;
            if (y > 1.0f) y = 1.0f;
            if (y < 0) y = 0;
            scale *= (1.0f - y);
            _pos.X *= 0.25f;
            controller.GetFrame().Draw(sb, _pos, scale, new Color(0, 0, 0, 1.0f));
        }

        public float? shadowline = null;
    }

}
