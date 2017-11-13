using UnityEngine;
using System.Collections;

public class LazerController : MonoBehaviour {
    public Transform position;
    public int Damage=10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = position.position;
        transform.up = position.up;
	}
    float endTime = 0;
    string bulletname;
    float time;
    private void Timer()
    {
        endTime += 0.1f;
        if (endTime >= time)
        {
            ObjectPool.GetInstance().DestroyObject(gameObject, bulletname);
            endTime = 0;
            CancelInvoke("Timer");

        }
    }
    public void InitState(Transform pos, string id)
    {
        endTime = 0;
        time = 2;
        position = pos;
        switch (id)
        {
            case "0": { } break;
            case "1": { } break;
            case "2": { } break;
            case "3": {
                    Damage = 10;
                    transform.up = pos.up;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "bullet3";
                } break;
            case "4": { } break;
            case "5": { } break;
            case "6": { } break;
            case "7": { } break;
        }
    }
    private void OnDisable()
    {
        position = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            Transform enemy = collision.transform;
            enemy.GetComponent<EnemyController>().
                CauseDamage(Damage);
        }
        
    }
}
