using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyBulletController : MonoBehaviour {
    //public GameObject showDamage;
    public int Damage = 1;//子弹伤害
    private Transform _enemyTransform;
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (isLazer)
        {
            if (_enemyTransform == null) return;
            if (bulletname == "enemylazer1")
            {
                transform.up = _enemyTransform.up;
                transform.position = _enemyTransform.position;
            }
            if (bulletname == "enemylazer2")
            {
                transform.RotateAround(_enemyTransform.position, Vector3.forward, 1f);
                //transform.LookAt(_enemyTransform.position);
                
            }
        }
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
    bool isLazer = false;
    public void InitState(Transform pos, string id)
    {
        _enemyTransform = pos;
        endTime = 0;
        time = 10;
        transform.position = pos.position;
        isLazer = false;
        switch (id)
        {
            case "0":
                {
                    Damage = 1;
                    transform.GetComponent<Rigidbody2D>().velocity = pos.up.normalized * 5;
                    transform.up = pos.up;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "enemybullet0";
                }
                break;
            case "1":
                {
                    time = 2;
                    Damage = 5;
                    isLazer = true;
                    transform.up = pos.up;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "enemylazer1";
                } break;
            case "2":
                {
                    Damage = 10;
                    time = 10;
                    isLazer = true;
                    transform.up = pos.up;
                    InvokeRepeating("Timer", 0, 0.1f);
                    bulletname = "enemylazer2";
                }
                break;
            case "3": { } break;
            case "4": { } break;
            case "5": { } break;
            case "6": { } break;
            case "7": { } break;
        }
    }
    private int targetcode;
    /// <summary>
    /// 碰撞事件
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.tag == "Player")
        {
            Transform player = collision.transform;
            if (!player.GetComponent<PlayerController>().IPName.Equals(Login.ownerIPName))
            {
                return;//只有自己的客户端才能判断造成伤害
            }
            //和上一次的目标不同
            if (targetcode!= player.GetHashCode())
            {
                targetcode = player.GetHashCode();
            }
            else
            {
                //只有激光才能重复伤害同一个目标
                if(!isLazer)return;
            }
            
            player.GetComponent<PlayerController>().
                DamageHandle(Damage);
            
            //ObjectPool.GetInstance().DestroyObject(gameObject, bulletname);
        }
        
    }
    private void OnDisable()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        endTime = 0;
        time = 10;
        targetcode = 0;
    }
}
