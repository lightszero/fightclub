using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;

public class MeshGroupFrame
{
    public Mesh mesh;
    public List<Material> mats = new List<Material>();


    public static Shader UIShader;
    public static Shader UIShaderShadow;
    public static void InitShader()
    {
        if (MeshGroupFrame.UIShader == null)
        {
            //MeshGroupFrame.UIShader = Shader.Find("clgc/BlendVertexColor");
            //MeshGroupFrame.UIShaderShadow = Shader.Find("clgc/BlendVertexColorShadow");
            MeshGroupFrame.UIShader = Shader.Find("clgc/nolit"); 
            MeshGroupFrame.UIShaderShadow = Shader.Find("clgc/nolit");
        }
    }
    Texture2D texlast;

    public void DrawRect(TextureBlock src, Rect dest, Color c)
    {
        Vector2 v1 = (new Vector2(dest.xMin, dest.yMin));
        Vector2 v2 = (new Vector2(dest.xMax, dest.yMin));
        Vector2 v3 = (new Vector2(dest.xMin, dest.yMax));
        Vector2 v4 = (new Vector2(dest.xMax, dest.yMax));

        var uv1 = new Vector2(src.uv.xMin, src.uv.yMin);
        var uv2 = new Vector2(src.uv.xMax, src.uv.yMin);
        var uv3 = new Vector2(src.uv.xMin, src.uv.yMax);
        var uv4 = new Vector2(src.uv.xMax, src.uv.yMax);

        AddTri(v1, v2, v3, uv1, uv2, uv3, c, c, c, src.tex);
        AddTri(v3, v2, v4, uv3, uv2, uv4, c, c, c, src.tex);
    }

    public void DrawRect(TextureBlock src, Vector2 size, Vector2 pos, Color c)
    {
        Vector2 v1 = (new Vector2(pos.x - size.x / 2, pos.y - size.y / 2));
        Vector2 v2 = (new Vector2(pos.x + size.x / 2, pos.y - size.y / 2));
        Vector2 v3 = (new Vector2(pos.x - size.x / 2, pos.y + size.y / 2));
        Vector2 v4 = (new Vector2(pos.x + size.x / 2, pos.y + size.y / 2));

        var uv1 = new Vector2(src.uv.xMin, src.uv.yMin);
        var uv2 = new Vector2(src.uv.xMax, src.uv.yMin);
        var uv3 = new Vector2(src.uv.xMin, src.uv.yMax);
        var uv4 = new Vector2(src.uv.xMax, src.uv.yMax);

        AddTri(v1, v2, v3, uv1, uv2, uv3, c, c, c, src.tex);
        AddTri(v3, v2, v4, uv3, uv2, uv4, c, c, c, src.tex);
        //throw new System.NotImplementedException();
    }


