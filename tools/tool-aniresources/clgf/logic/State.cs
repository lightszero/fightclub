using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clgf.Texture;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Size = clgf.type.Size;
namespace clgf.logic
{
    public class GameApp
    {
        public object apptag
        {
            get;
            private set;
        }
        public TextureMgr tmgr
        {
            get;
            private set;
        }
        public SpriteBatch spritebatch
        {
            get;
            private set;
        }
        public GraphicsDevice GraphicsDevice
        {
            get;
            private set;
        }
        IGameState curstate = null;
        IGameState nextstate = null;
        public void Init(GraphicsDevice gdevice, ContentManager content, IGameState first,object tag)
        {
            apptag = tag;
            GraphicsDevice = gdevice;
            spritebatch = new SpriteBatch(gdevice);
            tmgr = new TextureMgr();
            tmgr.Init(gdevice, content);
            //scene = new Scene.Scene();
            ChangeState(first);
        }
        public void Exit()
        {
            if (curstate != null)
            {
                curstate.OnGameExit();
            }
        }
        public void ChangeState(IGameState state)
        {
            nextstate = state;
        }
             
        void _ChangeState(IGameState state)
        {
            if (curstate != null)
            {
                curstate.OnExit();
            }
            curstate = state;
            if (curstate != null)
            {
                curstate.OnInit(this);
            }
        }
        public void Resize(Size size)
        {
            if (curstate != null)
            {
                curstate.OnResize(size);
            }
        }
        public void Update(float delta)
        {
            GraphicsDevice.Textures[0] = null;
            if (curstate != nextstate)
                _ChangeState(nextstate);
            if (curstate != null)
            {
                curstate.OnUpdate(delta);
            }
        }
        public void Draw()
        {
            if (curstate != null)
            {
                curstate.OnDraw();
            }
        }
        public void Activated()
        {
            if (curstate != null)
            {
                curstate.OnActivated();
            }
        }
        public void DeActivated()
        {
            if (curstate != null)
            {
                curstate.OnDeActivated();
            }
        }
    }
    public interface IGameState
    {
        void OnInit(GameApp app);
        void OnExit();
        void OnGameExit();
        void OnUpdate(float delta);
        void OnResize(Size size);
        void OnDraw();
        void OnActivated();
        void OnDeActivated();
    }
}
