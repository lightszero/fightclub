using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace clgf.UI
{


    public class SceneObj_UIContainer:UIElement
    {
        public override Scene.SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        public Scene.Scene Scene
        {
            get;
            private set;
        }
        internal SceneObj_UIContainer(Scene.Scene scene)
        {
            this.Scene = scene;
        }
        protected override void OnUpdate(float delta)
        {

            if (needResize)
            {
                this.Size.X = Scene.ViewPort.Width;
                this.Size.Y = Scene.ViewPort.Height;
                UpdateLayout();
                needResize = false;
            }
            //UpdateTouch();

        }
        bool needResize = false;
        public override bool needUpdate
        {
            get
            {
                return base.needUpdate;
            }
            set
            {
                base.needUpdate = value;
                needResize = true;
            }
        }
        bool bMouseDown = false;
        public void UpdateTouch()
        {
            if (Children == null) return;
            TouchCollection tc = TouchPanel.GetState();
            List<TouchLocation> listtouch=new List<TouchLocation>();
            int maxid = 0;
            foreach (var t in tc)
            {
                listtouch.Add(t);
                maxid = Math.Max(t.Id, maxid);
            }
            #region simkey
#if WIN32
            var kstate= Microsoft.Xna.Framework.Input.Keyboard.GetState();
            foreach(var k in simkeys)
            {
                if (kstate.IsKeyDown(k.Key))
                {
                    maxid++;
                    Vector2 pos = k.Value;
                    pos.X *= Scene.destDrawArea.Width;
                    pos.Y *= Scene.destDrawArea.Height;
                    TouchLocation t = new TouchLocation(maxid, TouchLocationState.Moved,pos);
                    listtouch.Add(t);
                }
            }
#endif
            #endregion
            #region simmouse
            maxid++;
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (bMouseDown == true)
                {
                    listtouch.Add(new TouchLocation(maxid, TouchLocationState.Moved, new Vector2(ms.X, ms.Y)));
                }
                else
                {
                    listtouch.Add(new TouchLocation(maxid, TouchLocationState.Pressed, new Vector2(ms.X, ms.Y)));
                    bMouseDown = true;
                }
            }
            else
            {
                if (bMouseDown == true)
                {
                    listtouch.Add(new TouchLocation(maxid, TouchLocationState.Released, new Vector2(ms.X, ms.Y)));
                    bMouseDown = false;
                }
            }
            #endregion
            TouchCollection touchs =new TouchCollection(listtouch.ToArray());
            for(int i=Children.Count-1;i>=0;i--)
            {
                UIElement uie = Children[i] as UIElement;
                if(uie==null)continue;

                if(uie.Touch(touchs))return;
            }
        }

       public  Dictionary<Keys, Vector2> simkeys = new Dictionary<Keys, Vector2>();

    }
}
