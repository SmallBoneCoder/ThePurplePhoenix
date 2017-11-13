using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BulletController : MonoBehaviour {
    // Use this for initialization
    
    Transform position;
    public int Damage = 1;//子弹伤害
    public CMDHandle destroyHandle;
	void Start () {
        //Camera.main.WorldToScreenPoint();
        
	}
    float endTime = 0;
    string bulletname;
    float time;
    /// <summary>
    /// 计时器
    /// </summary>
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
    bool isLazer = false;
    public void InitState(Transform pos, string id)
    {
        position = pos;
        Vector3 dir = pos.up;
        endTime = 0;
        time = 10;
        transform.position = pos.position;
        switch (id)
        {
            case "0": {
                    Damage = 1;
                    
                    transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 10;
                    transform.up = dir;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "bullet0";
                } break;
            case "1": {
                    Damage = 8;
                    transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 4;
                    transform.up = dir;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "bullet1";
                } break;
            case "2": {
                    Damage = 5;
                    transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 6;
                    transform.up = dir.normalized;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "bullet2";
                } break;
            case "3":
                {
                    isLazer = true;
                    Damage = 10;
                    time = 2;
                    transform.up = pos.up;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "bullet3";
                }
                break;
            case "4": { } break;
            case "5": { } break;
            case "6": { } break;
            case "7": { } break;
        }
    }
    // Update is called once per frame
    void Update () {
        if (isLazer)
        {
            transform.position = position.position;
            transform.up = position.up;
        }
	}
    /// <summary>
    /// 碰撞事件
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.transform.tag == "Enemy")
        {
            Transform enemy = collision.transform;
            enemy.GetComponent<EnemyController>().
                CauseDamage(Damage);
            //ObjectPool.GetInstance().DestroyObject(gameObject, bulletname);
        }
        if (collision.transform.tag == "Boss")
        {
            Transform enemy = collision.transform;
            enemy.GetComponent<BossController>().
                CauseDamage(Damage);
            
            //ObjectPool.GetInstance().DestroyObject(gameObject, bulletname);
        }

    }
    private void OnDisable()
    {
        position = null;
        isLazer = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        endTime = 0;
        time = 10;
    }
}
