using UnityEngine;
using FSMFrame;
public class State_Patrol :  BaseState<EnemyController>{

    public override void Enter(EnemyController entity)
    {
        Debug.Log(entity.GetHashCode()+":开始巡逻");
    }
    float time = 0;
    int dir = 1;
    Vector3 Dir;
    public override void Execute(EnemyController entity)
    {
        //Debug.Log(entity.PatrolPoint.position - entity.transform.position);
        //time += Time.deltaTime;
        if ( time<=0)//一秒钟检查一次
        {
            time = 1f;
            if (checkTarget())
            {
                parent.changeState(parent.allStates["追击"]);
                return;//退出当前状态
            }
        }
        time -= Time.deltaTime;
        if (Vector3.Distance( entity.transform.position, entity.PatrolPoint.position) > 10f)
        {
            //Debug.Log(Vector3.Distance(entity.transform.position, entity.PatrolPoint.position));
            dir = 1;
            Dir = (entity.PatrolPoint.position - entity.transform.position).normalized;
            
        }
        else if(Vector3.Distance(entity.transform.position, entity.PatrolPoint.position)< 1f)
        {
            dir = -1;
            Dir = (entity.transform.position-entity.PatrolPoint.position).normalized;
            
        }

        if (entity.transform.up != Dir)
        {
            entity.transform.up = Vector3.MoveTowards(entity.transform.up, Dir, 0.1f);
        }
        //entity.transform.up = entity.transform.up * dir;
        //改变巡逻半径
        //Debug.Log((entity.PatrolPoint.position - entity.transform.position).normalized
        //    * entity.Speed * Time.deltaTime * dir);
        entity.transform.position += (entity.PatrolPoint.position- entity.transform.position).normalized
            * entity.Speed * Time.deltaTime*dir;
        //entity.transform.position=Vector3.MoveTowards(entity.transform.position,
        //    entity.PatrolPoint.position,
        //    entity.Speed * Time.deltaTime) * dir;
        //围绕着出生点绕圈巡逻
        entity.transform.RotateAround(
            entity.PatrolPoint.position, Vector3.forward, entity.Speed * Time.deltaTime);
        //if (entity.transform.up != Dir)
        //{
        //    entity.transform.up = Vector3.MoveTowards(entity.transform.up, Dir, 0.05f);
        //}
    }
    /// <summary>
    /// 检查目标
    /// </summary>
    /// <returns></returns>
    private bool checkTarget()
    {
        if (entity.Target != null&& entity.Target.gameObject.activeSelf)
        {
            
            float x = entity.Target.position.x;
            float y = entity.Target.position.y;
            float a = entity.transform.position.x;
            float b = entity.transform.position.y;
            //原目标在视野范围内继续追踪
            if ((x - a) * (x - a) + (y - b) * (y - b)<= entity.Sight * entity.Sight)
            {
                return true;
            }
        }
        Collider2D[] cos= Physics2D.OverlapCircleAll(entity.transform.position, entity.Sight);
        //Debug.Log("11");
        for (int i = 0; i < cos.Length; i++)
        {
            //Debug.Log(cos[i].gameObject);
            if (cos[i].gameObject.tag == "Player")
            {
                entity.Target = cos[i].transform;
                return true;//只追踪看到的第一个目标
            }
        }
        entity.Target = null;
        return false;
    }

    public override void Exit(EnemyController entity)
    {
        Debug.Log(entity.GetHashCode() + ":退出巡逻");
    }
}
