using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace CLUI
{
    public class CLUI_Node_Label:CLUI_Node
    {
        public CLUI_Node_Label(string name)
            : base(name)
        {

        }
        string _text = "label";

        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                this.needUpdate = true;
            }
        }
        protected override void OnGUI()
        {
            GUI.Label(rectWorld, text);
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            
        }
    }
}
