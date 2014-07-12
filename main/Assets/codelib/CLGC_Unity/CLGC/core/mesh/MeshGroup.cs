
using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;


	public class MeshGroup
	{
        public MeshGroup(IMeshGroupGroupData data, string aniname)
        {
            //this.puppet = puppet;
            //this.anim = anim;
            this.data=data;
            this.aniname=aniname;
            maxFrame = data.GetMeshGroupMaxFrame(aniname);
        }
        public int maxFrame
        {
            get;
            private set;
        }
        Dictionary<int, MeshGroupFrame> frames = new Dictionary<int, MeshGroupFrame>();

        public MeshGroupFrame GetFrame(int frame)
        {

            if (frames.ContainsKey(frame))
            {
                return frames[frame];
            }
            else
            {
                MeshGroupFrame f = data.CreateMeshGroupFrame(aniname, frame);
                //CreateMesh Here
                //MeshGroupPuppet.CreateMesh(f,puppet, anim.frames[frame]);
                frames[frame] = f;
                return f;
            }

        }
        public bool BufMeshGroup()
        {
            bool bBuf = false;
            for(int i=0;i<=maxFrame;i++)
            {
                if (frames.ContainsKey(i) == false)
                {
                    bBuf = true;
                    GetFrame(i);
                }
            }
            return bBuf;
        }
        public IMeshGroupGroupData data
        {
            get;
            private set;
        }
        string aniname;
        //public ha_ani_tool.hadata.AniRuntime anim
        //{
        //    get;
        //    private set;
        //}
        //public static MeshGroup Create(MeshGroupPuppet puppet,string aniname)
        //{
        //    Debug.Log("MeshGroup.Create:" + puppet.puppet.FileName + "|" + aniname);
        //    var puppetani = Resources.Load(aniname+".puppetanim", typeof(TextAsset)) as TextAsset;
        //    var panddata = ha_ani_tool.hadata.puppetanimdata.LoadFromString(puppetani.text);
        //    var animdata = ha_ani_tool.hadata.AniRuntime.CreateFrom(panddata);
        //    if (animdata != null)
        //    {
        //        return new MeshGroup(puppet,animdata);
        //    }
        //    return null;
        //}
	}

    public class MeshGroupGroup
    {
        public IMeshGroupGroupData data;
        public MeshGroupGroup(IMeshGroupGroupData data)
        {
            this.data = data;
        }
        public Dictionary<string, MeshGroup> mapGroupAnim = new Dictionary<string, MeshGroup>();
        public MeshGroup GetMeshGroup(string aniname)
        {
            if (mapGroupAnim.ContainsKey(aniname)==false)
            {
                if (data.GetMeshGroupMaxFrame(aniname) < 0) return null;
                mapGroupAnim[aniname] = new MeshGroup(data, aniname);
            }
            return mapGroupAnim[aniname];
        }
        public bool BufMeshGroup()
        {
            bool bBuf = false;
            foreach(var l in   data.GetAllAniname())
            {
                if (GetMeshGroup(l).BufMeshGroup())
                    bBuf = true;
            }
            return bBuf;
        }
    }
    public interface IMeshGroupGroupData
    {
        MeshGroupFrame CreateMeshGroupFrame(string animname,int frameid);
        int GetMeshGroupMaxFrame(string aniname);
        List<String> GetAllAniname();
    }


