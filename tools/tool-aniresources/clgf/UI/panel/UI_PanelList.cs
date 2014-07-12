using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using clgf.Scene;

namespace clgf.UI.panel
{
    public class UI_PanelList:UI_Panel
    {
        public override Scene.SceneObj OnCopy()
        {
            throw new Exception("not allow.");
        }
        UIElement baseelement;
        public UI_PanelList(UIElement def)
        {
            Items = new List<object>();
            SelectedIndex = -1;
            onCreateLine = defCreateLine;
            onSelectElement = defSelectElement;
            baseelement = def;
            this.ClipRender = true;
        }
        public List<object> Items
        {
            get;
            private set;
        }
        List<UIElement> UIItem = new List<UIElement>();
        int _SelectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                if(_SelectedIndex>=0)
                    onSelectElement(Items[_SelectedIndex], UIItem[_SelectedIndex], false);
                _SelectedIndex = value;
                if (_SelectedIndex >= 0)
                    onSelectElement(Items[_SelectedIndex], UIItem[_SelectedIndex], true);
            }
        }
        public object SelectedItem
        {
            get
            {
                return Items[SelectedIndex];
            }
            set
            {
                SelectedIndex=Items.IndexOf(value);
            }
        }
        float maxpos = 0;
        public void UpdateData()
        {
            SelectedIndex = -1;
            foreach (var item in UIItem)
            {
                this.RemoveObject(item);
            }
            UIItem.Clear();
            maxpos=0;
            Vector2 pos = new Vector2(0,0);
            foreach (var obj in Items)
            {
                var e=onCreateLine(obj);
                e.pos = pos;
                UIItem.Add(e);
                this.AddObject(e);
                pos.Y += e.Size.Y*e.scale.Y;
            }
            maxpos = pos.Y;
        }

        public delegate UIElement deleCreateLine(object obj);
        public delegate void deleSelectElement(object obj,UIElement uie, bool select);
        public deleCreateLine onCreateLine;
        public deleSelectElement onSelectElement;

        public UIElement defCreateLine(object obj)
        {
            UIElement e =baseelement.Copy() as UIElement;
            e.Text = obj.ToString();
            //e.Size.X = this.Size.X;
            //e.UpdateAlign(3, 0);
            return e;
        }
        public void defSelectElement(object obj, UIElement uie, bool select)
        {
            SceneObjText text = uie as SceneObjText;
            if (text != null)
            {
                text.color = select ? Color.Red : Color.White;
                if (select)
                {
                    text.Size.Y = baseelement.Size.Y * 2.0f;
                    text.Orient.Y = -baseelement.Size.Y / 2;
                }
                else
                {
                    text.Size.Y = baseelement.Size.Y;
                    text.Orient.Y = 0;
                }
                text.needUpdate = true;
            }
            else
            {
                UI_PanelFold pf = uie as UI_PanelFold;
                if (pf != null)
                {
                    if (select)
                    {
                        pf.UnFold();
                    }
                    else
                    {
                        pf.Fold();
                    }
                    pf.needUpdate = true;
                }
            }
        }
        void UpdatePos()
        {
            maxpos = 0;
            Vector2 pos= new Vector2(0,-dragpos);
            foreach (var uie in UIItem)
            {
                uie.pos = pos;
                pos.Y += uie.Size.Y*uie.scale.Y;
                uie.NeedUpdateAll();
                maxpos += uie.Size.Y * uie.scale.Y;
            }
        }
        float dragpos = 0;
        bool bTouch = false;
        Vector2 oldpos;
        bool bdrag = false;
        protected override bool OnTouch(Microsoft.Xna.Framework.Input.Touch.TouchCollection touchs)
        {
            Vector2 lt = this.scenepos - (Orient) * this.scenescale;
            Vector2 rb = lt + this.scenescale * Size;

            List<Vector2> poss=new List<Vector2>();
            //if (bTouch == false)
            bool ntouchup = false;
            Vector2 npos = Vector2.Zero;
            {//检测位置
                foreach (var t in touchs)
                {
                    if (t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed || t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Moved)
                    {
                        if (lt.X <= t.Position.X && t.Position.X <= rb.X
                           && lt.Y <= t.Position.Y && t.Position.Y <= rb.Y)
                         {
                            poss.Add(t.Position);
                        }
                     }
                    if (t.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Released)
                    {
                        if (lt.X <= t.Position.X && t.Position.X <= rb.X
                            && lt.Y <= t.Position.Y && t.Position.Y <= rb.Y)
                        {
                            npos = t.Position;
                            ntouchup = true;
                        }
                    }
                }
            }

            if (poss.Count > 0)
            {
                foreach (var v in poss)
                {
                    npos += v;
                }
                npos /= poss.Count;
            }
            if(bTouch==false&&poss.Count>0)//按下
            {

                oldpos = npos;
                bTouch = true;
                return true;
            }
            else if (bTouch == true && poss.Count == 0)
            {
                if (ntouchup&&!bdrag)
                {
                    if ((npos - oldpos).Length() < 5)
                    {
                        for(int i=0;i<UIItem.Count;i++)
                        {
                            Vector2 slt = UIItem[i].scenepos - (UIItem[i].Orient) * UIItem[i].scenescale;
                            Vector2 srb = slt + UIItem[i].scenescale * UIItem[i].Size;
                            if (slt.Y <= npos.Y && npos.Y <= srb.Y)
                            {
                                SelectedIndex = i;
                                break;
                            }
                        }

                        return true;
                    }
                    //松开
                    oldpos = npos;
                }
                else
                {
                    //离开
                }
                bTouch = false;
                bdrag = false;
            }
            else if(poss.Count>0) //drag
            {
                if ((npos - oldpos).Length() > 3)
                {
                    bdrag = true;

                    dragpos -= npos.Y - oldpos.Y;
                    UpdatePos();
                    oldpos = npos;
                }
                return true;
            }
           
            //处理选中逻辑
            return false;
        }

        float rectimer = 0;
        float morespeed = 0;
        struct rec
        {
            public rec(float pos,float timer)
            {
                this.pos=pos;
                this.timer=timer;
            }
            public float pos;
            public float timer;
        }
        List<rec> speedrec = new List<rec>();
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            if (bTouch)
            {
                morespeed = 0;
            }
            if (bdrag == false&&bTouch==false)
            {
                //惯性
                if(speedrec.Count>1)
                {
                    float rt = speedrec[speedrec.Count - 1].timer - speedrec[0].timer;
                    float pos = speedrec[speedrec.Count - 1].pos - speedrec[0].pos;
                    morespeed = pos / rt / 200;
                    rectimer = 0;
                    speedrec.Clear();


                    
                }
                if (morespeed > 0)
                {
                    morespeed -= 10 * delta;
                    if (morespeed < 0) morespeed = 0;  
                }
                else if (morespeed < 0)
                {
                    morespeed += 10 * delta;
                    if (morespeed> 0) morespeed = 0;  
                }
                dragpos += 200 * morespeed * delta;
                //弹性
                if (dragpos < 0)
                {
                    if (morespeed > 0)
                    {
                        morespeed -= 10 * delta;
                        if (morespeed < 0) morespeed = 0;
                    }
                    else if (morespeed < 0)
                    {
                        morespeed += 10 * delta;
                        if (morespeed > 0) morespeed = 0;
                    }
                    if (dragpos < -Size.Y)
                    {
                        dragpos = -Size.Y;
                        morespeed = 0;
                    }
                    float speedi = (0 - dragpos) / Size.Y;
                    dragpos += 200 *10*speedi* delta;
                    if (dragpos > 0) dragpos = 0;
                    
                }
                else if (dragpos > maxpos - Size.Y)
                {
                    if (morespeed > 0)
                    {
                        morespeed -= 10 * delta;
                        if (morespeed < 0) morespeed = 0;
                    }
                    else if (morespeed < 0)
                    {
                        morespeed += 10 * delta;
                        if (morespeed > 0) morespeed = 0;
                    }
                    if (dragpos > maxpos)
                    {
                        dragpos = maxpos;
                        morespeed = 0;
                    }
                    float speedi = (dragpos - (maxpos - Size.Y)) / Size.Y;
                    dragpos -= 200 * 10 * speedi * delta;
                    if (dragpos < maxpos - Size.Y) dragpos = maxpos - Size.Y;
                }

            }
            if (bdrag)//记录速度
            {
               
                 
                    rectimer += delta;
                    speedrec.Add(new rec(dragpos,rectimer));
                    if (rectimer > 0.25f)
                    {
                        while ((rectimer - speedrec[0].timer) > 0.25f)
                        {
                            speedrec.RemoveAt(0);
                        }
                    }
            }
            //if(dragpos<0&&bdrag==false)
            UpdatePos();
        }
    }
}
