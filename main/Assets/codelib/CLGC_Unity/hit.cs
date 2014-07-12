using UnityEngine;
using System.Collections;

public class hit : MonoBehaviour {
		public Texture2D tex;
	public RenderTexture rt;
	// Use this for initialization
	void Start () {
	
		//tex.ReadPixels(new Rect(0,0,tex.width,tex.height),tex.width,tex.height);
		byte[] bb =tex.EncodeToPNG();
		Texture2D tex2 = new Texture2D(tex.width,tex.height,TextureFormat.ARGB32,false);
		tex2.LoadImage(bb);
		Color32[] col=	tex.GetPixels32(0);
		Debug.Log(col.Length);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