    //以源图块的中心点应用矩阵
    public void DrawRect(TextureBlock src, Vector2 size, Matrix4x4 dest, Color c)
    {
        Vector2 v1 = dest.MultiplyPoint(new Vector2(-size.x / 2, -size.y / 2));
        Vector2 v2 = dest.MultiplyPoint(new Vector2(size.x / 2, -size.y / 2));
        Vector2 v3 = dest.MultiplyPoint(new Vector2(-size.x / 2, size.y / 2));
        Vector2 v4 = dest.MultiplyPoint(new Vector2(size.x / 2, size.y / 2));

        var uv1 = new Vector2(src.uv.xMin, src.uv.yMin);
        var uv2 = new Vector2(src.uv.xMax, src.uv.yMin);
        var uv3 = new Vector2(src.uv.xMin, src.uv.yMax);
        var uv4 = new Vector2(src.uv.xMax, src.uv.yMax);
        AddTri(v1, v2, v3, uv1, uv2, uv3, c, c, c, src.tex);
        AddTri(v3, v2, v4, uv3, uv2, uv4, c, c, c, src.tex);

    }
    public void DrawRect(TextureBlock src, Vector2 size, Vector2 orient, Matrix4x4 dest, Color c)
    {
        float xmin = -orient.x;
        float xmax = (size.x - orient.x);
        float ymin = -(size.y - orient.y);
        float ymax = orient.y;
        Vector2 v1 = dest.MultiplyPoint(new Vector2(xmin, ymin));
        Vector2 v2 = dest.MultiplyPoint(new Vector2(xmax, ymin));
        Vector2 v3 = dest.MultiplyPoint(new Vector2(xmin, ymax));
        Vector2 v4 = dest.MultiplyPoint(new Vector2(xmax, ymax));

        var uv1 = new Vector2(src.uv.xMin, src.uv.yMin);
        var uv2 = new Vector2(src.uv.xMax, src.uv.yMin);
        var uv3 = new Vector2(src.uv.xMin, src.uv.yMax);
        var uv4 = new Vector2(src.uv.xMax, src.uv.yMax);
        AddTri(v1, v2, v3, uv1, uv2, uv3, c, c, c, src.tex);
        AddTri(v3, v2, v4, uv3, uv2, uv4, c, c, c, src.tex);
    }
    public bool ShadowEnable =true;
    bool lastShadow = true;
    public void AddTri(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Color c1, Color c2, Color c3, Texture2D tex)
    {
        int startindex = vecrtices.Count;
        vecrtices.Add(v1);
        vecrtices.Add(v2);
        vecrtices.Add(v3);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        uv.Add(uv1);
        uv.Add(uv2);
        uv.Add(uv3);
        if (submesh.Count == 0 || texlast != tex || ShadowEnable!=lastShadow)
        {
            submesh.Add(new List<int>());
            lastShadow = ShadowEnable;
        }
        
        submesh[submesh.Count - 1].Add(startindex);
        submesh[submesh.Count - 1].Add(startindex + 2);
        submesh[submesh.Count - 1].Add(startindex + 1);
        if (mats.Count < submesh.Count)
        {
            mats.Add(new Material(ShadowEnable?UIShaderShadow:UIShader));
        }
        mats[submesh.Count - 1].SetTexture("_MainTex", tex);
        texlast = tex;
    }

    List<Vector3> vecrtices = new List<Vector3>();
    //List<Vector3> normals = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<Vector2> uv = new List<Vector2>();

    List<List<int>> submesh = new List<List<int>>();
    public void BeginMesh()
    {
        mesh = new Mesh();
        //mesh.Clear();
        texlast = null;
        submesh.Clear();
        vecrtices.Clear();
        colors.Clear();
        //normals.Clear();
        uv.Clear();
        //mats.Clear();


      
    }
//    int submeshcount = 0;
//    int facecount = 0;

    public void FinishMesh()
    {
        if (vecrtices.Count == 0)
        {
            Color c = Color.white;
            float width = 1.0f;
            AddTri(new Vector2(-1, -width / 128), new Vector2(1, -width / 128), new Vector2(-1, width / 128), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), c, c, c, null);
            AddTri(new Vector2(-1, width / 128), new Vector2(1, -width / 128), new Vector2(1, width / 128), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), c, c, c, null);
            AddTri(new Vector2(-width / 128, -1), new Vector2(-width / 128, 1), new Vector2(width / 128, -1), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), c, c, c, null);
            AddTri(new Vector2(width / 128, -1), new Vector2(-width / 128, 1), new Vector2(width / 128, 1), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), c, c, c, null);
        }
        
        {
//            facecount = vecrtices.Count / 3;
//            submeshcount = submesh.Count;
            mesh.vertices = vecrtices.ToArray();
            //mesh.normals = normals.ToArray(); 
            mesh.colors = colors.ToArray();
            mesh.uv = uv.ToArray();
            mesh.subMeshCount = submesh.Count;
            for (int i = 0; i < submesh.Count; i++)
            {
                mesh.SetTriangles(submesh[i].ToArray(), i);
            }
			mesh.RecalculateBounds();

            //Debug.Log (mesh.bounds);
        }
        texlast = null;
    }
}
