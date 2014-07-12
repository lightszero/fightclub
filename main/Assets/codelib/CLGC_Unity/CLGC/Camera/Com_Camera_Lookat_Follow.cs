using UnityEngine;
using System.Collections;

public class Com_Camera_Lookat_Follow : MonoBehaviour {
	
	public Transform _target;

	Vector3 campostrans;
	// Use this for initialization
	void Start () {
			campostrans =this.camera.transform.position - _target.position;
		campos =this.camera.transform.position;
	}
	public float nearsplit =0.5f;
	// Update is called once per frame
	Vector3 campos;
	void Update () {
		
		
		if(_target==null)return;
					
		Vector3 earlypos = this.camera.transform.position- campostrans;	
		Vector3 target =Vector3.Lerp(earlypos,_target.position,nearsplit);
		
		Vector3 wanteye= Vector3.Lerp(this.camera.transform.position, _target.position+campostrans,nearsplit);
		this.camera.transform.position = Vector3.Lerp(this.camera.transform.position, wanteye,nearsplit);

		this.camera.transform.LookAt(target);

	}
    public void LookAt()
    {
        if (_target == null) return;
        Vector3 target = _target.position;
        this.camera.transform.LookAt(target);
		target.Normalize();
    }
    public void LookAtMove()
    {
        if (_target == null) return;
        Vector3 target = _target.position;

        Ray targetray = new Ray(target, -this.camera.transform.forward);
        Plane cpanle = new Plane(this.camera.transform.position, this.camera.transform.position + this.camera.transform.up,
            this.camera.transform.position + this.camera.transform.right);
        float dist = 0;
        cpanle.Raycast(targetray, out dist);
        Vector3 eye = target - this.camera.transform.forward * dist;
        
        
        this.camera.transform.position = eye;

        this.camera.transform.LookAt(target);
    }
}
