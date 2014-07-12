using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clgf.Anim
{
    public static class SpriteAniMgr
    {
        static Dictionary<string, SpriteAni> bufs = new Dictionary<string, SpriteAni>();
        public static SpriteAni CreateAni(Sprite sprite,string ani,Texture.TextureMgr tmgr,string texpath="")
        {
            string hash = sprite.name + "|" + ani;

            if (bufs.ContainsKey(hash) == false)
            {
                bufs[hash]= SpriteAni.CreateAni(sprite.anims[ani],tmgr,texpath);
            }


            return bufs[hash];
        }
        public static void ClearBuf()
        {
            bufs.Clear();
        }
    }
}
