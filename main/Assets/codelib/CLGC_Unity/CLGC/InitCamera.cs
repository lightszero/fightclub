using UnityEngine;
using System.Collections;

public class InitCamera : MonoBehaviour {

    public Camera cam;
	// Use this for initialization
	void Start () {
        cam.isOrthoGraphic = true;
        cam.orthographicSize = 1.0f;
        cam.transform.position = new Vector3(0, 0, -10);
        cam.transform.localScale = new Vector3(1, 1, 1);
        cam.transform.localRotation = new Quaternion(0, 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
