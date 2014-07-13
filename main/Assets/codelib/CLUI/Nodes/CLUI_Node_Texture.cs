using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace CLUI
{
    public class CLUI_Node_Texture:CLUI_Node
    {
        public CLUI_Node_Texture(string name):base(name)
        {

        }

        Texture2D _texture;

        public Texture2D texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;
                this.needUpdate = true;
            }
        }
        Rect _uv = new Rect(0, 0, 1, 1);
        public Rect uv
        {
            get
            {
                return _uv;
            }
            set
            {
                _uv = value;
                this.needUpdate = true;
            }
        }
        public Color color = Color.white;
        protected override void OnGUI()
        {
            var oldcolor = GUI.color;
            GUI.color = this.color;
            GUI.DrawTextureWithTexCoords(rectWorld,_texture,_uv);
            GUI.color = oldcolor;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            
        }
    }
}
