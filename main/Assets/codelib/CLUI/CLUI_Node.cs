using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;
using UnityEngine;

namespace CLUI
{
    //将事件平面化
    public interface CLUI_EventCenter
    {
        void DoEvent(string __actionname,CLUI_Node node,string eventname);
    }
    public struct CLUI_Border
    {
        public float x;
        public float y;
        public float x2;
        public float y2;
        public CLUI_Border(float left, float top, float right, float bottom)
        {
            x = left;
            y = top;
            x2 = right;
            y2 = bottom;
        }
        public static CLUI_Border ScaleLeftTop
        {
            get
            {
                return new CLUI_Border(0, 0, 0, 0);
            }
        }
        public static CLUI_Border ScaleFill
        {
            get
            {
                return new CLUI_Border(0, 0, 1.0f, 1.0f);
            }
        }
        public static CLUI_Border ScaleFixBorder(float side = 0.1f)
        {
            return new CLUI_Border(0 + side, 0 + side, 1.0f - side, 1.0f - side);
        }
        public static CLUI_Border PosZero
        {
            get
            {
                return new CLUI_Border(0, 0, 0, 0);
            }
        }
        public static Rect Calc(Rect world, CLUI_Border scale, CLUI_Border pos)
        {
            return Rect.MinMaxRect(
                world.xMin + world.width * scale.x + pos.x,
                world.yMin + world.height * scale.y + pos.y,
                world.xMin + world.width * scale.x2 + pos.x2,
                world.yMin + world.height * scale.y2 + pos.y2);
        }
        public static Rect CalcFromFullScreen(CLUI_Border scale, CLUI_Border pos)
        {
            return Rect.MinMaxRect(
               Screen.width * scale.x + pos.x,
               Screen.height * scale.y + pos.y,
               Screen.width * scale.x2 + pos.x2,
               Screen.height * scale.y2 + pos.y2
               );
        }
    }

    public abstract class CLUI_Node
    {
        public CLUI_Node(string name)
        {
            this.name = name;
        }

        public static CLUI_EventCenter eventCenter
        {
            get;
            private set;
        }
        public static void SetEventCenter(CLUI_EventCenter center)
        {
            eventCenter =center;
        }

        protected Dictionary<string, string> events;
        public void RegAction(string eventname,string actionname)
        {
            if (events.ContainsKey(eventname))
            {
                events[eventname] = actionname;
            }
            else
            {
                throw new Exception("不能注册这个事件");
            }
        }
        protected void DoEvent(string eventname)
        {
            if (events != null && events.ContainsKey(eventname) && events[eventname] != null)
                eventCenter.DoEvent(events[eventname], this, eventname);
        }
        public CLUI_Node parent
        {
            get;
            private set;
        }
        public string name
        {
            get;
            private set;
        }
        public CLUI_Node Find(string path)
        {
            int index = path.IndexOf('/');
            if (index<0)
            {
                foreach (var p in _subnodes)
                {
                    if (p.name == path)
                    {
                        return p;
                    }
                }
            }
            else if(index==0)
            {
                return Find(path.Substring(1));
            }
            else
            {
                string next = path.Substring(index + 1);
                return Find(path.Substring(0, index)).Find(next);
            }
            return null;
        }
        public void DoGUI()
        {
            OnGUI();
            if (_subnodes == null) return;
            foreach (var c in _subnodes)
            {
                c.DoGUI();
            }
        }
        public void DoUpdate(bool force = false)
        {
            if (needUpdate) force = true;
            if (force)
            {
                OnUpdate();

            }


            if (_subnodes != null)
            {
                foreach (var c in _subnodes)
                {
                    c.DoUpdate(force);
                }
            }

        }
        protected virtual void OnGUI()
        {

        }
        protected virtual void OnUpdate()
        {
            if (parent == null)
            {
                rectWorld = CLUI_Border.CalcFromFullScreen(_rectScale, _rectPos);
            }
            else
            {
                rectWorld = CLUI_Border.Calc(parent.rectWorld, _rectScale, _rectPos);
            }
        }
        protected List<CLUI_Node> _subnodes;
        public ReadOnlyCollection<CLUI_Node> subnodes
        {
            get
            {
                return _subnodes == null ? null : _subnodes.AsReadOnly();
            }
        }
        public void ClearNode()
        {
            _subnodes.Clear();
        }
        public void RemoveNode(CLUI_Node node)
        {
            if (node.parent != this)
                return;
            node.SetParent(this);
            _subnodes.Remove(node);
        }
        void SetParent(CLUI_Node node)
        {
            this.parent = node;
        }
        public void AddNode(CLUI_Node node)
        {
            if (node.parent == this)
                return;
            if (node.parent != null)
            {
                node.parent.RemoveNode(node);
            }
            node.SetParent(this);
            if (_subnodes == null) _subnodes = new List<CLUI_Node>();
            _subnodes.Add(node);
        }
        public Rect rectWorld
        {
            get;
            private set;
        }
        protected bool needUpdate = true;
        protected CLUI_Border _rectPos = CLUI_Border.PosZero;
        protected CLUI_Border _rectScale = CLUI_Border.ScaleFixBorder();
        public CLUI_Border rectPosition
        {
            get
            {
                return _rectPos;
            }
            set
            {
                _rectPos = value;
                needUpdate = true;
            }
        }
        public CLUI_Border rectScale
        {
            get
            {
                return _rectScale;
            }
            set
            {
                _rectScale = value;
                needUpdate = true;
            }
        }
    }

    public class CLUI_Node_Empty : CLUI_Node
    {
        public CLUI_Node_Empty(string name)
            : base(name)
        {

        }

    }
    public class CLUI_Node_Box : CLUI_Node
    {
        public CLUI_Node_Box(string name)
            : base(name)
        {

        }
        protected override void OnGUI()
        {
            GUI.Box(rectWorld,"");
            //Debug.LogWarning(rectWorld);
        }
    }
}
