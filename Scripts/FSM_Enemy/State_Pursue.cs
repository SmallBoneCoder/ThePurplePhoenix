using UnityEngine;
using FSMFrame;

public class State_Pursue : BaseState<EnemyController> {

    public override void Enter(EnemyController entity)
    {
        Debug.Log(entity.GetHashCode() + ":开始追击");
    }
    float _time = 0;
    Vector3 dir;
    public override void Execute(EnemyController entity)
    {
        
        _time -= Time.deltaTime;
        if (_time <=0)
        {
            _time= 3f;
            //检查目标是否还在
            if (!checkTarget())
            {
                parent.changeState(parent.allStates["巡逻"]);
                return;
            }
            //Debug.Log("11");
            //发射子弹
            //GameObject b = Object.Instantiate(entity.bullet);
            //b.transform.position = entity.transform.position + dir;
            //b.transform.up = dir;
            //b.GetComponent<Rigidbody2D>().
            //    AddForce(dir * 20,
            //    ForceMode2D.Impulse);
            //Object.Destroy(b, 5f);
            GameObject b = ObjectPool.GetInstance().CreateObject(entity.bullet, "enemybullet0");
            b.GetComponent<EnemyBulletController>().InitState(entity.transform.GetChild(1), "0");
        }
        dir = (entity.Target.position - entity.transform.position).normalized;
        if (entity.transform.up != dir)
        {
            entity.transform.up = Vector3.MoveTowards(entity.transform.up, dir, 0.05f);
        }
        //朝玩家靠近
        if (entity.Target == null) return;
        if (Vector3.Distance(entity.transform.position, entity.Target.position) > 2f)
        {
            //entity.transform.Translate(
            //    dir
            //    * entity.Speed * Time.deltaTime);
            entity.transform.position = Vector3.MoveTowards(entity.transform.position,
                entity.Target.position, entity.Speed * Time.deltaTime);
        }
    }
    public override void Exit(EnemyController entity)
    {
        Debug.Log(entity.GetHashCode() + ":结束追击");
    }
    /// <summary>
    /// 检查目标
    /// </summary>
    /// <returns></returns>
    private bool checkTarget()
    {
        if (entity.Target != null&&entity.Target.gameObject.activeSelf)
        {
            float x = entity.Target.position.x;
            float y = entity.Target.position.y;
            float a = entity.transform.position.x;
            float b = entity.transform.position.y;
            //原目标在视野范围内继续追踪
            if ((x - a) * (x - a) + (y - b) * (y - b) <= entity.Sight * entity.Sight)
            {
                return true;
            }
        }
        Collider2D[] cos = Physics2D.OverlapCircleAll(entity.transform.position, entity.Sight);
        for (int i = 0; i < cos.Length; i++)
        {
            if (cos[i].gameObject.tag == "Player")
            {
                entity.Target = cos[i].transform;
                return true;//只追踪看到的第一个目标
            }
        }
        entity.Target = null;
        return false;
    }
}
