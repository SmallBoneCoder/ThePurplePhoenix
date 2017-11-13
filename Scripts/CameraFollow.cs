using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public GameObject OwnerPlayer;
    public float speed = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    Vector2 dir;
    float timer = 0.5f;
    private void LateUpdate()
    {
        if (OwnerPlayer != null)
        {
            if (transform.position.x == OwnerPlayer.transform.position.x&&
                transform.position.y == OwnerPlayer.transform.position.y)
            {
                timer = 0.5f;
                return;
            }

            else
            {
                if(timer>=0)timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    
                    dir = Vector2.MoveTowards(transform.position,
                    OwnerPlayer.transform.position, speed * Time.deltaTime);
                    transform.position = new Vector3(
                        dir.x,
                        dir.y,
                        -10);
                }
               
            }
           
        }
    }
}
