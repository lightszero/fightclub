using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneUnlimit : MonoBehaviour {

	// Use this for initialization
    public GameObject scene1;
    public List<GameObject> srcScenes;
	void Start () {
        lastScene = scene1;
        mapqueue.Enqueue(scene1);
        AddScene();
	}

    public Transform testObj;
    public static Queue<GameObject> mapqueue = new Queue<GameObject>();
    public static int queueLength = 3;
	// Update is called once per frame
	void Update () {
        if (lastScene != null)
        {
            if (lastScene.collider.bounds.Contains(testObj.transform.position)
                )
            {
                AddScene();
            }
            //Debug.Log(lastScene.transform.collider.bounds);
        }
	}
    GameObject lastScene = null;
   
    void AddScene()
    {
        int iss = Random.Range(0, srcScenes.Count);
        var oldScene = lastScene;
        lastScene = GameObject.Instantiate(srcScenes[iss], Vector3.zero, Quaternion.identity) as GameObject;
        //lastScene.hideFlags = HideFlags.HideAndDontSave;

        if (lastScene.collider == null)
        {//计算碰撞体
            var collider = lastScene.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            Vector3 max = lastScene.transform.position;
            Vector3 min = max;
            for (int i = 0; i < lastScene.transform.childCount; i++)
            {
                var sobj = lastScene.transform.GetChild(i);
                if (sobj.collider != null)
                {
                    max = Vector3.Max(max, sobj.collider.bounds.max);
                    min = Vector3.Min(min, sobj.collider.bounds.min);
                }
            }
            Vector3 center = max * 0.5f + min * 0.5f;
            collider.center = center;
            collider.size = (max - center) * 2;
        }

        {
            //计算位置
            var outobj = oldScene.transform.FindChild("door_out");
            var inobj = lastScene.transform.FindChild("door_in");
            var move = inobj.transform.position - outobj.transform.position;
            lastScene.transform.position = -move;
        }
        mapqueue.Enqueue(lastScene);
        if (mapqueue.Count > queueLength)
        {
            var removeScene = mapqueue.Dequeue();
            GameObject.Destroy(removeScene);
        }
    }
}
