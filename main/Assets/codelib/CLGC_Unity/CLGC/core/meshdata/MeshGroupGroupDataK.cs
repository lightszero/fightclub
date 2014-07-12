using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;


    class MeshGroupGroupDataK : IMeshGroupGroupData
    {
        string path;
        KAnim.Sprite dataK;
        protected MeshGroupGroupDataK(string path, byte[] seeddata,ICollection<string> anis)
        {
            //Debug.Log("MeshGroupGroupDataK(" + path + "," + seeddata.Length + "," + anis.Count);
            anims.Clear();
            anims = new List<string>(anis);
            this.path = path;
            dataK = KAnim.Sprite.LoadUnity(path, seeddata);
        }
        public static IMeshGroupGroupData Create(string filename)
        {
            //Debug.Log("MeshGroupGroupDataK:Create=" + filename);
            filename = filename.ToLower();
            var textAnimlist = Resources.Load(filename + "/anims", typeof(TextAsset)) as TextAsset;
            if (textAnimlist == null) return null;
            var seed = Resources.Load(filename + "/seeds.bin", typeof(TextAsset)) as TextAsset;
            if (seed == null) return null;

            var animlist = textAnimlist.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return new MeshGroupGroupDataK(filename, seed.bytes, animlist);

            
        }

        public MeshGroupFrame CreateMeshGroupFrame(string aniname, int frameid)
        {
            var ani = GetAnim(aniname);
            if (ani == null)
                return null;

            MeshGroupFrame f = new MeshGroupFrame();
            CreateMesh(f, new Vector2(ani.size.X,ani.size.Y),  ani.frames[frameid]);
            return f;
        }
        Dictionary<string, KAnim.SpriteAni> animforplay = new Dictionary<string, KAnim.SpriteAni>();
        public KAnim.SpriteAni GetSpriteAni(string name)
        {
            if (animforplay.ContainsKey(name))
                return animforplay[name];
            var ani =KAnim.SpriteAni.CreateAni(GetAnim(name),name);
            animforplay[name] = ani;
            return ani;
        }
        KAnim.Anim GetAnim(string aniname)
        {
            //Debug.Log("GetAnim:" + aniname);
            if (dataK == null)
                return null;

            aniname = aniname.Replace('\\', '/');
            if (dataK.anims.ContainsKey(aniname) == false)
            {
                var anis = Resources.Load(aniname, typeof(TextAsset)) as TextAsset;
                if (anis == null)
                {
                    //Debug.Log("GetAnim Fail:" + aniname);
                    return null;
                }
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(anis.bytes))
                {
                    KAnim.Anim anim = new KAnim.Anim(dataK);
                    anim.Read(ms);
                    dataK.anims[aniname] = anim;
                }
               
            }

            if (dataK.anims.ContainsKey(aniname) == false)
            {
                Debug.Log("GetAnim Fail:" + aniname);
                return null;
            }
            return dataK.anims[aniname];
        }
        public int GetMeshGroupMaxFrame(string aniname)
        {
            var ani = GetAnim(aniname);
            if (ani == null)
                return -1;
            else
                return ani.frames.Count - 1;
        }

        List<string> anims = new List<string>();
        public List<string> GetAllAniname()
        {
            return anims;
        }

        void CreateMesh(MeshGroupFrame frame,Vector2 size, KAnim.Frame srcframe)
        {
            //Debug.Log("MeshGroupGroupDataK.CreateMesh");
            MeshGroupFrame.InitShader();

            frame.BeginMesh();
            for(int i =srcframe.elems.Count-1;i>=0;i--)
            {
                DoAttm(frame, size, srcframe.elems[i]);
            }
            frame.FinishMesh();
        }
        static float scaleWorld = 1.0f / 64.0f;
        void DoAttm(MeshGroupFrame meshframe,Vector2 anisize, KAnim.Element elem)
        {
            //跳过dummy的绘制
            if (elem.tag.IndexOf("dummy:") == 0) return;

            var seed =  dataK.seeds[elem.seed];

            TextureBlock? tb = TextureBlockMgr.getTexture(seed.texname.ToLower());
            //Vector2 size = new Vector2(tb.Value.uv.width * tb.Value.tex.width *scaleWorld,
            //         tb.Value.uv.height * tb.Value.tex.height *scaleWorld);

            Vector2 size = seed.size;
            Vector2 orient = seed.orient;

            Matrix4x4 dest = Matrix4x4.identity;
            //目标位置
            {
                
                Vector2 t = new Vector2( elem.pos.x - anisize.x/2 ,anisize.y -elem.pos.y);
               
                float rotate = (float)(elem.rotate / Math.PI * 180.0);
                while (rotate < 0) rotate += 360;
                while (rotate > 360) rotate -= 360;
                if (float.IsNaN(rotate)) rotate = 0;
                Matrix4x4 move = Matrix4x4.TRS(t, Quaternion.identity, Vector3.one);
                Matrix4x4 rs = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(rotate, new Vector3(0, 0, -1)), elem.scale);
                Matrix4x4 scaleworld = Matrix4x4.Scale(new Vector3(scaleWorld, scaleWorld, scaleWorld));

                dest = scaleworld * move * rs;// scaleworld* rs *move;
            }
            Color c = Color.white;
            c.a = (float)elem.color.a / 255.0f;
            c.r = (float)elem.color.r / 255.0f;
            c.g = (float)elem.color.g / 255.0f;
            c.b = (float)elem.color.b / 255.0f;
            if (tb != null)
            {

                meshframe.DrawRect(tb.Value, size, orient, dest, c);

            }
            else
            {
                meshframe.DrawRect(TextureBlock.Empty, new Vector2(1, 1), dest, c);
            }


        }
    }

