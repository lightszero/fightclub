using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;


    public class MeshGroupGroupDataHA : IMeshGroupGroupData
    {
        protected MeshGroupGroupDataHA(ha_ani_tool.hadata.puppetdata puppet)
        {
            this.puppet = puppet;
            mapPart.Clear();
            foreach (var p in puppet.Parts)
            {
                if (mapPart.ContainsKey(p.bone) == false)
                {
                    mapPart[p.bone] = new List<ha_ani_tool.hadata.Attachment>();
                }
                foreach (var attm in puppet.Attachments)
                {
                    if (attm.name == p.defaultAttachment)
                    {
                        mapPart[p.bone].Add(attm);
                    }
                }
            }
        }
        public ha_ani_tool.hadata.puppetdata puppet
        {
            get;
            private set;
        }
        public Dictionary<string, List<ha_ani_tool.hadata.Attachment>> mapPart = new Dictionary<string, List<ha_ani_tool.hadata.Attachment>>();

        public Dictionary<string, ha_ani_tool.hadata.AniRuntime> mapAnis = new Dictionary<string, ha_ani_tool.hadata.AniRuntime>();
        public static IMeshGroupGroupData Create(string puppetfile)
        {
            //Debug.Log("MeshGroupPuppet.Create:" + puppetfile);
            puppetfile = puppetfile.ToLower();

            var puppett = Resources.Load(puppetfile, typeof(TextAsset)) as TextAsset;
            if (puppett == null) return null;
            var puppetdata = ha_ani_tool.hadata.puppetdata.LoadFormString(puppett.text);
            if (puppetdata != null)
            {
                MeshGroupGroupDataHA puppet = new MeshGroupGroupDataHA(puppetdata);
                return puppet;
            }
            return null;
        }

        
        void CreateMesh(MeshGroupFrame frame, Dictionary<string, ha_ani_tool.hadata.Node.Frame> frames)
        {
            Debug.Log("MeshGroupFrame.CreateMesh");
            MeshGroupFrame.InitShader();

            frame.BeginMesh();
            foreach (var n in frames)
            {
                DoAttm(frame,n.Key, n.Value);
            }
            frame.FinishMesh();
        }


        void DoAttm(MeshGroupFrame meshframe, string name, ha_ani_tool.hadata.Node.Frame frame)
        {
            if (mapPart.ContainsKey(name))
            {

                foreach (var attm in mapPart[name])
                {


                    TextureBlock? tb = TextureBlockMgr.getTexture(attm.textureName.ToLower());
                    Vector2 size = new Vector2(tb.Value.uv.width * tb.Value.tex.width / 128 * 2,
                        tb.Value.uv.height * tb.Value.tex.height / 128 * 2);

                    Matrix4x4 src = Matrix4x4.identity;
                    {
                        float tx = attm.translationX / 128 * 2;
                        float ty = attm.translationY / 128 * 2;
                        src.SetTRS(new Vector2(tx, ty), Quaternion.AngleAxis(attm.rotation, new Vector3(0, 0, 1)), Vector3.one);
                    }

                    Matrix4x4 dest = Matrix4x4.identity;
                    //目标位置
                    {
                        float tx = frame.translationX / 128 * 2;
                        float ty = frame.translationY / 128 * 2;
                        while (frame.rotation < 0) frame.rotation += 360;
                        while (frame.rotation > 360) frame.rotation -= 360;
                        if (float.IsNaN(frame.rotation)) frame.rotation = 0;
                        dest.SetTRS(new Vector2(tx + 0, ty + 0), Quaternion.AngleAxis(frame.rotation, new Vector3(0, 0, -1)), Vector3.one);
                    }

                    dest = dest * src;
                    //dest = move * dest;
                    //dest = dest * src;
                    if (tb != null)
                    {
                        meshframe.DrawRect(tb.Value, size, dest, Color.white);

                    }
                    else
                    {
                        meshframe.DrawRect(TextureBlock.Empty, new Vector2(1, 1), dest, Color.white);
                    }

                }
            }
        }


        public MeshGroupFrame CreateMeshGroupFrame(string aniname, int frameid)
        {
            Debug.Log("CreateMeshGroupFrame:" + aniname+","+frameid);
           ha_ani_tool.hadata.AniRuntime ani = getAniRunTime(aniname);
           if (ani != null)
           {
               MeshGroupFrame f =new MeshGroupFrame();
               CreateMesh(f, ani.frames[frameid]);
               return f;
           }
           return null;
        }

        ha_ani_tool.hadata.AniRuntime getAniRunTime(string aniname)
        {
            if (mapAnis.ContainsKey(aniname))
            {
                return mapAnis[aniname];
            }
            else
            {
                var puppetani = Resources.Load(aniname + ".puppetanim", typeof(TextAsset)) as TextAsset;
                if (puppetani == null) return null;
                var panddata = ha_ani_tool.hadata.puppetanimdata.LoadFromString(puppetani.text);
                var animdata = ha_ani_tool.hadata.AniRuntime.CreateFrom(panddata);
                if (animdata != null)
                {
                    mapAnis[aniname] = animdata;
                    return mapAnis[aniname];
                }

            }
            return null;
        }
        public int GetMeshGroupMaxFrame(string aniname)
        {
            Debug.Log("GetMeahGroupMaxFrame:" + aniname);
            ha_ani_tool.hadata.AniRuntime ani = getAniRunTime(aniname);
            if (ani != null)
                return ani.MaxFrame;
            else
                return -1;
        }
        public bool TestAni(string aniname)
        {
            return getAniRunTime(aniname) != null;
        }

        public List<string> GetAllAniname()
        {
            List<string> anis =new List<string>();
            foreach (var ani in this.puppet.Anims)
            {
                anis.Add(ani.name);
            }
            return anis;
        }
    }


