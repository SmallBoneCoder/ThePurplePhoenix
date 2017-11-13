using UnityEngine;
using System;

public class ArticleController : MonoBehaviour {
    public int Score = 1;
    public Action<int> ScoreHandle;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!collision.GetComponent<PlayerController>().IPName.Equals(Login.ownerIPName))
            {
                return;//只有自己的客户端才能判断造成伤害
            }
            collision.GetComponent<PlayerController>().DamageHandle(-1);
            if (collision.GetComponent<PlayerController>().IPName == Login.ownerIPName)
            {
                if (ScoreHandle != null)
                {
                    ScoreHandle.Invoke(Score);
                }
            }
            ObjectPool.GetInstance().DestroyObject(gameObject, "star");
        }
    }
}
