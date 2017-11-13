using UnityEngine;
using System.Collections;

public class EnemyLazerController : MonoBehaviour {
    public Transform position;
    public int Damage = 10;
    // Use this for initialization
    void Start()
    {

    }
    public void InitState(Transform pos,string id)
    {
        position = pos;
        switch (id)
        {
            case "0": { }break;
            case "1": { } break;
            case "2": { } break;
            case "3": { } break;
            case "4": { } break;
            case "5": { } break;
            case "6": { } break;
            case "7": { } break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = position.position;
        transform.up = position.up;
    }
    private void OnDisable()
    {
        position = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Transform player = collision.transform;
            player.GetComponent<EnemyController>().
                CauseDamage(Damage);
            Destroy(gameObject,1f);
        }
        
    }
}
