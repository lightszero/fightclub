using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace control
{

    public enum DriveStyle
    {
        PadDrive,           //输入方向启动的技能，比如移动等等，
        //PadDrive_Clear,     //输入方向启动的技能，但是技能触发后之前的输入全部无效。比如 后后触发后闪，全部清除，再发后下前 需重新输入
        BtnPressDrive,      //按键按下启动的技能，
        //BtnPressDrive_NoClear,   //按键启动，但是不将输入无效化，比如A 和A.A.A ,只有保证A不会清除输入，A.A.A才能发的出来。
        BtnReleaseDrive     //通过按键释放启动的技能   
    }
    public class SkillCmd//描述一条出招
    {

        public String SkillName;
        public DriveStyle DriveStyle;//触发条件
        public bool ClearCmd=false;//是否清除之前的输入
        public bool BtnReleaseDrive =false;//是否是按钮释放触发
        public List<byte> padtag = new List<byte>();//手柄输入
        public List<byte> btntag = new List<byte>();//按键顺序
        private bool ComparePad(Queue<cmd> queue)
        {
            int ipos = padtag.Count - 1;
            if (ipos < 0) return false;
            int iseek = 1;
            int lastpad = 0;
            int pd = 0;
            foreach (cmd p in queue.Reverse())
            {

                if (p.padtag == lastpad) continue;      //1.输入冗余过滤
                lastpad = p.padtag;

                if (p.padtag == 5)                      //2.无输入冗余过滤
                {
                    if (padtag[ipos] == 5) ipos--;//输入要求空
                    if (ipos < 0) return true;
                    continue;//无方向，跳过
                }


                if (ipos < padtag.Count - 1)
                {
                    if (padtag[ipos + 1] == p.padtag)   //3.简化操作判定
                    {//相容引起的重复可以跳
                        if(pd>0)
                            continue;//重复方向，跳过
                        
                    }
                }



                pd = PadDist(padtag[ipos], p.padtag);
                if (pd > 1) return false;
                ipos--;
                if (ipos < 0) return true;
                if (pd == 0)//相等
                {
                    iseek = 0;
                    //ipos--;
                }
                else//相容                            //3.简化操作判定，斜方向可以简化两个方向
                {
                    int irpd=0;
                    if(ipos+2<padtag.Count)
                        irpd = padtag[ipos + 2];      //3.1同一个四分之一圆里来回不能简化
                    if (iseek == 0)
                    {
                        pd = PadDist(padtag[ipos], p.padtag);
                        if (pd <= 1)
                        {
                            iseek = 0;
                            if (padtag[ipos] != irpd)
                            {
                                ipos--;
                                if (ipos < 0) return true;
                            }
                        }
                    }
                    iseek++;
                    if (iseek > 1)   iseek = 0;
                }
               

            }
            return false;
        }
        private bool CompareBtn(Queue<cmd> queue)
        {
            return false;
        }
        int PadDist(byte l, byte r)
        {
            if (l == r) return 0;
            if (l==5||r==5) return -1;
            if (l == 2)
            {
                if (r == 1 || r == 3) return 1;
                if (r == 4 || r == 6) return 2;
                if (r == 7 || r == 9) return 3;
                if (r == 8) return 4;
            }
            if (l == 8)
            {
                if (r == 1 || r == 3) return 3;
                if (r == 4 || r == 6) return 2;
                if (r == 7 || r == 9) return 1;
                if (r == 2) return 4;
            }
            if (l ==4)
            {
                if (r == 1 || r == 7) return 1;
                if (r == 2 || r == 8) return 2;
                if (r == 3 || r == 9) return 3;
                if (r == 6) return 4;
            }
            if (l == 6)
            {
                if (r == 1 || r == 7) return 3;
                if (r == 2 || r == 8) return 2;
                if (r == 3 || r == 9) return 1;
                if (r == 4) return 4;
            }
            return 0;
        }
        public bool Parse(Queue<cmd> queue)//判断是否匹配
        {
            if (btntag.Count > 0)//按键驱动
            {
                if (BtnReleaseDrive == false)//按钮按下驱动
                {
                    if (((byte)queue.Last().keytag & btntag.Last()) > 0)
                    {
                        if (btntag.Count > 1)//按键匹配
                        {
                            if (!CompareBtn(queue)) return false;
                        }
                        if (padtag.Count > 0)//方向匹配
                        {
                            if (!ComparePad(queue)) return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else            //按钮释放驱动
                {
                    if (((~(byte)queue.Last().keytag) & btntag.Last()) > 0)
                    {
                        if (btntag.Count > 1)//按键匹配
                        {
                            if (!CompareBtn(queue)) return false;
                        }
                        if (padtag.Count > 0)//方向匹配
                        {
                            if (!ComparePad(queue)) return false;
                        }
                    }
                }
            }
            else if (padtag.Count > 0)//方向驱动
            {
                if (queue.Last().keytag != 0)
                {
                    if (padtag.Count > 0)//方向匹配
                    {
                        if (!ComparePad(queue)) return false;
                    }
                }
            }
            else
            {
                return false;
            }
            if (ClearCmd) queue.Clear();
            return true;
        }
        public override string ToString()
        {
            string str = SkillName;
            str += ":";
            foreach (byte ptag in padtag)
            {
                if (ptag == 1) str += "↙";
                if (ptag == 2) str += "↓";
                if (ptag == 3) str += "↘";
                if (ptag == 4) str += "←";
                if (ptag == 5) str += "松";
                if (ptag == 6) str += "→";
                if (ptag == 7) str += "↖";
                if (ptag == 8) str += "↑";
                if (ptag == 9) str += "↗";
            }
            foreach (byte ktag in btntag)
            {
                if ((ktag & (byte)KEYTAG.GAMEKEY_1) != 0) str += "跳";
                if ((ktag & (byte)KEYTAG.GAMEKEY_2) != 0) str += "打";
                if (ktag != 0 && str.Length == 0) str += ktag.ToString("X2");
            }
            str += ";";
            return str;
        }
    }
    public class PlayerState//角色状态
    {
        public List<SkillCmd> listSkill = new List<SkillCmd>();//技能列表
    }
    public class Player
    {
        IPlayerInputController controller;
        public  void SetInput(IPlayerInputController _controller)//设定输入源，可以是AI
        {

            controller = _controller;

        }
        public PlayerState PlayerState
        {
            get;
            set;
        }
        public bool Update()
        {
            //判断是否有招式符合条件
            //Debug.Log("Update " + (PlayerState != null) + "|" + (controller!=null));
            if (PlayerState != null&&controller.CommandList.Count!=0)
            {
                //Debug.Log("Update 2");

                foreach (SkillCmd scmd in PlayerState.listSkill)
                {
                    //Debug.Log(scmd.SkillName);
                    if (scmd.Parse(controller.CommandList))
                    {
                        lastskill = scmd;
                        return true;
                    }
                }
            }
            return false;
        }
        SkillCmd lastskill=null;
        public string GetSkillStr()
        {
            if (lastskill == null) return "";
            return lastskill.ToString();
        }
    }
}
