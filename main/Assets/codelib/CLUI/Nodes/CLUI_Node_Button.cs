using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace CLUI
{
    public class CLUI_Node_Button:CLUI_Node
    {
        public CLUI_Node_Button(string name)
            : base(name)
        {
            events = new Dictionary<string, string>();
            events["click"] = null;
        }
        string _text = "button";

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

            if(GUI.Button(rectWorld, text))
            {
                DoEvent("click");
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
        }
    }
}
