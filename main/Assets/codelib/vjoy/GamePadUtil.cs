using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace control
{
    public class GamePadUtil
    {
    }
    public enum PADTAG      //一个字节可以表示方向，两次
    {
        PADKEY_BACKUP = 7, PADKEY_UP = 8, PADKEY_FRONTUP = 9,
        PADKEY_BACK = 4, PADKEY_RELEASE = 5, PADKEY_FRONT = 6,
        PADKEY_BACKDOWN = 1, PADKEY_DOWN = 2, PADKEY_FRONTDOWN = 3,
    }
    public enum KEYTAG      //一个字节描述八个按键状态，一次
    {
        GAMEKEY_NONE=0,
        GAMEKEY_1 = 1,
        GAMEKEY_2 = 2,
        GAMEKEY_3 = 4,
        GAMEKEY_4 = 8,
        GAMEKEY_5 = 16,
        GAMEKEY_6 = 32,
        GAMEKEY_7 = 64,
        GAMEKEY_8 = 128,
    }
    public enum FUNCKEYTAG  //功能键，Controller不需要管理
    {
        FUNCKEY_BACK = 1,
        FUNCKEY_START= 2,
    }
    public interface IPlayerInputController
    {
        int ErrorCount
        {
            get;
            set;
        }
        void Update();
        FACETAG FACETAG
        {
            get;
            set;
        }
        Queue<cmd> CommandList
        {
            get;
        }
        Queue<FUNCKEYTAG> EventList
        {
            get;
        }
        void OnDetach(Player driver);
        void OnAttach(Player driver);
    }
    public struct cmd
    {
        public cmd(byte _pad, byte _key)
        {
            padtag = _pad;
            keytag = _key;
        }
        public byte padtag;
        public byte keytag;
        public override string ToString()
        {
            return  GetPadString()+GetBtnString();
        }
        public string GetPadString()
        {
            string str = "";// padtag.ToString();
            if (padtag == 1) str += "↙";
            if (padtag == 2) str += "↓";
            if (padtag == 3) str += "↘";
            if (padtag == 4) str += "←";
            //if (padtag == 5) str += "⊕";
            if (padtag == 6) str += "→";
            if (padtag == 7) str += "↖";
            if (padtag == 8) str += "↑";
            if (padtag == 9) str += "↗";
            return str;
        }
        public string GetBtnString()
        {
            string str = "";
            if ((keytag & (byte)KEYTAG.GAMEKEY_1) != 0) str += "拳";
            if ((keytag & (byte)KEYTAG.GAMEKEY_2) != 0) str += "脚";
            if ((keytag & (byte)KEYTAG.GAMEKEY_3) != 0) str += "跳";
            if (keytag != 0 && str.Length == 0) str += keytag.ToString("X2");
            return str;
        }
    }
    public enum FACETAG
    {
        FACETORIGHT,
        FACETOLEFT,
    }
    public class GamePadController : IPlayerInputController
    {
        public GamePadController()
        {
            //PlayerIndex = pindex;
            CommandList = new Queue<cmd>();
            FACETAG = control.FACETAG.FACETORIGHT;
        }
        //public PlayerIndex PlayerIndex
        //{
        //    get;
        //    private set;
        //}
        public int ErrorCount
        {
            get;
            set;
        }
        public void Update()
        {
            try
            {
                PADTAG ptag = PADTAG.PADKEY_RELEASE;
                KEYTAG ktag = KEYTAG.GAMEKEY_NONE;
                //GamePadState state = GamePad.GetState(PlayerIndex);
                if(vjoy.joydir.x<-0.5f)
                {
                    ptag--;
                }
                if(vjoy.joydir.x>0.5f)
                {
                    ptag++;
                }
                if(vjoy.joydir.y<-0.5f)
                {
                    ptag += 3;
                }
                if(vjoy.joydir.y>0.5f)
                {
                    ptag -= 3;
                }
                if (vjoy.btnInfo[0].bdown || vjoy.btnInfo[2].bdown)//J or L
                {
                    ktag |= KEYTAG.GAMEKEY_1;
                }
                if(vjoy.btnInfo[1].bdown||vjoy.btnInfo[2].bdown)//K or L
                {
                    ktag |= KEYTAG.GAMEKEY_1;
                }


                if (last.padtag != (byte)ptag || last.keytag != (byte)ktag)
                {
                    last.padtag = (byte)ptag;
                    last.keytag = (byte)ktag;
                    CommandList.Enqueue(last);
                }


            }
            catch
            {
                ErrorCount = ErrorCount + 1;
            }
        }
        public override string ToString()
        {
            return last.ToString();
            
        }
        public FACETAG FACETAG
        {
            get;
            set;
        }
        private cmd last = new cmd((byte)PADTAG.PADKEY_RELEASE, (byte)KEYTAG.GAMEKEY_NONE);
        public Queue<cmd> CommandList
        {
            get;
            private set;
        }
        public Queue<FUNCKEYTAG> EventList
        {
            get;
            private set;
        }


        public void OnDetach(Player driver)
        {

        }

        public void OnAttach(Player driver)
        {

        }
    };
}
