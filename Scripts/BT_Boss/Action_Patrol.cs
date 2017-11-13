using UnityEngine;
using BehaviorTreeFrame;

public class Action_Patrol : ActionNode<BossController> {
    float time = 0;
    int dir = 1;
    float patrolRadius = 10;
    public override void Enter()
    {
        Debug.Log("开始巡逻");
    }
    Vector3 Dir;
    public override BTResultStatus Execute()
    {
        
        if (Vector3.Distance(myEntity.transform.position, myEntity.PatrolPoint.position) > patrolRadius)
        {
            dir = 1;
            Dir = (myEntity.PatrolPoint.position - myEntity.transform.position).normalized;
        }
        else if (Vector3.Distance(myEntity.transform.position, myEntity.PatrolPoint.position) < 1f)
        {
            dir = -1;
            Dir = (myEntity.transform.position-myEntity.PatrolPoint.position).normalized;
        }
        
        if (myEntity.transform.up != Dir)
        {
            myEntity.transform.up = Vector3.MoveTowards(myEntity.transform.up, Dir, 0.1f);
        }
        //改变巡逻半径
        myEntity.transform.position += (myEntity.PatrolPoint.position - myEntity.transform.position).normalized
            * myEntity.Speed * Time.deltaTime * dir;
        //围绕着出生点绕圈巡逻
        myEntity.transform.RotateAround(
            myEntity.PatrolPoint.position, Vector3.forward, myEntity.Speed * Time.deltaTime);
        return BTResultStatus.Running;
    }
    public override void Exit()
    {
        Debug.Log("退出巡逻");
    }
}
