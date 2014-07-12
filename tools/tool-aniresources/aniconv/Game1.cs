#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using clgf.Anim;
using clgf.Texture;
#endregion

namespace aniconv
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public TextureMgr tmgr = new TextureMgr();
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            ui fedit = new ui(this);

            fedit.OnGraphReszie += (x, y) =>
            {
                this.graphics.PreferredBackBufferWidth = x;
                this.graphics.PreferredBackBufferHeight = y;

                this.graphics.ApplyChanges();

                this.GraphicsDevice.Viewport = new Viewport(0, 0, x, y);
            };
            tmgr.Init(GraphicsDevice, Content);

            System.Windows.Forms.Form form = System.Windows.Forms.Form.FromHandle(this.Window.Handle) as System.Windows.Forms.Form;
            fedit.Init(form);
            fedit.Show();


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        Vector2? mousepos = null;
        protected override void Update(GameTime gameTime)
        {
            MouseState m = Mouse.GetState();

            if (m.LeftButton == ButtonState.Pressed)
            {
                if (mousepos == null)
                {
                    mousepos = new Vector2(m.X, m.Y);
                }
                else
                {
                    offect.X += m.X - mousepos.Value.X;
                    offect.Y += m.Y - mousepos.Value.Y;
                    mousepos = new Vector2(m.X, m.Y);
                }
            }
            else
            {
                mousepos = null;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            foreach (var f in dummys)
            {
                f.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                if (f.timer > 2.0f)
                {
                    dummys.Remove(f);
                    return;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public bool drawShadow = true;
        RenderTarget2D shadowrt;
        void DrawShadow(DrawFrame frame,Vector2 pos)
        {
            Vector2 _pos = new Vector2(pos.X, pos.Y);

            Vector2 scale = new Vector2(1, 1);
            scale.Y *= -0.25f;// 0.25f;
            scale.X *= 0.25f;
            _pos.X *= 0.25f;
            frame.DrawWithTag(shadowtags, spriteBatch, _pos, scale, new Color(0, 0, 0, 1.0f));
        }
        public List<string> shadowtags = new List<string>();
        public Dictionary<string, Color> boundcolor = new Dictionary<string, Color>();
        Texture2D white;
        public class FlyDummy
        {
            Dummy dum;
            Vector2 pos;
            Point size;
            public FlyDummy(Dummy d,Vector2 pos,Point size)
            {
                dum = d;
                this.pos = pos;
                this.size = size;
            }
            public void Update(float delta)
            {
                timer += delta;

                Matrix rt =Matrix.CreateRotationZ(dum.rotate);
                Vector3 foward = new Vector3(1, 0, 0);
                foward = Vector3.TransformNormal(foward, rt);

                pos.X += foward.X * 100 * delta;
                pos.Y += foward.Y * 100 * delta;
            }
            public float timer = 0;
            public void Draw(SpriteBatch sb)
            {
                DrawFrame.DrawElement(size, dum.elem, sb, pos, new Vector2(1, 1), Color.White);
            }
        }
        public List<FlyDummy> dummys = new List<FlyDummy>();

        void CreateDummy(Dummy d, Vector2 pos, Point size)
        {
            if (dummyshow.ContainsKey(d.name))
            {
                if (dummyshow[d.name])
                {
                    dummys.Add(new FlyDummy(d, pos, size));
                    if (dummys.Count > 20)
                    {
                        dummys.RemoveAt(0);
                    }
                }

            }
            else
            {
                dummyshow[d.name] = false;
            }
            //if (d.name.ToLower() == "throw")


        }
        protected override void Draw(GameTime gameTime)
        {
            if (white == null)
            {
                white = new Texture2D(GraphicsDevice, 1, 1);
                UInt32[] c = new UInt32[1];
                c[0] = UInt32.MaxValue;
                white.SetData(c);
            }
           
            if (drawShadow)
            {
                if (shadowrt == null)
                {
                    shadowrt = new RenderTarget2D(GraphicsDevice, 256, 1024, false, SurfaceFormat.Color, DepthFormat.None);
                }
                GraphicsDevice.SetRenderTarget(shadowrt);
                {
                    GraphicsDevice.Clear(new Color(0, 0, 0, 0f));
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    foreach (var ani in anis)
                    {
                        Vector2 vdest = ani.Key + offect;
                        ani.Value.AdvTime((float)gameTime.ElapsedGameTime.TotalSeconds);
                        //ani.play = true;
                        DrawShadow(ani.Value.GetFrame(), vdest);
                      
                    }
                    spriteBatch.End();
                }
               

            }
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (drawShadow)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                spriteBatch.Draw(shadowrt, new Rectangle(0, 0, 1024, 1024), new Color(1.0f, 1.0f, 1.0f, 0.5f));//shadow
                spriteBatch.End();
            }
            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,SamplerState.LinearClamp,DepthStencilState.None,RasterizerState.CullNone);
            int y = 0;
            //if (seeds != null)
            //{
            //    foreach (var seed in seeds)
            //    {
            //        if (seed.Value.__block == null)
            //        {
            //            seed.Value.__block = tmgr.GetTexture("chars/"+seed.Value.texname);
            //            seed.Value.size.X = seed.Value.__block.uv.Width*seed.Value.__block.parent.texture.Width;
            //            seed.Value.size.Y = seed.Value.__block.uv.Height * seed.Value.__block.parent.texture.Height;
            //        }
            //        if (seed.Value.__block != null)
            //        {
            //            seed.Value.__DebugDraw(spriteBatch,new Point(50, y));
            //            y += 50;
            //        }
            //    }
            //}
            foreach (var ani in anis)
            {
                //ani.Value.AdvTime((float)gameTime.ElapsedGameTime.TotalSeconds);
                //ani.play = true;
                Vector2 vdest = ani.Key + offect;
                ani.Value.GetFrame().Draw(spriteBatch, vdest, new Vector2(1.0f, 1.0f), Color.White);
                var f = ani.Value.GetFrame();
                foreach (var d in f.dummys)
                {
                    CreateDummy(d,vdest,f.size);
                }

                foreach (var cb in boundcolor)
                {
                    var b = ani.Value.GetFrame().GetBounds(vdest, new Vector2(1.0f, 1.0f), cb.Key);
                    if (b != null)
                    {
                        Rectangle dest = new Rectangle((int)b.Value.X, (int)b.Value.Y, (int)b.Value.Width, (int)b.Value.Height);
                        spriteBatch.Draw(white, dest, cb.Value);

                    }
                }

                foreach (var s in f.sounds)
                {
                    Console.WriteLine("PlaySound:" + s);
                }
            }

            foreach (var f in dummys)
            {
                f.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
 
        }
        //Dictionary<string, Seed> seeds;
        //public void DrawSeed(Dictionary<string, Seed> _seeds)
        //{
        //    seeds = _seeds;
        //}
        Vector2 offect = new Vector2(0, 0);
        Dictionary<Vector2, SpriteAniController> anis = new Dictionary<Vector2, SpriteAniController>();
        public Dictionary<string, bool> dummyshow = new Dictionary<string, bool>();
        public void ClearSprite()
        {
            anis.Clear();
            offect = new Vector2(0, 0);
            dummyshow.Clear();
        }
        public SpriteAniController onlycontroller
        {
            get
            {
                if (anis.Count == 1)
                {
                    foreach (var i in anis.Values)
                    {
                        return i;
                    }
                    return null;
                }
                else
                    return null;
            }
        }
        public void DrawSprite(SpriteAni ani,Vector2 pos)
        {
            SpriteAniController con = new SpriteAniController(ani.GetElement(null));
            con.bplay = true;
            
            anis.Add(pos, con);
            //anis.Add(ani);
        }
    }
}
