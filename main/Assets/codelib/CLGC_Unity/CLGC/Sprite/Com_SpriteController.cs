using UnityEngine;
using System.Collections;

public class Com_SpriteController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	float speedjump=0;
	
	void Update () {
		Vector3 dir = new Vector3(0,-9.8f*Time.deltaTime,0);
		if(vjoy.joydir.magnitude>0)
		{

			
			

			dir.x=vjoy.joydir.x*3*Time.deltaTime;
			dir.z=vjoy.joydir.y*3*Time.deltaTime;

			
		}
		foreach(var binfo in vjoy.btnInfo)
		{
			if(binfo.id== KeyCode.J&&binfo.bdown)
			{//跳跃按下
				if(nJump<2)
				{
					Jump();
				}
				
			}
            if (binfo.id == KeyCode.K && binfo.bdown)
            {//Rush按下
                if (rushTimer <= 0)
                {
                    rushTimer = 1.0f;
                }
            }
		}
		speedjump -= 9.8f*3*Time.deltaTime;
		if(speedjump<-9.8f)
		{
			speedjump = -9.8f;
		}
        if (rushTimer > 0)
        {
            dir.x *= 7;
            rushTimer -= Time.deltaTime;
        }
		dir.y =speedjump*Time.deltaTime;
		CharacterController cc =this.GetComponent<CharacterController>();
		CollisionFlags flag= cc.Move(dir);
        if (flag == CollisionFlags.CollidedBelow)
        {
            nJump = 0;
        }
        else
        {

            Debug.Log(flag);
        }
	}
	int nJump=0;
	void Jump()
	{
		if(speedjump>0)return;
		nJump+=1;
		speedjump =9.8f;
	}

    float rushTimer = 0f;
    
}
